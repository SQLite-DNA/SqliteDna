using SqliteDna.Integration;

namespace TestShared
{
    public class MyFunctions
    {
        public record CustomRecord(string Name, int Id);
        public record RecordWithSQLiteKeyword(string Exists);

        public class BaseClassWithProperty
        {
            public string? BaseName { get; set; }
        }

        public class DerivedClassWithProperty : BaseClassWithProperty
        {
            public string? DerivedName { get; set; }
        }

        [SqliteFunction]
        public static int Foo2()
        {
            return 2;
        }

        [SqliteFunction]
        public static int Foo42()
        {
            return 42;
        }

        [SqliteFunction]
        public static string FooHello()
        {
            return "Hello";
        }

        [SqliteFunction]
        public static int MyIntSum(int a1, int a2)
        {
            return a1 + a2;
        }

        [SqliteFunction]
        public static long MyInt64Sum(long a1, long a2)
        {
            return a1 + a2;
        }

        [SqliteFunction]
        public static double MyDoubleSum(double a1, double a2)
        {
            return a1 + a2;
        }

        [SqliteFunction]
        public static string MyConcat(string s1, string s2)
        {
            return s1 + " " + s2;
        }

        [SqliteFunction]
        public static string? MyNullableConcat(string? s1, string? s2)
        {
            if (s1 == null && s2 == null)
                return null;

            if (s1 == null)
                return s2;

            if (s2 == null)
                return s1;

            return s1 + " " + s2;
        }

        [SqliteFunction]
        public static void Nop()
        {
        }

        [SqliteFunction]
        public static void ResetInternalCounter()
        {
            internalCounter = 0;
        }

        [SqliteFunction]
        public static void IncrementInternalCounter()
        {
            ++internalCounter;
        }

        [SqliteFunction]
        public static int GetInternalCounter()
        {
            return internalCounter;
        }

        [SqliteFunction]
        public static DateTime DateTimeNop(DateTime dt)
        {
            return dt;
        }

        [SqliteFunction]
        public static Guid GuidNop(Guid g)
        {
            return g;
        }

        [SqliteFunction]
        public static byte[] ShiftBlob(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
                bytes[i] = (byte)(bytes[i] + 1);

            return bytes;
        }

        [SqliteFunction]
        public static byte[]? NullableBlob(byte[]? bytes)
        {
            return bytes;
        }

        [SqliteFunction]
        public static void MyError()
        {
            throw new Exception("My error message");
        }

        [SqliteTableFunction]
        public static IEnumerable<long> MyLongTable()
        {
            List<long> result = new List<long> { 1, 5, 17 };
            return result;
        }

        [SqliteTableFunction]
        public static IEnumerable<string> MyStringTable()
        {
            List<string> result = new List<string> { "str1", "str2" };
            return result;
        }

        [SqliteTableFunction]
        public static IEnumerable<CustomString> MyCustomStringTable()
        {
            List<CustomString> result = new List<CustomString> { new CustomString() };
            return result;
        }

        [SqliteTableFunction]
        public static IEnumerable<CustomRecord> MyRecordTable()
        {
            List<CustomRecord> result = new List<CustomRecord> { new CustomRecord("n42", 420), new CustomRecord("n50", 5) };
            return result;
        }

        [SqliteTableFunction]
        public static IEnumerable<RecordWithSQLiteKeyword> MyTableWithSQLiteKeyword()
        {
            return new List<RecordWithSQLiteKeyword> { new RecordWithSQLiteKeyword("yes") };
        }

        [SqliteTableFunction]
        public static IEnumerable<DerivedClassWithProperty> MyTableWithInheritedProperty()
        {
            DerivedClassWithProperty o = new DerivedClassWithProperty();
            o.BaseName = "base";
            o.DerivedName = "derived";
            return new List<DerivedClassWithProperty> { o };
        }

        [SqliteTableFunction]
        public static IEnumerable<CustomRecord> MyRecordParamsTable(string name, int id)
        {
            List<CustomRecord> result = new List<CustomRecord> { new CustomRecord(name, id) };
            return result;
        }

        [SqliteTableFunction]
        public static DynamicTable MyDynamicTable()
        {
            List<object[]> data = new List<object[]> {
                new object[] { 11L, "Diane", "London" },
                new object[] { 22L, "Grace", "Berlin" },
                new object[] { 33, "Alice", "Paris" },
            };

            return new("id integer, name text, city text", data);
        }

        [SqliteTableFunction]
        public static DynamicTable MyDynamicParamsTable(string schema, string record)
        {
            var types = schema.Split(',').Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last()).ToArray();

            var values = record.Split(',').ToArray();

            var valuesArray = new object[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                switch (types[i])
                {
                    case "integer":
                        valuesArray[i] = long.Parse(values[i]);
                        break;
                    case "real":
                        valuesArray[i] = double.Parse(values[i]);
                        break;
                    case "text":
                        valuesArray[i] = values[i];
                        break;
                }
            }

            return new(schema, new List<object[]> { valuesArray });
        }

        public static int Noo1()
        {
            return 1;
        }

        public class CustomString
        {
            public override string ToString()
            {
                return "MyCustomString";
            }
        }

        private static int internalCounter;
    }
}
