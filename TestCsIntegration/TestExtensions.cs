using SqliteDna.Testing;
using System.Globalization;

namespace TestCsIntegration
{
    public class TestExtensions
    {
        [Theory, MemberData(nameof(ConnectionData))]
        public void Functions(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                Assert.Equal(2, connection.ExecuteCommand<long>("SELECT Foo2()"));
                Assert.Equal(42, connection.ExecuteCommand<long>("SELECT Foo42()"));
                Assert.Equal("Hello", connection.ExecuteCommand<string>("SELECT FooHello()"));
                Assert.Equal(5, connection.ExecuteCommand<long>("SELECT MyIntSum(2, 3)")); // System.Data.SQLite and Microsoft.Data.Sqlite support only Int64.
                Assert.Equal(223372036854775809, connection.ExecuteCommand<long>("SELECT MyInt64Sum(223372036854775807, 2)"));
                Assert.Equal(2.5, connection.ExecuteCommand<double>("SELECT MyDoubleSum(1.2, 1.3)"));
                Assert.Equal("Hello world", connection.ExecuteCommand<string>("SELECT MyConcat('Hello', 'world')"));
            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "TestDNNENE", "TestAOT" });
    }
}