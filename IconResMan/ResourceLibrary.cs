using System.Runtime.InteropServices;
using System.Text;
using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public sealed class ResourceLibrary : SafeHandle
    {
        public ResourceLibrary(string filename, ILogger logger) : base(IntPtr.Zero, true)
        {
            Filename = filename;
            Flags = LOAD_LIBRARY_AS_DATAFILE;
            _logger = logger;
        }

        public bool Load()
        {
            if (!IsInvalid)
                ReleaseHandle();

            LastError = 0;
            var ptr = LoadLibraryEx(Filename, IntPtr.Zero, Flags);
            if (ptr == IntPtr.Zero)
            {
                ProcessLastError(nameof(Load));
                return false;
            }
            else
            {
                base.SetHandle(ptr);
                return true;
            }
        }

        public IntPtr FindResource(ResourceKey key, ushort language)
        {
            if (!LoadLibraryIfNeeded())
                return IntPtr.Zero;

            LastError = 0;
            var ptr = FindResourceEx(this.handle, (IntPtr)key.ResourceType, key.ResourceName.IntPtr, language);
            if (ptr == IntPtr.Zero)
            {
                ProcessLastError(nameof(FindResource));
            }
            return ptr;
        }

        public List<ushort>? GetResourceLanguages(ResourceKey key)
        {
            if (!LoadLibraryIfNeeded())
                return null;

            LastError = 0;
            var result = new List<ushort>();
            var gchandle = GCHandle.Alloc(result);
            try
            {
                if (!EnumResourceLanguages(this.handle,
                    (IntPtr)key.ResourceType,
                    key.ResourceName.IntPtr,
                    new EnumResLangDelegate(EnumResLangs),
                    GCHandle.ToIntPtr(gchandle)))
                {
                    ProcessLastError(nameof(GetResourceLanguages));
                    return null;
                }
                return result;
            }
            finally
            {
                gchandle.Free();
            }
        }

        public IntPtr LoadResource(IntPtr resptr)
        {
            if (!LoadLibraryIfNeeded())
                return IntPtr.Zero;

            LastError = 0;
            var ptr = LoadResource(this.handle, resptr);
            if (ptr == IntPtr.Zero)
            {
                ProcessLastError(nameof(LoadResource));
            }
            return ptr;
        }

        public static IntPtr LockResource(IntPtr resptr)
        {
            return LockResourceInternal(resptr);
        }

        public uint SizeofResource(IntPtr resptr)
        {
            if (!LoadLibraryIfNeeded())
                return 0;

            LastError = 0;
            var size = SizeofResource(this.handle, resptr);
            if (size == 0)
            {
                ProcessLastError(nameof(SizeofResource));
            }
            return size;
        }

        public List<ResourceKey>? GetResources(ResourceType type, ResourceName? resourcename, bool errorOnNotFound = false)
        {
            if (!LoadLibraryIfNeeded())
                return null;

            LastError = 0;
            var result = new List<ResourceKey>();
            var gchandle = GCHandle.Alloc(new EnumResNamesData(result, resourcename));
            try
            {
                if (!EnumResourceNamesW(this.handle, 
                    (IntPtr)type, 
                    new EnumResNameDelegate(EnumResNames),
                    GCHandle.ToIntPtr(gchandle)))
                {
                    if (ProcessLastError(nameof(GetResources), (error) =>
                        {
                            return errorOnNotFound || (error != ERROR_RESOURCE_TYPE_NOT_FOUND);
                        }
                    ))
                    {
                        return null;
                    }
                }
                return result;
            }
            finally
            {
                gchandle.Free();
            }
        }

        public bool BeginUpdate()
        {
            if (!LoadLibraryIfNeeded())
                return false;

            LastError = 0;
            _updateHandle = BeginUpdateResource(Filename, false);
            if (_updateHandle == IntPtr.Zero)
            {
                ProcessLastError(nameof(BeginUpdate));
                return false;
            }
            return true;
        }

        public bool EndUpdate(bool discard)
        {
            if (!LoadLibraryIfNeeded())
                return false;

            LastError = 0;
            if (!EndUpdateResource(_updateHandle, discard))
            {
                ProcessLastError(nameof(EndUpdate));
                return false;
            }
            return true;
        }

        public bool UpdateResource(ResourceKeyLang key, IntPtr data, uint dataSize)
        {
            if (!LoadLibraryIfNeeded())
                return false;

            LastError = 0;
            if (!UpdateResource(_updateHandle, (IntPtr)key.Key.ResourceType, key.Key.ResourceName.IntPtr, key.Language, data, dataSize))
            {
                ProcessLastError(nameof(UpdateResource));
                return false;
            }
            return true;
        }

        public bool DeleteResource(ResourceKeyLang key)
        {
            if (!LoadLibraryIfNeeded())
                return false;

            LastError = 0;
            if (!UpdateResource(_updateHandle, (IntPtr)key.Key.ResourceType, key.Key.ResourceName.IntPtr, key.Language, IntPtr.Zero, 0))
            {
                ProcessLastError(nameof(DeleteResource));
                return false;
            }
            return true;
        }

        public override bool IsInvalid
        {
            get { return this.handle == IntPtr.Zero; }
        }

        public int LastError { get; private set; } = 0;

        public string Filename { get; init; }

        public uint Flags { get; init; }

        protected override bool ReleaseHandle()
        {
            return FreeLibrary(this.handle);
        }

        private readonly ILogger _logger;
        private IntPtr _updateHandle = IntPtr.Zero;

        private bool LoadLibraryIfNeeded()
        {
            if (IsInvalid)
                return Load();
            else
                return true;
        }

        private bool ProcessLastError(string originator, LastErrorIsErrorDelegate? lasterrorIsError = null)
        {
            LastError = Marshal.GetLastWin32Error();
            if (LastError == 0)
                return false;

            if (lasterrorIsError != null)
                if (!lasterrorIsError(LastError))
                    return false;

            _logger.Error(originator, LastError);
            return true;
        }

        private bool EnumResNames(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
        {
            var gch = GCHandle.FromIntPtr(lParam);
            if (gch.Target == null)
                return false;

            var data = (EnumResNamesData)gch.Target;
            using var name = new ResourceName(lpszName);
            if (data.ResourceName == null || data.ResourceName.Equals(name)) 
                data.Result.Add(new ResourceKey(lpszType, lpszName));
            return true;
        }

        private bool EnumResLangs(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, ushort wIDLanguage, IntPtr lParam)
        {
            var gch = GCHandle.FromIntPtr(lParam);
            if (gch.Target == null)
                return false;

            var list = (List<ushort>)gch.Target;
            list.Add(wIDLanguage);
            return true;
        }

        private delegate bool LastErrorIsErrorDelegate(int lasterror);

        private class EnumResNamesData
        {
            public EnumResNamesData(List<ResourceKey> resultlist, ResourceName? resourcename)
            {
                Result = resultlist;
                ResourceName = resourcename;
            }

            public List<ResourceKey> Result { get; init; }
            public ResourceName? ResourceName { get; init; }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpType, IntPtr lpName, ushort wLanguage);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", EntryPoint = "LockResource")]
        private static extern IntPtr LockResourceInternal(IntPtr hResData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool EnumResourceNamesW(
          IntPtr hModule,
          IntPtr lpszType,
          EnumResNameDelegate lpEnumFunc,
          IntPtr lParam);

        private delegate bool EnumResNameDelegate(
          IntPtr hModule,
          IntPtr lpszType,
          IntPtr lpszName,
          IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool EnumResourceLanguages(
            IntPtr hModule, 
            IntPtr lpType, 
            IntPtr lpName, 
            EnumResLangDelegate lpEnumFunc, 
            IntPtr lParam);

        private delegate bool EnumResLangDelegate(
            IntPtr hModule, 
            IntPtr lpszType, 
            IntPtr lpszName, 
            ushort wIDLanguage, 
            IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr BeginUpdateResource(string pFileName,[MarshalAs(UnmanagedType.Bool)] bool bDeleteExistingResources);


        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool EndUpdateResource(IntPtr hUpdate, [MarshalAs(UnmanagedType.Bool)] bool fDiscard);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, ushort wLanguage, IntPtr lpData, uint cbData);


        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, 
            uint dwMessageId, uint dwLanguageId, IntPtr lpBuffer, uint nSize, IntPtr pArguments);
    }
}
