using SqliteDna.Integration;

namespace ExtensionAOT;

public static class MyFunctions
{
        [SqliteFunction]
        public static int Foo2()
        {
            return 2;
        }
}
