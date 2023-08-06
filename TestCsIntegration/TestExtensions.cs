using SqliteDna.Testing;
using System.Globalization;

namespace TestCsIntegration
{
    public class TestExtensions
    {
        [Theory, MemberData(nameof(ConnectionData))]
        public void Functions(string extensionFile, SqliteProvider provider)
        {
            int nonQuerySuccess = provider == SqliteProvider.SQLiteCpp ? 0 : -1;
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                Assert.Equal(2, connection.ExecuteScalar<long>("SELECT Foo2()"));
                Assert.Equal(42, connection.ExecuteScalar<long>("SELECT Foo42()"));
                Assert.Equal("Hello", connection.ExecuteScalar<string>("SELECT FooHello()"));
                Assert.Equal(5, connection.ExecuteScalar<long>("SELECT MyIntSum(2, 3)")); // System.Data.SQLite and Microsoft.Data.Sqlite support only Int64.
                Assert.Equal(223372036854775809, connection.ExecuteScalar<long>("SELECT MyInt64Sum(223372036854775807, 2)"));
                Assert.Equal(2.5, connection.ExecuteScalar<double>("SELECT MyDoubleSum(1.2, 1.3)"));
                Assert.Equal("Hello world", connection.ExecuteScalar<string>("SELECT MyConcat('Hello', 'world')"));
                Assert.Equal(nonQuerySuccess, connection.ExecuteNonQuery("SELECT Nop()"));
                {
                    Assert.Equal(nonQuerySuccess, connection.ExecuteNonQuery("SELECT ResetInternalCounter()"));
                    Assert.Equal(nonQuerySuccess, connection.ExecuteNonQuery("SELECT IncrementInternalCounter()"));
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
                    SqliteProvider.System => connection.ExecuteScalar<DateTime>(commandText).ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture),
                    _ => connection.ExecuteScalar<string>(commandText),
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
                        SqliteProvider.System => connection.ExecuteScalar<DateTime>(commandText).ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture),
                        _ => connection.ExecuteScalar<string>(commandText),
                    };
                    string expected = provider switch
                    {
                        SqliteProvider.System => "2014-11-15 00:00:00",
                        _ => "2014-11-15",
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

        [Theory, MemberData(nameof(SystemConnectionData))]
        public void JulianDB(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=Julian.db;DateTimeFormat=JulianDay", extensionFile, provider))
            {
                Assert.Equal("2023-06-11 15:00:02.874", connection.ExecuteScalar<DateTime>("SELECT InvoiceDate FROM invoices").ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture));
                Assert.Equal("2023-06-11 15:00:02.874", connection.ExecuteScalar<string>("SELECT DateTimeNop(InvoiceDate) FROM invoices"));
            }
        }


        [Theory, MemberData(nameof(SystemConnectionData))]
        public void UnixEpochDB(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=UnixEpoch.db;DateTimeFormat=UnixEpoch", extensionFile, provider))
            {
                Assert.Equal("2023-06-17 14:15:00", connection.ExecuteScalar<DateTime>("SELECT InvoiceDate FROM invoices").ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture));
                Assert.Equal("2023-06-17 14:15:00", connection.ExecuteScalar<string>("SELECT DateTimeNop(InvoiceDate) FROM invoices"));
            }
        }

        [Theory, MemberData(nameof(SystemConnectionData))]
        public void GuidTextDB(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=GuidText.db;BinaryGuid=false", extensionFile, provider))
            {
                Assert.Equal("a2f6cc56-92b8-4c49-a08b-b971bdf2dfb5", connection.ExecuteScalar<Guid>("SELECT guid FROM guids").ToString());
                Assert.Equal("a2f6cc56-92b8-4c49-a08b-b971bdf2dfb5", connection.ExecuteScalar<string>("SELECT GuidNop(guid) FROM guids"));
            }
        }

        [Theory, MemberData(nameof(SystemConnectionData))]
        public void GuidBinaryDB(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=GuidBinary.db;BinaryGuid=true", extensionFile, provider))
            {
                Assert.Equal("172173d2-dbf3-40bb-8cd8-d82b778dc96f", connection.ExecuteScalar<Guid>("SELECT guid FROM guids").ToString());
                Assert.Equal("172173d2-dbf3-40bb-8cd8-d82b778dc96f", connection.ExecuteScalar<string>("SELECT GuidNop(guid) FROM guids"));
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        public void Tables(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE LongTable USING MyLongTable"));
                    Assert.Equal(3, connection.ExecuteScalar<long>("SELECT COUNT(*) FROM LongTable"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM LongTable"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal(1, reader.GetItem<long>("Value"));
                        Assert.True(reader.Read());
                        Assert.Equal(5, reader.GetItem<long>("Value"));
                        Assert.True(reader.Read());
                        Assert.Equal(17, reader.GetItem<long>("Value"));
                        Assert.False(reader.Read());
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE StringTable USING MyStringTable"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM StringTable"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("str1", reader.GetItem<string>("Value"));
                        Assert.True(reader.Read());
                        Assert.Equal("str2", reader.GetItem<string>("Value"));
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE CustomStringTable USING MyCustomStringTable"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM CustomStringTable"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("MyCustomString", reader.GetItem<string>("Value"));
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE RecordTable USING MyRecordTable"));
                    using (var reader = connection.ExecuteReader("SELECT Name, Id FROM RecordTable"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("n42", reader.GetItem<string>("Name"));
                        Assert.Equal(420, reader.GetItem<long>("Id"));
                        Assert.True(reader.Read());
                        Assert.Equal("n50", reader.GetItem<string>("Name"));
                        Assert.Equal(5, reader.GetItem<long>("Id"));
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE RecordParamsTable USING MyRecordParamsTable(name1, 100)"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM RecordParamsTable"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("name1", reader.GetItem<string>("Name"));
                        Assert.Equal(100, reader.GetItem<long>("Id"));
                        Assert.False(reader.Read());
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery(@"CREATE VIRTUAL TABLE RecordParamsTable2 USING MyRecordParamsTable(""Hello, world!"", 100)"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM RecordParamsTable2"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("Hello, world!", reader.GetItem<string>("Name"));
                        Assert.Equal(100, reader.GetItem<long>("Id"));
                        Assert.False(reader.Read());
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE RecordParamsTable3 USING MyRecordParamsTable('Hello, world 3!', 100)"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM RecordParamsTable3"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal("Hello, world 3!", reader.GetItem<string>("Name"));
                        Assert.Equal(100, reader.GetItem<long>("Id"));
                        Assert.False(reader.Read());
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE DynamicTable USING MyDynamicTable()"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM DynamicTable"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal(11, reader.GetItem<long>("id"));
                        Assert.Equal("Diane", reader.GetItem<string>("name"));
                        Assert.Equal("London", reader.GetItem<string>("city"));
                        Assert.True(reader.Read());
                        Assert.Equal(22, reader.GetItem<long>("id"));
                        Assert.Equal("Grace", reader.GetItem<string>("name"));
                        Assert.Equal("Berlin", reader.GetItem<string>("city"));
                        Assert.True(reader.Read());
                        Assert.Equal(33, reader.GetItem<long>("id"));
                        Assert.Equal("Alice", reader.GetItem<string>("name"));
                        Assert.Equal("Paris", reader.GetItem<string>("city"));
                        Assert.False(reader.Read());
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE DynamicParamsTable1 USING MyDynamicParamsTable(\"id integer, name text, city text\", \"11,Diane,London\")"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM DynamicParamsTable1"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal(11, reader.GetItem<long>("id"));
                        Assert.Equal("Diane", reader.GetItem<string>("name"));
                        Assert.Equal("London", reader.GetItem<string>("city"));
                        Assert.False(reader.Read());
                    }
                }
                {
                    Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE DynamicParamsTable2 USING MyDynamicParamsTable(\"value real, id integer, name text\", \"1.21,3,Grace\")"));
                    using (var reader = connection.ExecuteReader("SELECT * FROM DynamicParamsTable2"))
                    {
                        Assert.True(reader.Read());
                        Assert.Equal(1.21, reader.GetItem<double>("value"));
                        Assert.Equal(3, reader.GetItem<long>("id"));
                        Assert.Equal("Grace", reader.GetItem<string>("name"));
                        Assert.False(reader.Read());
                    }
                }
            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "TestDNNENE", "TestAOT" });

        public static IEnumerable<object[]> SystemConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "TestDNNENE", "TestAOT" }, SqliteProvider.System);
    }
}