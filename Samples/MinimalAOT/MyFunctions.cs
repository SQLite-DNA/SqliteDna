using SqliteDna.Integration;

namespace MinimalAOT
{
    public class MyFunctions
    {
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
    }
}
