using Microsoft.Data.Sqlite;

namespace TestIntergration
{
    public class TestExtensions
    {
        [Theory]
        [InlineData("TestDNNENE.dll")]
        [InlineData("TestAOT.dll")]
        public void TestFunctions(string extensionFile)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.LoadExtension(extensionFile);
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Foo2()";
                    Assert.Equal(2, (double)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Foo42()";
                    Assert.Equal(42, (double)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT FooHello()";
                    Assert.Equal("Hello", (string)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Noo1()";
                    Assert.Throws<SqliteException>(command.ExecuteScalar);
                }
            }
        }
    }
}