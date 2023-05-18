using SqliteDna.Integration;

namespace TestShared
{
    public class MyFunctions
    {
        public record CustomRecord(string Name, int Id);

        [Function]
        public static int Foo2()
        {
            return 2;
        }

        [Function]
        public static int Foo42()
        {
            return 42;
        }

        [Function]
        public static string FooHello()
        {
            return "Hello";
        }

        [Function]
        public static int MyIntSum(int a1, int a2)
        {
            return a1 + a2;
        }

        [Function]
        public static long MyInt64Sum(long a1, long a2)
        {
            return a1 + a2;
        }

        [Function]
        public static double MyDoubleSum(double a1, double a2)
        {
            return a1 + a2;
        }

        [Function]
        public static string MyConcat(string s1, string s2)
        {
            return s1 + " " + s2;
        }

        [Function]
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

        [Function]
        public static void Nop()
        {
        }

        [Function]
        public static void IncrementInternalCounter()
        {
            ++internalCounter;
        }

        [Function]
        public static int GetInternalCounter()
        {
            return internalCounter;
        }

        [Function]
        public static DateTime DateTimeNop(DateTime dt)
        {
            return dt;
        }

        [Function]
        public static byte[] ShiftBlob(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
                bytes[i] = (byte)(bytes[i] + 1);

            return bytes;
        }

        [Function]
        public static byte[]? NullableBlob(byte[]? bytes)
        {
            return bytes;
        }

        [Function]
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
