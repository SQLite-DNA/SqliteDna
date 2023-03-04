using Microsoft.Data.Sqlite;

namespace TestIntergration
{
    public class TestExtensions
    {
        [Theory]
        [InlineData("TestDNNENE.dll")]
        [InlineData("TestAOT.dll")]
        public void Functions(string extensionFile)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.LoadExtension(extensionFile);
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Foo2()";
                    Assert.Equal(2, (long)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Foo42()";
                    Assert.Equal(42, (long)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT FooHello()";
                    Assert.Equal("Hello", (string)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT MyIntSum(2, 3)";
                    Assert.Equal(5, (long)command.ExecuteScalar()!); // Microsoft.Data.Sqlite supports only Int64
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT MyDoubleSum(1.2, 1.3)";
                    Assert.Equal(2.5, (double)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Noo1()";
                    Assert.Throws<SqliteException>(command.ExecuteScalar);
                }
            }
        }

        [Theory]
        [InlineData("TestDNNENE.dll")]
        [InlineData("TestAOT.dll")]
        public void Int64Sum(string extensionFile)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.LoadExtension(extensionFile);

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT MyInt64Sum(223372036854775807, 2)";
                Assert.Equal(223372036854775809, (long)command.ExecuteScalar()!);
            }
        }
    }
}