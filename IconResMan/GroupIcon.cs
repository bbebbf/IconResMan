using System.Runtime.InteropServices;
using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class GroupIcon
    {
        public GroupIcon(ResourceKeyLang key)
        {
            KeyLang = key;
        }

        public ResourceKeyLang KeyLang { get; init; }
        public IntPtr IntPtr { get; private set; }
        public int BufferSize { get; private set; }
        public GroupIconRsrcHeader Record { get; private set; }
        public List<GroupIconMemberIcon> Members { get; init; } = new();

        public void ReadFromIntPtr(IntPtr headerptr, Dictionary<ushort, GroupIconMemberIcon> usedMemberIcons)
        {
            IntPtr = headerptr;
            BufferSize = Marshal.SizeOf<GroupIconRsrcHeader>();
            Record = Marshal.PtrToStructure<GroupIconRsrcHeader>(headerptr);
            Members.Clear();
            if (Record.wCount <= 0)
                return;

            var entryptr = new IntPtr(headerptr.ToInt64() + Marshal.SizeOf<GroupIconRsrcHeader>());
            var entrybytesize = Marshal.SizeOf<GroupIconRsrcEntry>();
            while (true)
            {
                var iconentry = Marshal.PtrToStructure<GroupIconRsrcEntry>(entryptr);
                 var member = new GroupIconMemberIcon(
                    new ResourceKeyLang(new ResourceKey(ResourceType.ICON, iconentry.wID), KeyLang.Language), iconentry);
                Members.Add(member);
                BufferSize += entrybytesize;

                if (usedMemberIcons.TryGetValue(iconentry.wID, out var othermember))
                {
                    othermember.ReferencedByOtherGroups = true;
                    member.ReferencedByOtherGroups = true;
                }
                else
                {
                    usedMemberIcons.Add(iconentry.wID, member);
                }

                if (Members.Count >= Record.wCount)
                {
                    break;
                }
                entryptr = new IntPtr(entryptr.ToInt64() + entrybytesize);
            }
        }
    }
}
