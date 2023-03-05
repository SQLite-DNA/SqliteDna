using SqliteDna.Integration;

namespace TestShared
{
    public class MyFunctions
    {
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

        public static int Noo1()
        {
            return 1;
        }
    }
}
