namespace IconResMan
{
    public class NextIndexProvider
    {
        public NextIndexProvider()
        {
        }

        public NextIndexProvider(ushort[] alreadyUsedIndexes)
        {
            foreach (ushort index in alreadyUsedIndexes)
                UsedIndexes.TryAdd(index, false);
        }

        public ushort RequestNextIndex()
        {
            while (true)
            {
                NextIndex++;
                if (!UsedIndexes.ContainsKey(NextIndex))
                    return NextIndex;
            }
        }

        private readonly Dictionary<ushort, bool> UsedIndexes = new();
        private ushort NextIndex = 0;
    }
}
