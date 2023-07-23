using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

                case SqliteProvider.SQLiteCpp:
                    return new SqliteCppConnection(connectionString, extensionPath);
            }

            throw new ArgumentException("provider");
        }

        public static IEnumerable<object[]> GenerateConnectionParameters(IEnumerable<string> extensionFiles)
        {
            return GenerateConnectionParameters(extensionFiles, Enum.GetValues(typeof(SqliteProvider)).Cast<SqliteProvider>());
        }

        public static IEnumerable<object[]> GenerateConnectionParameters(IEnumerable<string> extensionFiles, SqliteProvider provider)
        {
            return GenerateConnectionParameters(extensionFiles, new SqliteProvider[] { provider });
        }

        private static IEnumerable<object[]> GenerateConnectionParameters(IEnumerable<string> extensionFiles, IEnumerable<SqliteProvider> providers)
        {
            List<object[]> result = new List<object[]>();
            foreach (string extensionFile in extensionFiles)
                foreach (SqliteProvider provider in providers)
                {
                    if (provider == SqliteProvider.SQLiteCpp && Environment.OSVersion.Platform != PlatformID.Win32NT)
                        continue;

                    result.Add(new object[] { extensionFile, provider });
                }

            return result;
        }
    }
}
