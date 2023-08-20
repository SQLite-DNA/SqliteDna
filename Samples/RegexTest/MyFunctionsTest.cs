using SqliteDna.Testing;

namespace RegexTest
{
    public class MyFunctionsTest
    {
        [Theory, MemberData(nameof(ConnectionData))]
        public void Regexp(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                string rx = "^[A-Z0-9]\\d{2}-\\d{3}$";
                Assert.Equal(1, connection.ExecuteScalar<long>($"SELECT \"129-673\" REGEXP \"{rx}\""));
                Assert.Equal(1, connection.ExecuteScalar<long>($"SELECT \"A98-673\" REGEXP \"{rx}\""));
                Assert.Equal(0, connection.ExecuteScalar<long>($"SELECT \"a98-673\" REGEXP \"{rx}\""));
                Assert.Equal(0, connection.ExecuteScalar<long>($"SELECT \"A98-6734\" REGEXP \"{rx}\""));
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        public void RegexpReplace(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                string input = "This is   text with   far  too   much   white space.";
                string pattern = "\\s+";
                string replacement = " ";
                Assert.Equal("This is text with far too much white space.", connection.ExecuteScalar<string>($"SELECT RegexpReplace(\"{input}\", \"{pattern}\", \"{replacement}\")"));
            }
        }

        public static IEnumerable<object[]> ConnectionData =>
            SqliteConnection.GenerateConnectionParameters(new string[] { "Regex" });
    }

}
