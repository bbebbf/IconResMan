using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class GroupIconRenamer
    {
        public GroupIconRenamer(ResourceLibrary target, ILogger logger)
        {
            _target = new GroupIconAccessor(target, logger);
            _logger = logger;
        }

        public CommandResult RenameGroupIcon(ResourceName? targetname, ResourceName newname)
        {
            var searchresult = _target.LoadGroupIconByName(targetname);
            if (searchresult.CommandResult != CommandResult.Success)
                return searchresult.CommandResult;

            if (_target.Library.BeginUpdate())
            {
                _logger.InfoVerbose(nameof(GroupIconRenamer), $"Update process started for file {_target.Library.Filename}.");
            }
            else
            {
                return CommandResult.Error;
            }

            var discardChanges = true;
            bool endUpdateSuccessful;
            try
            {
                foreach (var targetgroup in searchresult.GroupIcons)
                {
                    if (!RenameGroup(targetgroup, newname))
                        return CommandResult.Error;
                }
                discardChanges = false;
            }
            finally
            {
                endUpdateSuccessful = _target.Library.EndUpdate(discardChanges);
                if (endUpdateSuccessful)
                {
                    _logger.InfoVerbose(nameof(GroupIconRenamer), $"Update process {(discardChanges ? "discarded" : "finished")} for file {_target.Library.Filename}.");
                }
            }
            return endUpdateSuccessful ? CommandResult.Success : CommandResult.Error;
        }

        private bool RenameGroup(GroupIcon targetgroup_to_be_renamed, ResourceName newname)
        {
            _logger.InfoVerbose(nameof(GroupIconRenamer), $"Renaming {targetgroup_to_be_renamed.KeyLang} ...");

            var new_targetkeylang = new ResourceKeyLang(
                new ResourceKey(targetgroup_to_be_renamed.KeyLang.Key.ResourceType, newname),
                targetgroup_to_be_renamed.KeyLang.Language);

            using var targetgroupintptr = new GroupIconIntPtr(targetgroup_to_be_renamed);
            if (_target.Library.UpdateResource(new_targetkeylang, targetgroupintptr.IntPtr, (uint)targetgroupintptr.BufferSize))
            {
                _logger.InfoVerbose(nameof(GroupIconRenamer), $"{new_targetkeylang} updated.");
                if (_target.Library.DeleteResource(targetgroup_to_be_renamed.KeyLang))
                {
                    _logger.InfoVerbose(nameof(GroupIconRenamer), $"{targetgroup_to_be_renamed.KeyLang} deleted.");
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private readonly GroupIconAccessor _target;
        private readonly ILogger _logger;
    }
}
