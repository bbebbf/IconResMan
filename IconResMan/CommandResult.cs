using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
