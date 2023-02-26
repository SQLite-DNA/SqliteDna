using SqliteDna.Integration;

namespace Minimal
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
    }
}
