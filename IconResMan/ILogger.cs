using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconResMan
{
    public interface ILogger
    {
        void Info(string originator, string message);
        void InfoVerbose(string originator, string message);
        void Warn(string originator, string message);
        void Error(string originator, string message);
        void Error(string originator, string message, int errorcode);
        void Error(string originator, int errorcode);
    }
}
