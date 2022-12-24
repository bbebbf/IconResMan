namespace IconResMan
{
    public class ResourceKeyLang
    {
        public ResourceKeyLang(ResourceKey key, ushort language)
        {
            Key = key;
            Language = language;
        }

        public ResourceKey Key { get; init; }
        public ushort Language { get; init; }

        public override string ToString()
        {
            return $"{Key}, language = {Language}";
        }
    }
}
