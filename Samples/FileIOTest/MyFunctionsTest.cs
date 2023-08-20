using SqliteDna.Testing;

namespace FileIOTest
{
    public class MyFunctionsTest
    {
        [Theory, MemberData(nameof(ConnectionData))]
        public void ReadAllLines(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE FileLines USING ReadAllLines(test.txt)"));
                using (var reader = connection.ExecuteReader("SELECT * FROM FileLines"))
                {
                    Assert.True(reader.Read());
                    Assert.Equal("one", reader.GetItem<string>("Value"));
                    Assert.True(reader.Read());
                    Assert.Equal("two", reader.GetItem<string>("Value"));
                    Assert.True(reader.Read());
                    Assert.Equal("three", reader.GetItem<string>("Value"));
                }
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        public void ListFiles(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE Files USING ListFiles(., *.txt)"));
                using (var reader = connection.ExecuteReader("SELECT * FROM Files"))
                {
                    Assert.True(reader.Read());
                    Assert.Equal("test.txt", reader.GetItem<string>("Name"));
                    Assert.Equal(15, reader.GetItem<long>("Length"));
                    Assert.Equal(".txt", reader.GetItem<string>("Extension"));
                }
            }
        }

        public static IEnumerable<object[]> ConnectionData =>
            SqliteConnection.GenerateConnectionParameters(new string[] { "FileIO" });
    }

}
