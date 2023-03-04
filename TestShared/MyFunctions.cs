﻿using SqliteDna.Integration;

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

        public static int Noo1()
        {
            return 1;
        }
    }
}
