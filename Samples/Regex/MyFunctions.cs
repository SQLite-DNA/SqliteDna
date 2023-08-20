using SqliteDna.Integration;

namespace Regex
{
    public static class MyFunctions
    {
        [SqliteFunction]
        public static bool Regexp(string pattern, string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        }

        [SqliteFunction]
        public static string RegexpReplace(string input, string pattern, string replacement)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement);
        }
    }
}
