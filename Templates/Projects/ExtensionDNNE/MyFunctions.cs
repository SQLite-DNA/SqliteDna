using SqliteDna.Integration;

namespace ExtensionDNNE;

public static class MyFunctions
{
        [SqliteFunction]
        public static int Foo42()
        {
            return 42;
        }
}
