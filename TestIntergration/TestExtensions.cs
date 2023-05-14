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
                    command.CommandText = @"SELECT MyInt64Sum(223372036854775807, 2)";
                    Assert.Equal(223372036854775809, (long)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT MyDoubleSum(1.2, 1.3)";
                    Assert.Equal(2.5, (double)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT MyConcat('Hello', 'world')";
                    Assert.Equal("Hello world", (string)command.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT Nop()";
                    Assert.Equal(-1, command.ExecuteNonQuery());
                }
                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"SELECT IncrementInternalCounter()";
                    Assert.Equal(-1, command1.ExecuteNonQuery());

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT GetInternalCounter()";
                    Assert.Equal(1, (long)command2.ExecuteScalar()!);
                }
                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"SELECT MyNullableConcat('Hello', 'world')";
                    Assert.Equal("Hello world", (string)command1.ExecuteScalar()!);

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT MyNullableConcat(NULL, NULL)";
                    Assert.True(command2.ExecuteScalar() is DBNull);

                    var command3 = connection.CreateCommand();
                    command3.CommandText = @"SELECT MyNullableConcat('Hello', NULL)";
                    Assert.Equal("Hello", (string)command3.ExecuteScalar()!);

                    var command4 = connection.CreateCommand();
                    command4.CommandText = @"SELECT MyNullableConcat(NULL, 'world')";
                    Assert.Equal("world", (string)command4.ExecuteScalar()!);
                }
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT MyError()";
                    Assert.Throws<SqliteException>(command.ExecuteScalar);
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
        public void ChinookDB(string extensionFile)
        {
            using (var connection = new SqliteConnection("Data Source=chinook.db"))
            {
                connection.Open();
                connection.LoadExtension(extensionFile);

                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"SELECT InvoiceDate FROM invoices WHERE InvoiceId = 27";
                    Assert.Equal("2009-04-22 00:00:00", (string)command1.ExecuteScalar()!);

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT DateTimeNop(InvoiceDate) FROM invoices WHERE InvoiceId = 27";
                    Assert.Equal("2009-04-22 00:00:00", (string)command2.ExecuteScalar()!);
                }
            }
        }

        [Theory]
        [InlineData("TestDNNENE.dll")]
        [InlineData("TestAOT.dll")]
        public void NorthwindDB(string extensionFile)
        {
            using (var connection = new SqliteConnection("Data Source=northwind.db"))
            {
                connection.Open();
                connection.LoadExtension(extensionFile);

                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"SELECT HireDate FROM Employees WHERE EmployeeId = 9";
                    Assert.Equal("2014-11-15", (string)command1.ExecuteScalar()!);

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT DateTimeNop(HireDate) FROM Employees WHERE EmployeeId = 9";
                    Assert.Equal("2014-11-15 00:00:00", (string)command2.ExecuteScalar()!);
                }

                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"SELECT Picture FROM Categories WHERE CategoryID = 1";
                    byte[] bytes1 = (byte[])command1.ExecuteScalar()!;
                    Assert.Equal(10151, bytes1.Length);
                    Assert.Equal(255, bytes1[0]);
                    Assert.Equal(216, bytes1[1]);
                    Assert.Equal(217, bytes1[10149]);
                    Assert.Equal(0, bytes1[10150]);

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT ShiftBlob(Picture) FROM Categories WHERE CategoryID = 1";
                    byte[] bytes2 = (byte[])command2.ExecuteScalar()!;
                    Assert.Equal(10151, bytes2.Length);
                    Assert.Equal(0, bytes2[0]);
                    Assert.Equal(217, bytes2[1]);
                    Assert.Equal(218, bytes2[10149]);
                    Assert.Equal(1, bytes2[10150]);
                }

                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"SELECT NullableBlob(Picture) FROM Categories WHERE CategoryID = 1";
                    byte[] bytes1 = (byte[])command1.ExecuteScalar()!;
                    Assert.Equal(10151, bytes1.Length);
                    Assert.Equal(255, bytes1[0]);

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT NullableBlob(NULL)";
                    Assert.True(command2.ExecuteScalar() is DBNull);
                }
            }
        }

        [Theory]
        [InlineData("TestDNNENE.dll")]
        [InlineData("TestAOT.dll")]
        public void Tables(string extensionFile)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.LoadExtension(extensionFile);
                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"CREATE VIRTUAL TABLE LongTable USING MyLongTable";
                    Assert.Equal(0, command1.ExecuteNonQuery());

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT COUNT(*) FROM LongTable";
                    Assert.Equal(3, (long)command2.ExecuteScalar()!);

                    var command3 = connection.CreateCommand();
                    command3.CommandText = @"SELECT * FROM LongTable";
                    using (var reader = command3.ExecuteReader())
                    {
                        Assert.True(reader.Read());
                        Assert.Equal(1, (long)reader["Value"]);
                        Assert.True(reader.Read());
                        Assert.Equal(5, (long)reader["Value"]);
                        Assert.True(reader.Read());
                        Assert.Equal(17, (long)reader["Value"]);
                        Assert.False(reader.Read());
                    }
                }
                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"CREATE VIRTUAL TABLE StringTable USING MyStringTable";
                    Assert.Equal(0, command1.ExecuteNonQuery());

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT * FROM StringTable";
                    using (var reader = command2.ExecuteReader())
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("str1", (string)reader["Value"]);
                        Assert.True(reader.Read());
                        Assert.Equal("str2", (string)reader["Value"]);
                    }
                }
                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"CREATE VIRTUAL TABLE CustomStringTable USING MyCustomStringTable";
                    Assert.Equal(0, command1.ExecuteNonQuery());

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT * FROM CustomStringTable";
                    using (var reader = command2.ExecuteReader())
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("MyCustomString", (string)reader["Value"]);
                    }
                }
                {
                    var command1 = connection.CreateCommand();
                    command1.CommandText = @"CREATE VIRTUAL TABLE RecordTable USING MyRecordTable";
                    Assert.Equal(0, command1.ExecuteNonQuery());

                    var command2 = connection.CreateCommand();
                    command2.CommandText = @"SELECT Name, Id FROM RecordTable";
                    using (var reader = command2.ExecuteReader())
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("n42", (string)reader["Name"]);
                        Assert.Equal(420, (long)reader["Id"]);
                        Assert.True(reader.Read());
                        Assert.Equal("n50", (string)reader["Name"]);
                        Assert.Equal(5, (long)reader["Id"]);
                    }
                }
            }
        }
    }
}