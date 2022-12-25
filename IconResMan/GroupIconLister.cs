namespace IconResMan
{
    public class GroupIconLister
    {
        public GroupIconLister(ResourceLibrary source, LogProcessor logger)
        {
            _source = new GroupIconAccessor(source, logger);
        }

        public CommandResult List(IResourceWriter outputter)
        {
            var sourcesearchresult = _source.LoadGroupIcons();
            if (sourcesearchresult.ErrorOccurred || sourcesearchresult.GroupIcons == null)
                return CommandResult.Error;

            foreach(var icon in sourcesearchresult.GroupIcons)
                outputter.WriteGroupIcon(icon);

            return CommandResult.Success;
        }

        private readonly GroupIconAccessor _source;
    }
}
