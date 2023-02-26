using SqliteDna.Integration;

namespace MinimalAOT
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
