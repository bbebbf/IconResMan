// See https://aka.ms/new-console-template for more information
using IconResMan;

var IconResManExe = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

const int MINIMUM_ARGS_LENGHTH = 2;
const int PARAM_IDX_ACTION = 0;
const int PARAM_IDX_TARGETFILENAME = 1;

const string PARAM_NAME_VERBOSE = "verbose";
const string PARAM_NAME_SOURCE = "sfile";
const string PARAM_NAME_SOURCE_RESOURCENAME = "sres";
const string PARAM_NAME_TARGET_RESOURCENAME = "tres";
const string PARAM_NAME_NEW_RESOURCENAME = "newname";

IIconResManager iconresmanager = new IconResManager();
iconresmanager.SetLogger(new ConsoleLogger());
iconresmanager.SetVerbosity(ParamTools.FindParamSwitch(PARAM_NAME_VERBOSE));

try
{
    if (args.Length < MINIMUM_ARGS_LENGHTH)
    {
        PrintHelp();
        return 1;
    }

    var command = args[PARAM_IDX_ACTION];
    var targetfilename = args[PARAM_IDX_TARGETFILENAME];

    switch(command)
    {
        case "add":
            return UpdateGoupIcon(targetfilename, true);
        case "update":
            return UpdateGoupIcon(targetfilename, false);
        case "delete":
            return DeleteGoupIcon(targetfilename);
        case "rename":
            return RenameGoupIcon(targetfilename);
        case "list":
            return ListGoupIcon(targetfilename);
    }
    PrintHelp();
    return (int)CommandResult.ErrorUnknownCommand;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Exception occurred: [{ex.GetType()}] {ex.Message}");
    return (int)CommandResult.Error;
}

int UpdateGoupIcon(string targetfilename, bool addAlways)
{
    var sourcepath = ParamTools.GetParamValue(PARAM_NAME_SOURCE);
    if (sourcepath == null)
    {
        Console.Error.WriteLine(nameof(IconResManExe), $"Parameter \"-{PARAM_NAME_NEW_RESOURCENAME}\" not found.");
        return (int)CommandResult.ErrorParameterNotFound;
    }

    if (addAlways)
    {
        return iconresmanager.AddGroupIcon(targetfilename,
            sourcepath,
            ParamTools.GetParamValue(PARAM_NAME_SOURCE_RESOURCENAME),
            ParamTools.GetParamValue(PARAM_NAME_NEW_RESOURCENAME));
    }
    else
    {
        return iconresmanager.UpdateGroupIcon(targetfilename,
            sourcepath,
            ParamTools.GetParamValue(PARAM_NAME_SOURCE_RESOURCENAME),
            ParamTools.GetParamValue(PARAM_NAME_TARGET_RESOURCENAME),
            ParamTools.GetParamValue(PARAM_NAME_NEW_RESOURCENAME));
    }
}

int DeleteGoupIcon(string targetfilename)
{
    return iconresmanager.DeleteGroupIcon(targetfilename,
        ParamTools.GetParamValue(PARAM_NAME_TARGET_RESOURCENAME));
}

int RenameGoupIcon(string targetfilename)
{
    var newname = ParamTools.GetParamValue(PARAM_NAME_NEW_RESOURCENAME);
    if (newname == null)
    {
        Console.Error.WriteLine(nameof(IconResManExe), $"Parameter \"-{PARAM_NAME_NEW_RESOURCENAME}\" not found.");
        return (int)CommandResult.ErrorParameterNotFound;
    }

    return iconresmanager.RenameGroupIcon(targetfilename,
        ParamTools.GetParamValue(PARAM_NAME_TARGET_RESOURCENAME),
        newname);
}

int ListGoupIcon(string targetfilename)
{
    return iconresmanager.ListGroupIcons(targetfilename, new ConsoleWriter());
}

void PrintHelp()
{
    Console.Out.WriteLine("Commands:");
    Console.Out.WriteLine($"{IconResManExe} update <resource file>");
    Console.Out.WriteLine($"{IconResManExe} delete <resource file>");
    Console.Out.WriteLine($"{IconResManExe} list <resource file>");
}
