using SqliteDna.Testing;
using System.Globalization;

namespace TestCsIntegration
{
    public class TestExtensions
    {
        [Theory]
        [MemberData(nameof(ConnectionData))]
        public void Functions(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {

            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "TestDNNENE", "TestAOT" });
    }
}