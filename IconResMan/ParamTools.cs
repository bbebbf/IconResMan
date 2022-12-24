using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IconResMan
{
    public static class ParamTools
    {
        public static bool FindParamSwitch(string paramName)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                if (ParamIsEquals(paramName, args[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static Tuple<bool, string> GetParamValue(string paramName)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length - 1; i++)
            {
                if (ParamIsEquals(paramName, args[i]))
                {
                    return new Tuple<bool, string>(true, args[i + 1]);
                }
            }
            return new Tuple<bool, string>(false, "");
        }

        private static bool ParamIsEquals(string expected, string paramName)
        {
            return (string.Compare(paramName, $"/{expected}") == 0 || 
                    string.Compare(paramName, $"-{expected}") == 0);
        }
    }
}
