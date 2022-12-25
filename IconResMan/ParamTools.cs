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

        public static string? GetParamValue(string paramName)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length - 1; i++)
            {
                if (ParamIsEquals(paramName, args[i]))
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        private static bool ParamIsEquals(string expected, string paramName)
        {
            return (string.Compare(paramName, $"/{expected}") == 0 || 
                    string.Compare(paramName, $"-{expected}") == 0);
        }
    }
}
