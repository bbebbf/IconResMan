using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconResMan
{
    public sealed class ConsoleLogger : ILogger
    {
        public void Error(string originator, string message, int errorcode)
        {
            Console.Error.Write($"[ERROR] [{originator}] {message} [Error code: {errorcode}]\n");
        }

        public void Error(string originator, string message)
        {
            Console.Error.Write($"[ERROR] [{originator}] {message}\n");
        }

        public void Error(string originator, int errorcode)
        {
            Console.Error.Write($"[ERROR] [{originator}] [Error code: {errorcode}]\n");
        }

        public void Info(string originator, string message)
        {
            Console.Out.Write($"[INFO] [{originator}] {message}\n");
        }

        public void InfoVerbose(string originator, string message)
        {
            if (ParamTools.FindParamSwitch("verbose"))
                Console.Out.Write($"[INFOVERBOSE] [{originator}] {message}\n");
        }

        public void Warn(string originator, string message)
        {
            Console.Out.Write($"[WARNING] [{originator}] {message}\n");
        }
    }
}
