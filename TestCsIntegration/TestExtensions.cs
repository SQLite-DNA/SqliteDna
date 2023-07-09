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
                Assert.Equal(2, connection.ExecuteScalar<long>("SELECT Foo2()"));
                Assert.Equal(42, connection.ExecuteScalar<long>("SELECT Foo42()"));
                Assert.Equal("Hello", connection.ExecuteScalar<string>("SELECT FooHello()"));
                Assert.Equal(5, connection.ExecuteScalar<long>("SELECT MyIntSum(2, 3)")); // System.Data.SQLite and Microsoft.Data.Sqlite support only Int64.
                Assert.Equal(223372036854775809, connection.ExecuteScalar<long>("SELECT MyInt64Sum(223372036854775807, 2)"));
                Assert.Equal(2.5, connection.ExecuteScalar<double>("SELECT MyDoubleSum(1.2, 1.3)"));
                Assert.Equal("Hello world", connection.ExecuteScalar<string>("SELECT MyConcat('Hello', 'world')"));
                Assert.Equal(-1, connection.ExecuteNonQuery("SELECT Nop()"));
                {
                    Assert.Equal(-1, connection.ExecuteNonQuery("SELECT ResetInternalCounter()"));
                    Assert.Equal(-1, connection.ExecuteNonQuery("SELECT IncrementInternalCounter()"));
                    Assert.Equal(1, connection.ExecuteScalar<long>("SELECT GetInternalCounter()"));
                }
                Assert.Equal("Hello world", connection.ExecuteScalar<string>("SELECT MyNullableConcat('Hello', 'world')"));
                Assert.Equal(DBNull.Value, connection.ExecuteScalar<DBNull>("SELECT MyNullableConcat(NULL, NULL)"));
                Assert.Equal("Hello", connection.ExecuteScalar<string>("SELECT MyNullableConcat('Hello', NULL)"));
                Assert.Equal("world", connection.ExecuteScalar<string>("SELECT MyNullableConcat(NULL, 'world')"));
            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "TestDNNENE", "TestAOT" });
    }
}