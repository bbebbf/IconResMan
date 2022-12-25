namespace IconResMan
{
    public enum CommandResult
    {
        Success = 0,
        Error,
        ErrorUnknownCommand,
        ErrorResourceNotFound,
        ErrorResourceAmbiguous,
        ErrorFileNotFound,
        ErrorParameterNotFound
    }
}
