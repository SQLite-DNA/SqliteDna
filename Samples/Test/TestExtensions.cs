using SqliteDna.Testing;

namespace Test
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
            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "MinimalNE", "MinimalAOT" });
    }
}