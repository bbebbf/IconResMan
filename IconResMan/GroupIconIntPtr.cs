using System.Runtime.InteropServices;
using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public sealed class GroupIconIntPtr : CachedIntPtr
    {
        public GroupIconIntPtr(GroupIcon groupIcon)
        {
            GroupIcon = groupIcon;
        }

        protected override void GetCachedIntPtr(ref IntPtr handle, ref bool needsToBeReleased)
        {
            handle = Marshal.AllocHGlobal(BufferSize);
            needsToBeReleased = true;
            WriteToIntPtr(handle);
        }

        private void WriteToIntPtr(IntPtr cachedIntPtr)
        {
            var headerrecord = GroupIcon.Record;
            headerrecord.wCount = (ushort)GroupIcon.Members.Count;

            var entrybytesize = Marshal.SizeOf<GroupIconRsrcEntry>();
            Marshal.StructureToPtr<GroupIconRsrcHeader>(headerrecord, cachedIntPtr, false);
            if (GroupIcon.Members.Count == 0)
                return;

            var entryptr = new IntPtr(cachedIntPtr.ToInt64() + Marshal.SizeOf<GroupIconRsrcHeader>());
            ushort i = 0;
            var member = GroupIcon.Members.First();
            while (true)
            {
                Marshal.StructureToPtr<GroupIconRsrcEntry>(member.Record, entryptr, false);
                i++;

                if (i >= headerrecord.wCount)
                {
                    break;
                }
                entryptr = new IntPtr(entryptr.ToInt64() + entrybytesize);
                member = GroupIcon.Members[i];
            }
        }

        public GroupIcon GroupIcon { get; init; }

        public int BufferSize
        {
            get
            {
                return Marshal.SizeOf<GroupIconRsrcHeader>() + 
                    (GroupIcon.Members.Count * Marshal.SizeOf<GroupIconRsrcEntry>());
            }
        }
    }
}
