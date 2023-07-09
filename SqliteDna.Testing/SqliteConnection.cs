using System;
using System.Collections.Generic;

namespace SqliteDna.Testing
{
    public static class SqliteConnection
    {
        public static ISqliteConnection Create(string connectionString, string extensionFile, SqliteProvider provider)
        {
            return null;
        }

        public static IEnumerable<object[]> GenerateConnectionParameters(IEnumerable<string> extensionFiles)
        {
            List<object[]> result = new List<object[]>();
            foreach (string extensionFile in extensionFiles)
                foreach (SqliteProvider provider in Enum.GetValues(typeof(SqliteProvider)))
                    result.Add(new object[] { extensionFile, provider });

            return result;
        }
    }
}
