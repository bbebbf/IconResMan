using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconResMan
{
    public sealed class ConsoleLogger : ILogger
    {
        public void Error(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.Out.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
