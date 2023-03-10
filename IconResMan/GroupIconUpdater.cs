using static IconResMan.WinapiTypes;

namespace IconResMan
{
    public class GroupIconUpdater
    {
        public GroupIconUpdater(ResourceLibrary target, ResourceLibrary source, LogProcessor logger)
        {
            _target = new GroupIconAccessor(target, logger);
            _source = new GroupIconAccessor(source, logger);
            _logger = logger;
        }

        public CommandResult UpdateGroupIcon(ResourceName? sourcename,
            ResourceName? targetname, ResourceName? target_newname)
        {
            return Update(false, sourcename, targetname, target_newname);
        }

        public CommandResult AddGroupIcon(ResourceName? sourcename,
            ResourceName? target_newname)
        {
            return Update(true, sourcename, null, target_newname);
        }

        private CommandResult Update(bool addalways,
            ResourceName? sourcename,
            ResourceName? targetname,
            ResourceName? target_newname)
        {
            var searchresult = _source.LoadGroupIconByName(sourcename);
            if (searchresult.CommandResult != CommandResult.Success || searchresult.GroupIcons == null)
                return searchresult.CommandResult;

            var alltargeticons = _target.Library.GetResources(ResourceType.ICON, null);
            if (alltargeticons == null)
                return CommandResult.Error;

            var targeticonids = new List<ushort>();
            foreach (var icon in alltargeticons)
                if (icon.ResourceName.Id != null)
                    targeticonids.Add((ushort)icon.ResourceName.Id);

            var nextIndexProvider = new NextIndexProvider(targeticonids.ToArray());

            if (_target.Library.BeginUpdate())
            {
                _logger.InfoVerbose(nameof(GroupIconUpdater), $"Update process started for file {_target.Library.Filename}.");
            }
            else
            {
                return CommandResult.Error;
            }

            var discardChanges = true;
            bool endUpdateSuccessful;
            try
            {
                foreach (var sourcegroup in searchresult.GroupIcons)
                {
                    if (!CopySourcegroup(nextIndexProvider, addalways, sourcegroup, targetname, target_newname))
                        return CommandResult.Error;
                }
                discardChanges = false;
            }
            finally
            {
                endUpdateSuccessful = _target.Library.EndUpdate(discardChanges);
                if (endUpdateSuccessful)
                {
                    _logger.InfoVerbose(nameof(GroupIconUpdater), $"Update process {(discardChanges ? "discarded" : "finished")} for file {_target.Library.Filename}.");
                }
            }
            return endUpdateSuccessful ? CommandResult.Success : CommandResult.Error;
        }

        private bool CopySourcegroup(NextIndexProvider nextIndexProvider, bool addalways, 
            GroupIcon sourcegroup, 
            ResourceName? targetname,
            ResourceName? target_newname)
        {
            GroupIcon? targetgroup_to_be_replaced = null;

            if (!addalways)
            {
                var targetsearchresult = _target.LoadGroupIcons(targetname);
                if (targetsearchresult.ErrorOccurred)
                    return false;

                if (targetname != null && targetsearchresult.CountBeforeLanguages == 0)
                {
                    _logger.Error(nameof(GroupIconUpdater), $"{ResourceType.GROUP_ICON} \"{targetname}\" not found in file {_target.Library.Filename}.");
                    return false;
                }
                targetgroup_to_be_replaced = targetsearchresult.GroupIcons?.Where(
                    g => g.KeyLang.Language == sourcegroup.KeyLang.Language).FirstOrDefault();
            }

            var availTargetIconIdx = new Queue<ushort>();
            if (targetgroup_to_be_replaced == null)
            {
                _logger.InfoVerbose(nameof(GroupIconUpdater), $"Adding {sourcegroup.KeyLang} ...");
            }
            else
            {
                _logger.InfoVerbose(nameof(GroupIconUpdater), $"Replacing {targetgroup_to_be_replaced.KeyLang} by {sourcegroup.KeyLang} ...");

                foreach (var icon in targetgroup_to_be_replaced.Members)
                {
                    if (icon.ReferencedByOtherGroups)
                        continue;
                    availTargetIconIdx.Enqueue(icon.Record.wID);
                }
            }

            foreach (var sourceicon in sourcegroup.Members)
            {
                var loadresult = _source.LoadMemberIcon(sourceicon);
                if (!loadresult.IconFound)
                    return false;

                var iconAdded = false;
                if (!availTargetIconIdx.TryDequeue(out ushort nextIconIdx))
                { 
                    nextIconIdx = nextIndexProvider.RequestNextIndex();
                    iconAdded = true;
                }

                var record = sourceicon.Record;
                record.wID = nextIconIdx;
                sourceicon.Record = record;

                var iconKeyLang = new ResourceKeyLang(new ResourceKey(ResourceType.ICON, nextIconIdx), sourceicon.Key.Language);
                if (_target.Library.UpdateResource(iconKeyLang, loadresult.StartPointer, loadresult.ByteCount))
                {
                    _logger.InfoVerbose(nameof(GroupIconUpdater), $"{iconKeyLang} {(iconAdded ? "added" : "updated")}.");
                }
                else
                {
                    return false;
                }
            }

            ResourceKeyLang targetgroupKey;
            if (targetgroup_to_be_replaced != null)
            {
                while (availTargetIconIdx.Count > 0)
                {
                    var tobeDeletedIdx = availTargetIconIdx.Dequeue();
                    var iconKeyLang = new ResourceKeyLang(new ResourceKey(ResourceType.ICON, tobeDeletedIdx), targetgroup_to_be_replaced.KeyLang.Language);
                    if (_target.Library.DeleteResource(iconKeyLang))
                    {
                        _logger.InfoVerbose(nameof(GroupIconUpdater), $"Unused {iconKeyLang} deleted.");
                    }
                    else
                    {
                        return false;
                    }
                }
                targetgroupKey = targetgroup_to_be_replaced.KeyLang;
            }
            else
            {
                targetgroupKey = sourcegroup.KeyLang;
            }

            if (target_newname != null)
            {
                targetgroupKey = new ResourceKeyLang(new ResourceKey(targetgroupKey.Key.ResourceType, target_newname), targetgroupKey.Language);
                if (targetgroup_to_be_replaced != null && !targetgroup_to_be_replaced.KeyLang.Key.ResourceName.Equals(target_newname))
                {
                    if (_target.Library.DeleteResource(targetgroup_to_be_replaced.KeyLang))
                    {
                        _logger.InfoVerbose(nameof(GroupIconUpdater), $"{targetgroup_to_be_replaced.KeyLang} deleted.");
                    }
                    else
                    {
                        return false;
                    }

                }
            }

            using var sourcegroupintptr = new GroupIconIntPtr(sourcegroup);
            if (_target.Library.UpdateResource(targetgroupKey, sourcegroupintptr.IntPtr, (uint)sourcegroupintptr.BufferSize))
            {
                _logger.InfoVerbose(nameof(GroupIconUpdater), $"{targetgroupKey} updated.");
            }
            else
            {
                return false;
            }
            return true;
        }

        private readonly GroupIconAccessor _target;
        private readonly GroupIconAccessor _source;
        private readonly LogProcessor _logger;
    }
}
