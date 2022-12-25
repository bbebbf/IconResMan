namespace IconResMan
{
    public class ArgumentToResNameAdapter
    {
        public ArgumentToResNameAdapter(string argument)
        {
            Argument = argument;
            IdFound = false;
            if (Argument.StartsWith("#"))
            {
                var idstr = Argument.Substring(1);
                if (ushort.TryParse(idstr, out var id))
                {
                    IdFound = true;
                    Id = id;
                }
            }
        }

        public string Argument { get; init; }

        public bool IdFound { get; init; }

        public ushort Id { get; init; }

    }
}
