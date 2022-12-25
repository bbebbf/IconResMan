using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconResMan
{
    public class LogProcessor
    {
        public LogProcessor(ILogger logger, bool verbose)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.verbose = verbose;
        }

        public void Info(string originator, string message)
        {
            logger.Info($"[{originator}] {message}");
        }

        public void InfoVerbose(string originator, string message)
        {
            if (verbose)
                logger.Info($"[{originator}] {message}");
        }

        public void Warn(string originator, string message)
        {
            logger.Warn($"[{originator}] {message}");
        }

        public void Error(string originator, string message)
        {
            logger.Error($"[{originator}] {message}");
        }

        public void Error(string originator, string message, int errorcode)
        {
            logger.Error($"[{originator}] {message} [Error code: {errorcode}]");
        }

        public void Error(string originator, int errorcode)
        {
            logger.Error($"[{originator}] [Error code: {errorcode}]");
        }

        private ILogger logger;
        private bool verbose;
    }
}
