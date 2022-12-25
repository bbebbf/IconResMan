namespace IconResMan
{
    public interface IIconResManager
    {
        public void SetLogger(ILogger logger);

        public void SetVerbosity(bool verbose);

        public int AddGroupIcon(string targetPath, string sourcePath, string? sourceResource, string? newResourcename);

        public int UpdateGroupIcon(string targetPath, string sourcePath, string? sourceResource, string? targetResource, string? newResourcename);

        public int DeleteGroupIcon(string targetPath, string? targetResource);

        public int RenameGroupIcon(string targetPath, string? targetResource, string newResourcename);

        public int ListGroupIcons(string targetPath, IResourceWriter resourceWriter);
    }
}
