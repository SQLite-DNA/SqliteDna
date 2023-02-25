using Microsoft.Data.Sqlite;

namespace TestIntergration
{
    public class AOT
    {
        [Fact]
        public void TestFunctions()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.LoadExtension("TestAOTTarget.dll");
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Woo2()";
                    Assert.Equal(2, (double)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Woo42()";
                    Assert.Equal(42, (double)command.ExecuteScalar()!);
                }
            }
        }
    }
}