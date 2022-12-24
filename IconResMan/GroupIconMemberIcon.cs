using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class GroupIconMemberIcon
    {
        public GroupIconMemberIcon(ResourceKeyLang key, GroupIconRsrcEntry record)
        {
            Key = key;
            Record = record;
        }

        public ResourceKeyLang Key { get; init; }
        public GroupIconRsrcEntry Record { get; set; }
        public bool ReferencedByOtherGroups { get; set; } = false;
    }
}
