using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SqliteDna.Integration
{
    internal class FunctionModule
    {
        public FunctionModule()
        {
            CreateModule();
        }

        private unsafe void CreateModule()
        {
            module = new();
            module.xCreate = &xCreate;
            module.xConnect = &XConnect;
            module.xBestIndex = &xBestIndex;
            module.xDisconnect = &xDisconnect;
            module.xDestroy = &xDestroy;
            module.xOpen = &xOpen;
            module.xClose = &xClose;
            module.xFilter = &xFilter;
            module.xNext = &xNext;
            module.xEof = &xEof;
            module.xColumn = &xColumn;
            module.xRowid = &xRowid;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xCreate(IntPtr db, IntPtr aux, int argc, IntPtr argv, IntPtr* vtab, IntPtr err)
        {
            BaseFunctionModuleParams moduleParams = (BaseFunctionModuleParams)GCHandle.FromIntPtr(aux).Target!;

            string[]? arguments = null;
            if (argc > defaultCreationArgc)
            {
                arguments = new string[argc - defaultCreationArgc];
                char** pargv = (char**)argv;
                for (int i = 0; i < arguments.Length; ++i)
                {
                    arguments[i] = RemoveEnclosingQuotes(Marshal.PtrToStringAnsi((IntPtr)(*(pargv + i + defaultCreationArgc)))!);
                }
            }
            moduleParams.OnCreate(arguments);
            string schema = moduleParams.GetSchema();

            Sqlite.GetAPI().declare_vtab(db, Sqlite.StringToSqliteUtf8($"CREATE TABLE x({schema})", out _));
            (*vtab) = Sqlite.GetAPI().malloc(sizeof(VTab));
            (*(VTab*)(*vtab)).moduleParams = aux;
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int XConnect(IntPtr db, IntPtr aux, int argc, IntPtr argv, IntPtr* vtab, IntPtr err)
        {
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xDisconnect(IntPtr vtab)
        {
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xDestroy(IntPtr vtab)
        {
            Sqlite.GetAPI().free(vtab);
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xBestIndex(IntPtr vtab, IntPtr info)
        {
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xOpen(IntPtr vtab, IntPtr* pcursor)
        {
            BaseFunctionModuleParams moduleParams = (BaseFunctionModuleParams)GCHandle.FromIntPtr((*(VTab*)vtab).moduleParams).Target!;
            var enumerator = moduleParams.GetData().GetEnumerator();

            (*pcursor) = Sqlite.GetAPI().malloc(sizeof(Cursor));
            Cursor* cursor = (Cursor*)(*pcursor);
            (*cursor).enumerator = GCHandle.ToIntPtr(GCHandle.Alloc(enumerator));
            (*cursor).eof = !enumerator.MoveNext();

            PropertyInfo[]? properties = moduleParams.GetProperties();
            (*cursor).properties = properties != null ? GCHandle.ToIntPtr(GCHandle.Alloc(properties)) : IntPtr.Zero;

            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xClose(IntPtr cursor)
        {
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xFilter(IntPtr cursor, int idxNum, IntPtr idxStr, int argc, IntPtr argv)
        {
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xNext(IntPtr cursor)
        {
            IEnumerator enumerator = (IEnumerator)GCHandle.FromIntPtr((*(Cursor*)cursor).enumerator).Target!;
            (*(Cursor*)cursor).eof = !enumerator.MoveNext();
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xEof(IntPtr cursor)
        {
            Cursor* my_cursor = (Cursor*)cursor;
            return (*(Cursor*)cursor).eof ? 1 : 0;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xColumn(IntPtr cursor, IntPtr context, int i)
        {
            IEnumerator enumerator = (IEnumerator)GCHandle.FromIntPtr((*(Cursor*)cursor).enumerator).Target!;
            IntPtr cursorProperties = (*(Cursor*)cursor).properties;
            if (cursorProperties != IntPtr.Zero)
            {
                PropertyInfo[] properties = (PropertyInfo[])GCHandle.FromIntPtr(cursorProperties).Target!;
                if (properties.Length == 0)
                    Sqlite.ResultObject(context, enumerator.Current);
                else
                    Sqlite.ResultObject(context, properties[i].GetValue(enumerator.Current, null));
            }
            else
            {
                object[] record = (object[])enumerator.Current;
                Sqlite.ResultObject(context, record[i]);
            }

            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xRowid(IntPtr cursor, IntPtr pRowid)
        {
            return Sqlite.SQLITE_OK;
        }

        private static string RemoveEnclosingQuotes(string s)
        {
            if (s.Length >= 2)
            {
                foreach (char c in new char[] { '"', '\'' })
                    if (s.StartsWith(c) && s.EndsWith(c))
                        return s.Substring(1, s.Length - 2);
            }

            return s;
        }

        public sqlite3_module module;
        private const int defaultCreationArgc = 3;

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct VTab
        {
            private sqlite3_vtab vtab;

            public IntPtr moduleParams;
        };

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Cursor
        {
            private sqlite3_vtab_cursor cursor;

            public IntPtr enumerator;
            public bool eof;
            public IntPtr properties;
        };
    }
}
