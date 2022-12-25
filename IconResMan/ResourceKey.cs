using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class ResourceKey
    {
        public ResourceKey(ResourceType type, ushort id)
        {
            ResourceType = type;
            ResourceName = new ResourceName(id);
        }

        public ResourceKey(IntPtr type, IntPtr name_or_id)
        {
            ResourceType = (ResourceType)type;
            ResourceName = new ResourceName(name_or_id);
        }

        public ResourceKey(ResourceType type, ResourceName name)
        {
            ResourceType = type;
            ResourceName = name;
        }

        public ResourceType ResourceType { get; init; }

        public ResourceName ResourceName { get; init; }

        public override string ToString()
        {
            return $"Resource {ResourceType} {ResourceName}";
        }
    }
}
