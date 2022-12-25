namespace IconResMan
{
    public class IconResManager : IIconResManager
    {
        public int DeleteGroupIcon(string targetPath, string? targetResource)
        {
            if (!FileExists(targetPath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            using var targetlib = new ResourceLibrary(targetPath, logProcessor);
            using var target_resname = targetResource != null ? new ResourceName(new ArgumentToResNameAdapter(targetResource)) : null;

            var deleter = new GroupIconDeleter(targetlib, logProcessor);
            return CommandResultToInt(deleter.DeleteGroupIcon(target_resname));
        }

        public int ListGroupIcons(string targetPath, IResourceWriter resourceWriter)
        {
            if (!FileExists(targetPath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            using var targetlib = new ResourceLibrary(targetPath, logProcessor);
            var lister = new GroupIconLister(targetlib, logProcessor);
            return CommandResultToInt(lister.List(resourceWriter));
        }

        public int RenameGroupIcon(string targetPath, string? targetResource, string newResourcename)
        {
            if (!FileExists(targetPath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            using var targetlib = new ResourceLibrary(targetPath, logProcessor);
            using var target_resname = targetResource != null ? new ResourceName(new ArgumentToResNameAdapter(targetResource)) : null;
            using var new_resname = new ResourceName(new ArgumentToResNameAdapter(newResourcename));
            var renamer = new GroupIconRenamer(targetlib, logProcessor);
            return CommandResultToInt(renamer.RenameGroupIcon(target_resname, new_resname));
        }

        public void SetLogger(ILogger logger)
        {
            logProcessor.Logger = logger;
        }

        public void SetVerbosity(bool verbose)
        {
            logProcessor.Verbose = verbose;
        }

        public int AddGroupIcon(string targetPath, string sourcePath, string? sourceResource, string? newResourcename)
        {
            if (!FileExists(targetPath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            if (!FileExists(sourcePath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            using var sourcelib = new ResourceLibrary(sourcePath, logProcessor);
            using var targetlib = new ResourceLibrary(targetPath, logProcessor);

            using var source_resname = sourceResource != null ? new ResourceName(new ArgumentToResNameAdapter(sourceResource)) : null;
            using var newresname = newResourcename != null ? new ResourceName(new ArgumentToResNameAdapter(newResourcename)) : null;

            var updater = new GroupIconUpdater(sourcelib, targetlib, logProcessor);
            return CommandResultToInt(updater.AddGroupIcon(source_resname, newresname));
        }

        public int UpdateGroupIcon(string targetPath, string sourcePath, string? sourceResource, string? targetResource, string? newResourcename)
        {
            if (!FileExists(targetPath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            if (!FileExists(sourcePath))
                return CommandResultToInt(CommandResult.ErrorFileNotFound);

            using var sourcelib = new ResourceLibrary(sourcePath, logProcessor);
            using var targetlib = new ResourceLibrary(targetPath, logProcessor);

            using var source_resname = sourceResource != null ? new ResourceName(new ArgumentToResNameAdapter(sourceResource)) : null;
            using var target_resname = targetResource != null ? new ResourceName(new ArgumentToResNameAdapter(targetResource)) : null;
            using var newresname = newResourcename != null ? new ResourceName(new ArgumentToResNameAdapter(newResourcename)) : null;

            var updater = new GroupIconUpdater(sourcelib, targetlib, logProcessor);
            return CommandResultToInt(updater.UpdateGroupIcon(source_resname, target_resname, newresname));
        }

        private static int CommandResultToInt(CommandResult actionresult)
        {
            return (int)actionresult;
        }

        private bool FileExists(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }

            logProcessor.Error(nameof(IconResManager), $"File {path} does not exist.");
            return false;
        }

        private readonly LogProcessor logProcessor = new();
    }
}
