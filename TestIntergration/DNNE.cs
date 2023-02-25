using Microsoft.Data.Sqlite;

namespace TestIntergration
{
    public class DNNE
    {
        [Fact]
        public void TestFunctions()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.LoadExtension("TestDNNENE.dll");
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Too2()";
                    Assert.Equal(2, (double)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Too42()";
                    Assert.Equal(42, (double)command.ExecuteScalar()!);
                }
            }
        }
    }
}