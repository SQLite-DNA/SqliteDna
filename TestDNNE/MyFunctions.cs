using SqliteDna.Integration;

namespace TestDNNETarget
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

        public static int Noo1()
        {
            return 1;
        }
    }
}
