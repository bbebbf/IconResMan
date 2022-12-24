// See https://aka.ms/new-console-template for more information
using IconResMan;

var IconResManExe = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

const int MINIMUM_ARGS_LENGHTH = 2;
const int PARAM_IDX_ACTION = 0;
const int PARAM_IDX_TARGETFILENAME = 1;

const string PARAM_NAME_SOURCE = "sfile";
const string PARAM_NAME_SOURCE_RESOURCENAME = "sres";
const string PARAM_NAME_TARGET_RESOURCENAME = "tres";
const string PARAM_NAME_SWITCH_ADD_ALWAYS = "add";
const string PARAM_NAME_NEW_RESOURCENAME = "newname";

try
{
    if (args.Length < MINIMUM_ARGS_LENGHTH)
    {
        PrintHelp();
        return 1;
    }

    var command = args[PARAM_IDX_ACTION];
    var targetfilename = args[PARAM_IDX_TARGETFILENAME];

    if (!FileExists(targetfilename))
        return ActionResultToInt(CommandResult.ErrorFileNotFound);

    switch(command)
    {
        case "update":
            return ActionResultToInt(UpdateGoupIcon(targetfilename));
        case "delete":
            return ActionResultToInt(DeleteGoupIcon(targetfilename));
        case "rename":
            return ActionResultToInt(RenameGoupIcon(targetfilename));
        case "list":
            return ActionResultToInt(ListGoupIcon(targetfilename));
    }
    PrintHelp();
    return ActionResultToInt(CommandResult.ErrorUnknownCommand);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Exception occurred: [{ex.GetType()}] {ex.Message}");
    return ActionResultToInt(CommandResult.Error);
}

int ActionResultToInt(CommandResult actionresult)
{
    return (int)actionresult;
}

CommandResult UpdateGoupIcon(string targetfilename)
{
    var logger = new ConsoleLogger();

    var sourcefilename = ParamTools.GetParamValue(PARAM_NAME_SOURCE);
    if (!sourcefilename.Item1)
    {
        logger.Error(nameof(IconResManExe), $"Parameter \"-{PARAM_NAME_SOURCE}\" not found.");
        return CommandResult.ErrorParameterNotFound;
    }
    if (!FileExists(sourcefilename.Item2))
    {
        return CommandResult.ErrorFileNotFound;
    }
    var addalways = ParamTools.FindParamSwitch(PARAM_NAME_SWITCH_ADD_ALWAYS);

    ArgumentToResNameAdapter? sourcename_arg = null;
    ArgumentToResNameAdapter? targetname_arg = null;
    ArgumentToResNameAdapter? newresname_arg = null;

    var resourcename = ParamTools.GetParamValue(PARAM_NAME_SOURCE_RESOURCENAME);
    if (resourcename.Item1)
        sourcename_arg = new ArgumentToResNameAdapter(resourcename.Item2);
    resourcename = ParamTools.GetParamValue(PARAM_NAME_TARGET_RESOURCENAME);
    if (resourcename.Item1)
        targetname_arg = new ArgumentToResNameAdapter(resourcename.Item2);
    var newresname_param = ParamTools.GetParamValue(PARAM_NAME_NEW_RESOURCENAME);
    if (newresname_param.Item1 && !string.IsNullOrEmpty(newresname_param.Item2))
        newresname_arg = new ArgumentToResNameAdapter(newresname_param.Item2);

    using var sourcelib = new ResourceLibrary(sourcefilename.Item2, logger);
    using var targetlib = new ResourceLibrary(targetfilename, logger);

    using var source_resname = sourcename_arg != null ? new ResourceName(sourcename_arg) : null;
    using var target_resname = targetname_arg != null ? new ResourceName(targetname_arg) : null;
    using var newresname = newresname_arg != null ? new ResourceName(newresname_arg) : null;

    var copier = new GroupIconCopier(sourcelib, targetlib, logger);
    return copier.CopyGroupIcon(addalways, source_resname, target_resname, newresname);
}

CommandResult DeleteGoupIcon(string targetfilename)
{
    ArgumentToResNameAdapter? targetname_arg = null;

    var resourcename = ParamTools.GetParamValue(PARAM_NAME_TARGET_RESOURCENAME);
    if (resourcename.Item1)
        targetname_arg = new ArgumentToResNameAdapter(resourcename.Item2);

    var logger = new ConsoleLogger();
    using var targetlib = new ResourceLibrary(targetfilename, logger);
    using var target_resname = targetname_arg != null ? new ResourceName(targetname_arg) : null;

    var deleter = new GroupIconDeleter(targetlib, logger);
    return deleter.DeleteGroupIcon(target_resname);
}

CommandResult RenameGoupIcon(string targetfilename)
{
    var logger = new ConsoleLogger();

    var new_resourcename = ParamTools.GetParamValue(PARAM_NAME_NEW_RESOURCENAME);
    if (!new_resourcename.Item1)
    {
        logger.Error(nameof(IconResManExe), $"Parameter \"-{PARAM_NAME_NEW_RESOURCENAME}\" not found.");
        return CommandResult.ErrorParameterNotFound;
    }

    ArgumentToResNameAdapter? targetname_arg = null;
    ArgumentToResNameAdapter newname_arg = new ArgumentToResNameAdapter(new_resourcename.Item2);

    var resourcename = ParamTools.GetParamValue(PARAM_NAME_TARGET_RESOURCENAME);
    if (resourcename.Item1)
        targetname_arg = new ArgumentToResNameAdapter(resourcename.Item2);

    using var target_resname = targetname_arg != null ? new ResourceName(targetname_arg) : null;
    using var new_resname = new ResourceName(newname_arg);
    using var targetlib = new ResourceLibrary(targetfilename, logger);

    var renamer = new GroupIconRenamer(targetlib, logger);
    return renamer.RenameGroupIcon(target_resname, new_resname);
}

CommandResult ListGoupIcon(string targetfilename)
{
    var logger = new ConsoleLogger();
    using var targetlib = new ResourceLibrary(targetfilename, logger);

    var lister = new GroupIconLister(targetlib, logger);
    return lister.List(new ConsoleWriter());
}

bool FileExists(string path)
{
    if (File.Exists(path))
    {
        return true;
    }

    Console.Error.WriteLine($"File {path} does not exist.");
    return false;
}

void PrintHelp()
{
    Console.Out.WriteLine("Commands:");
    Console.Out.WriteLine($"{IconResManExe} update <resource file>");
    Console.Out.WriteLine($"{IconResManExe} delete <resource file>");
    Console.Out.WriteLine($"{IconResManExe} list <resource file>");
}
