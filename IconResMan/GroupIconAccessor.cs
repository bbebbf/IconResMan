using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class GroupIconAccessor
    {
        public struct LoadGroupIconsResult
        {
            public LoadGroupIconsResult()
            {
                CommandResult = CommandResult.Error;
                CountBeforeLanguages = 0;
                GroupIcons = null;
            }

            public LoadGroupIconsResult(CommandResult commandresult)
            {
                CommandResult = commandresult;
                CountBeforeLanguages = 0;
                GroupIcons = null;
            }

            public bool WasSuccessful { get { return CommandResult == CommandResult.Success; } }

            public bool ErrorOccurred { get { return !WasSuccessful; } }

            public CommandResult CommandResult;
            public int CountBeforeLanguages;
            public IEnumerable<GroupIcon>? GroupIcons;
        }

        public struct LoadMemberIconResult
        {
            public LoadMemberIconResult()
            {
                IconFound = false;
                ByteCount = 0;
                StartPointer = IntPtr.Zero;
            }

            public bool IconFound;
            public uint ByteCount;
            public IntPtr StartPointer;
        }

        public GroupIconAccessor(ResourceLibrary library, LogProcessor logger)
        {
            Library = library;
            _logger = logger;
        }

        public LoadGroupIconsResult LoadGroupIcons(ResourceName? name = null)
        {
            if (name == null)
            {
                _logger.InfoVerbose(nameof(GroupIconAccessor), $"Loading all group icons from file {Library.Filename}");
            }
            else
            {
                _logger.InfoVerbose(nameof(GroupIconAccessor), $"Loading group icons with {name.Title()} = {name} from file {Library.Filename}");
            }
            var list = Library.GetResources(ResourceType.GROUP_ICON, name);
            if (list == null)
            {
                _logger.Error(nameof(GroupIconAccessor), "Loading group icon resources failed.");
                return new LoadGroupIconsResult();
            }

            var groups = new List<GroupIcon>();
            var result = new LoadGroupIconsResult
            {
                CommandResult = CommandResult.Success,
                CountBeforeLanguages = list.Count,
                GroupIcons = groups
            };

            _usedMemberIcons.Clear();
            foreach (var item in list)
                if (!LoadGroupIconForLanguagesInternal(item, groups))
                    return new LoadGroupIconsResult();

            return result;
        }

        public LoadGroupIconsResult LoadGroupIconByName(ResourceName? name = null)
        {
            var searchresult = LoadGroupIcons(name);
            if (searchresult.CommandResult != CommandResult.Success || searchresult.GroupIcons == null)
                return new LoadGroupIconsResult(CommandResult.Error);

            if (searchresult.CountBeforeLanguages > 1)
            {
                _logger.Error(nameof(GroupIconAccessor), $"More than one ({searchresult.CountBeforeLanguages}) group icon found in {Library.Filename}. Please specify resoure name.");
                return new LoadGroupIconsResult(CommandResult.ErrorResourceAmbiguous);
            }
            else if (searchresult.CountBeforeLanguages == 0)
            {
                _logger.Error(nameof(GroupIconAccessor), $"No group icon found in {Library.Filename}.");
                return new LoadGroupIconsResult(CommandResult.ErrorResourceNotFound);
            }

            var found1stGroupIcon = searchresult.GroupIcons.FirstOrDefault();
            if (found1stGroupIcon != null)
                _logger.InfoVerbose(nameof(GroupIconAccessor), $"{found1stGroupIcon.KeyLang} found in file {Library.Filename}.");

            return new LoadGroupIconsResult
            {
                CommandResult = CommandResult.Success,
                CountBeforeLanguages = searchresult.CountBeforeLanguages,
                GroupIcons = searchresult.GroupIcons
            };
        }

        public LoadMemberIconResult LoadMemberIcon(GroupIconMemberIcon membericon)
        {
            var key = new ResourceKey(ResourceType.ICON, membericon.Record.wID);
            var hRes = Library.FindResource(key, membericon.Key.Language);
            if (hRes == IntPtr.Zero)
            {
                _logger.Warn(nameof(GroupIconAccessor), $"{key} not found for language {membericon.Key.Language}.");
                return new LoadMemberIconResult();
            }

            var bytecount = Library.SizeofResource(hRes);
            if (bytecount == 0)
            {
                _logger.Error(nameof(GroupIconAccessor), $"Unable to get the byte size of {key}, language {membericon.Key.Language}.");
                return new LoadMemberIconResult();
            }

            var lpResLock = LoadAndLock(hRes);
            if (lpResLock == IntPtr.Zero)
            {
                _logger.Error(nameof(GroupIconAccessor), $"Unable to load {key}, language {membericon.Key.Language}.");
                return new LoadMemberIconResult();
            }
            return new LoadMemberIconResult
            {
                IconFound = true,
                ByteCount = bytecount,
                StartPointer = lpResLock
            };
        }

        private bool LoadGroupIconForLanguagesInternal(ResourceKey key, List<GroupIcon> groups)
        {
            var languages = Library.GetResourceLanguages(key);
            if (languages == null)
            {
                _logger.Error(nameof(GroupIconAccessor), $"No languages found for {key}.");
                return false;
            }

            foreach (var language in languages)
            {
                var keylang = new ResourceKeyLang(key, language);
                if (!ReadIconGroupFromFile(keylang, groups))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ReadIconGroupFromFile(ResourceKeyLang key, List<GroupIcon> groups)
        {
            IntPtr hRes = Library.FindResource(key.Key, key.Language);
            if (hRes == IntPtr.Zero)
            {
                _logger.Warn(nameof(GroupIconAccessor), $"{key} not found for language {key.Language}.");
                return false;
            }
            var lpResLock = LoadAndLock(hRes);
            if (lpResLock == IntPtr.Zero)
            {
                _logger.Error(nameof(GroupIconAccessor), $"Unable to load {key}, language {key.Language}.");
                return false;
            }

            var group = new GroupIcon(key);
            group.ReadFromIntPtr(lpResLock, _usedMemberIcons);
            groups.Add(group);
            return true;
        }

        public ResourceLibrary Library { get; init; }

        private readonly LogProcessor _logger;

        private Dictionary<ushort, GroupIconMemberIcon> _usedMemberIcons = new();

        private IntPtr LoadAndLock(IntPtr intPtr)
        {
            var hResLoad = Library.LoadResource(intPtr);
            if (hResLoad == IntPtr.Zero)
            {
                _logger.Error(nameof(GroupIconAccessor), $"{Library.Filename} wasn't successful.");
                return IntPtr.Zero;
            }
            var lpResLock = ResourceLibrary.LockResource(hResLoad);
            if (lpResLock == IntPtr.Zero)
            {
                _logger.Error(nameof(GroupIconAccessor), $"{Library.Filename} wasn't successful.");
                return IntPtr.Zero;
            }
            return lpResLock;
        }
    }
}
