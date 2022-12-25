using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class GroupIconDeleter
    {
        public GroupIconDeleter(ResourceLibrary target, LogProcessor logger)
        {
            _target = new GroupIconAccessor(target, logger);
            _logger = logger;
        }

        public CommandResult DeleteGroupIcon(ResourceName? targetname)
        {
            var searchresult = _target.LoadGroupIconByName(targetname);
            if (searchresult.CommandResult != CommandResult.Success || searchresult.GroupIcons == null)
                return searchresult.CommandResult;

            if (_target.Library.BeginUpdate())
            {
                _logger.InfoVerbose(nameof(GroupIconDeleter), $"Update process started for file {_target.Library.Filename}.");
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
                    if (!DeleteGroup(targetgroup))
                        return CommandResult.Error;
                }
                discardChanges = false;
            }
            finally
            {
                endUpdateSuccessful = _target.Library.EndUpdate(discardChanges);
                if (endUpdateSuccessful)
                {
                    _logger.InfoVerbose(nameof(GroupIconDeleter), $"Update process {(discardChanges ? "discarded" : "finished")} for file {_target.Library.Filename}.");
                }
            }
            return endUpdateSuccessful ? CommandResult.Success : CommandResult.Error;
        }

        private bool DeleteGroup(GroupIcon targetgroup_to_be_deleted)
        {
            var availTargetIconIdx = new Queue<ushort>();

            foreach (var icon in targetgroup_to_be_deleted.Members)
            {
                if (icon.ReferencedByOtherGroups)
                    continue;
                availTargetIconIdx.Enqueue(icon.Record.wID);
            }

            _logger.InfoVerbose(nameof(GroupIconDeleter), $"Deleting {targetgroup_to_be_deleted.KeyLang} ...");

            while (availTargetIconIdx.Count > 0)
            {
                var tobeDeletedIdx = availTargetIconIdx.Dequeue();
                var iconKeyLang = new ResourceKeyLang(new ResourceKey(ResourceType.ICON, tobeDeletedIdx),
                    targetgroup_to_be_deleted.KeyLang.Language);
                if (_target.Library.DeleteResource(iconKeyLang))
                {
                    _logger.InfoVerbose(nameof(GroupIconDeleter), $"{iconKeyLang} deleted.");
                }
                else
                {
                    return false;
                }
            }

            if (_target.Library.DeleteResource(targetgroup_to_be_deleted.KeyLang))
            {
                _logger.InfoVerbose(nameof(GroupIconDeleter), $"{targetgroup_to_be_deleted.KeyLang} deleted.");
            }
            else
            {
                return false;
            }
            return true;
        }

        private readonly GroupIconAccessor _target;
        private readonly LogProcessor _logger;
    }
}
