﻿using SqliteDna.Testing;
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
                Assert.ThrowsAny<Exception>(() => connection.ExecuteNonQuery("SELECT MyError()"));
                Assert.ThrowsAny<Exception>(() => connection.ExecuteNonQuery("SELECT Noo1()"));
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        public void ChinookDB(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=chinook.db", extensionFile, provider))
            {
                string commandText = "SELECT InvoiceDate FROM invoices WHERE InvoiceId = 27";
                string result = provider switch
                {
                    SqliteProvider.Microsoft => connection.ExecuteScalar<string>(commandText),
                    SqliteProvider.System => connection.ExecuteScalar<DateTime>(commandText).ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture),
                    _ => throw new NotImplementedException(),
                };
                Assert.Equal("2009-04-22 00:00:00", result);
                Assert.Equal("2009-04-22 00:00:00", connection.ExecuteScalar<string>("SELECT DateTimeNop(InvoiceDate) FROM invoices WHERE InvoiceId = 27"));
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        public void NorthwindDB(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=northwind.db", extensionFile, provider))
            {
                {
                    string commandText = "SELECT HireDate FROM Employees WHERE EmployeeId = 9";
                    string result = provider switch
                    {
                        SqliteProvider.Microsoft => connection.ExecuteScalar<string>(commandText),
                        SqliteProvider.System => connection.ExecuteScalar<DateTime>(commandText).ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture),
                        _ => throw new NotImplementedException(),
                    };
                    string expected = provider switch
                    {
                        SqliteProvider.Microsoft => "2014-11-15",
                        SqliteProvider.System => "2014-11-15 00:00:00",
                        _ => throw new NotImplementedException(),
                    };
                    Assert.Equal(expected, result);
                    Assert.Equal("2014-11-15 00:00:00", connection.ExecuteScalar<string>("SELECT DateTimeNop(HireDate) FROM Employees WHERE EmployeeId = 9"));
                }
                {
                    byte[] bytes1 = connection.ExecuteScalar<byte[]>("SELECT Picture FROM Categories WHERE CategoryID = 1");
                    Assert.Equal(10151, bytes1.Length);
                    Assert.Equal(255, bytes1[0]);
                    Assert.Equal(216, bytes1[1]);
                    Assert.Equal(217, bytes1[10149]);
                    Assert.Equal(0, bytes1[10150]);

                    byte[] bytes2 = connection.ExecuteScalar<byte[]>("SELECT ShiftBlob(Picture) FROM Categories WHERE CategoryID = 1");
                    Assert.Equal(10151, bytes2.Length);
                    Assert.Equal(0, bytes2[0]);
                    Assert.Equal(217, bytes2[1]);
                    Assert.Equal(218, bytes2[10149]);
                    Assert.Equal(1, bytes2[10150]);
                }
                {
                    byte[] bytes1 = connection.ExecuteScalar<byte[]>("SELECT NullableBlob(Picture) FROM Categories WHERE CategoryID = 1");
                    Assert.Equal(10151, bytes1.Length);
                    Assert.Equal(255, bytes1[0]);

                    Assert.Equal(DBNull.Value, connection.ExecuteScalar<DBNull>("SELECT NullableBlob(NULL)"));
                }
            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "TestDNNENE", "TestAOT" });
    }
}