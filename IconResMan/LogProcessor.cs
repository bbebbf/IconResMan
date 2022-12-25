namespace IconResMan
{
    public class LogProcessor
    {
        public ILogger? Logger { get; set; }

        public bool Verbose { get; set; }

        public void Info(string originator, string message)
        {
            Logger?.Info($"[{originator}] {message}");
        }

        public void InfoVerbose(string originator, string message)
        {
            if (Verbose)
                Logger?.Info($"[{originator}] {message}");
        }

        public void Warn(string originator, string message)
        {
            Logger?.Warn($"[{originator}] {message}");
        }

        public void Error(string originator, string message)
        {
            Logger?.Error($"[{originator}] {message}");
        }

        public void Error(string originator, string message, int errorcode)
        {
            Logger?.Error($"[{originator}] {message} [Error code: {errorcode}]");
        }

        public void Error(string originator, int errorcode)
        {
            Logger?.Error($"[{originator}] [Error code: {errorcode}]");
        }
    }
}
