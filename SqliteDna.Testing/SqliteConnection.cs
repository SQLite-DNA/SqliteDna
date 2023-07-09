using System;
using System.Collections.Generic;
using System.IO;

namespace SqliteDna.Testing
{
    public static class SqliteConnection
    {
        public static ISqliteConnection Create(string connectionString, string extensionFile, SqliteProvider provider)
        {
            string extensionPath = Path.Combine(".", extensionFile);
            switch (provider)
            {
                case SqliteProvider.Microsoft:
                    return new MicrosoftSqliteConnection(connectionString, extensionPath);

                case SqliteProvider.System:
                    return new SystemSqliteConnection(connectionString, extensionPath);
            }

            throw new ArgumentException("provider");
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
