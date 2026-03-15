using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SqliteDna.Integration
{
    internal sealed class ManagedVirtualTableModule
    {
        public ManagedVirtualTableModule()
        {
            CreateModule();
        }

        private unsafe void CreateModule()
        {
            module = new();
            module.iVersion = 1;
            module.xCreate = &xCreate;
            module.xConnect = &xCreate;
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
            module.xUpdate = &xUpdate;
            module.xBegin = &xBegin;
            module.xCommit = &xCommit;
            module.xRollback = &xRollback;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xCreate(IntPtr db, IntPtr aux, int argc, IntPtr argv, IntPtr* vtab, IntPtr err)
        {
            var factory = (Func<string[]?, ISqliteVirtualTable>)GCHandle.FromIntPtr(aux).Target!;
            var args = ParseCreateArguments(argc, argv);

            ISqliteVirtualTable table;
            try
            {
                table = factory(args);
            }
            catch (Exception ex)
            {
                SetCreateError(err, ex.Message);
                return Sqlite.SQLITE_ERROR;
            }

            var schema = table.Schema ?? throw new InvalidOperationException("Virtual table schema must not be null.");
            Sqlite.GetAPI().declare_vtab(db, Sqlite.StringToSqliteUtf8($"CREATE TABLE x({schema})", out _));

            (*vtab) = Sqlite.GetAPI().malloc(sizeof(VTab));
            new Span<byte>((void*)(*vtab), sizeof(VTab)).Clear();
            var gch = GCHandle.Alloc(table);
            (*(VTab*)(*vtab)).tableHandle = GCHandle.ToIntPtr(gch);
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xBestIndex(IntPtr vtab, IntPtr info)
        {
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xDisconnect(IntPtr vtab)
        {
            DisconnectImpl(vtab);
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xDestroy(IntPtr vtab)
        {
            DisconnectImpl(vtab);
            return Sqlite.SQLITE_OK;
        }

        private static unsafe void DisconnectImpl(IntPtr vtab)
        {
            var handlePtr = (*(VTab*)vtab).tableHandle;
            if (handlePtr != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(handlePtr);
                try
                {
                    if (handle.Target is IDisposable d)
                        d.Dispose();
                }
                finally
                {
                    handle.Free();
                }
            }

            Sqlite.GetAPI().free(vtab);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xOpen(IntPtr vtab, IntPtr* pcursor)
        {
            var table = (ISqliteVirtualTable)GCHandle.FromIntPtr((*(VTab*)vtab).tableHandle).Target!;
            var cursor = table.OpenCursor();

            (*pcursor) = Sqlite.GetAPI().malloc(sizeof(Cursor));
            new Span<byte>((void*)(*pcursor), sizeof(Cursor)).Clear();
            Cursor* cur = (Cursor*)(*pcursor);
            (*cur).cursorHandle = GCHandle.ToIntPtr(GCHandle.Alloc(cursor));
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xClose(IntPtr cursor)
        {
            var handlePtr = (*(Cursor*)cursor).cursorHandle;
            if (handlePtr != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(handlePtr);
                try
                {
                    if (handle.Target is IDisposable d)
                        d.Dispose();
                }
                finally
                {
                    handle.Free();
                }
            }

            Sqlite.GetAPI().free(cursor);
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xFilter(IntPtr cursor, int idxNum, IntPtr idxStr, int argc, IntPtr argv)
        {
            var cur = (ISqliteVirtualTableCursor)GCHandle.FromIntPtr((*(Cursor*)cursor).cursorHandle).Target!;

            var args = new List<SqliteValue>(argc);
            if (argc > 0)
            {
                IntPtr* values = (IntPtr*)argv;
                for (int i = 0; i < argc; i++)
                {
                    args.Add(new SqliteValue(values[i]));
                }
            }

            string? idx = idxStr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(idxStr);
            cur.Filter(new SqliteVirtualTableFilter(idxNum, idx, args));
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xNext(IntPtr cursor)
        {
            var cur = (ISqliteVirtualTableCursor)GCHandle.FromIntPtr((*(Cursor*)cursor).cursorHandle).Target!;
            cur.Next();
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xEof(IntPtr cursor)
        {
            var cur = (ISqliteVirtualTableCursor)GCHandle.FromIntPtr((*(Cursor*)cursor).cursorHandle).Target!;
            return cur.Eof ? 1 : 0;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xColumn(IntPtr cursor, IntPtr context, int i)
        {
            var cur = (ISqliteVirtualTableCursor)GCHandle.FromIntPtr((*(Cursor*)cursor).cursorHandle).Target!;
            Sqlite.ResultObject(context, cur.GetColumnValue(i));
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xRowid(IntPtr cursor, IntPtr pRowid)
        {
            var cur = (ISqliteVirtualTableCursor)GCHandle.FromIntPtr((*(Cursor*)cursor).cursorHandle).Target!;
            *(long*)pRowid = cur.RowId;
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xUpdate(IntPtr vtab, int argc, IntPtr argv, IntPtr pRowid)
        {
            var table = (ISqliteVirtualTable)GCHandle.FromIntPtr((*(VTab*)vtab).tableHandle).Target!;
            if (table is not ISqliteWritableVirtualTable writable)
                return Sqlite.SQLITE_READONLY;

            IntPtr* values = (IntPtr*)argv;

            // SQLite calling convention:
            // - argc==1 => DELETE (argv[0]=old rowid)
            // - otherwise: argv[0]=old rowid or NULL (INSERT), argv[1]=new rowid or NULL, argv[2..]=column values
            if (argc == 1)
            {
                var old = new SqliteValue(values[0]);
                var oldId = old.IsNull ? (long?)null : old.GetInt64();
                try
                {
                    writable.Update(new SqliteVirtualTableUpdate(SqliteVirtualTableUpdateKind.Delete, oldId, null, Array.Empty<SqliteValue>()));
                    return Sqlite.SQLITE_OK;
                }
                catch (Exception ex)
                {
                    SetVirtualTableError(vtab, ex.Message);
                    return Sqlite.SQLITE_ERROR;
                }
            }

            var oldRowIdValue = new SqliteValue(values[0]);
            var newRowIdValue = new SqliteValue(values[1]);
            long? oldRowId = oldRowIdValue.IsNull ? null : oldRowIdValue.GetInt64();
            long? newRowId = newRowIdValue.IsNull ? null : newRowIdValue.GetInt64();

            var columnValues = new List<SqliteValue>(Math.Max(0, argc - 2));
            for (int i = 2; i < argc; i++)
            {
                columnValues.Add(new SqliteValue(values[i]));
            }

            var kind = oldRowId == null ? SqliteVirtualTableUpdateKind.Insert : SqliteVirtualTableUpdateKind.Update;
            try
            {
                var result = writable.Update(new SqliteVirtualTableUpdate(kind, oldRowId, newRowId, columnValues));
                *(long*)pRowid = result.RowId;
                return Sqlite.SQLITE_OK;
            }
            catch (Exception ex)
            {
                SetVirtualTableError(vtab, ex.Message);
                return Sqlite.SQLITE_ERROR;
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xBegin(IntPtr vtab)
        {
            var table = (ISqliteVirtualTable)GCHandle.FromIntPtr((*(VTab*)vtab).tableHandle).Target!;
            if (table is ISqliteVirtualTableTransaction tx)
                tx.Begin();
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xCommit(IntPtr vtab)
        {
            var table = (ISqliteVirtualTable)GCHandle.FromIntPtr((*(VTab*)vtab).tableHandle).Target!;
            if (table is ISqliteVirtualTableTransaction tx)
                tx.Commit();
            return Sqlite.SQLITE_OK;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int xRollback(IntPtr vtab)
        {
            var table = (ISqliteVirtualTable)GCHandle.FromIntPtr((*(VTab*)vtab).tableHandle).Target!;
            if (table is ISqliteVirtualTableTransaction tx)
                tx.Rollback();
            return Sqlite.SQLITE_OK;
        }

        private static unsafe string[]? ParseCreateArguments(int argc, IntPtr argv)
        {
            if (argc <= defaultCreationArgc)
                return null;

            var arguments = new string[argc - defaultCreationArgc];
            char** pargv = (char**)argv;
            for (int i = 0; i < arguments.Length; ++i)
            {
                arguments[i] = RemoveEnclosingQuotes(Marshal.PtrToStringAnsi((IntPtr)(*(pargv + i + defaultCreationArgc)))!);
            }

            return arguments;
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

        private static unsafe void SetCreateError(IntPtr err, string message)
        {
            if (err == IntPtr.Zero)
                return;

            *(IntPtr*)err = (IntPtr)Sqlite.StringToSqliteUtf8(message, out _);
        }

        private static unsafe void SetVirtualTableError(IntPtr vtab, string message)
        {
            if (vtab == IntPtr.Zero)
                return;

            ref var nativeVtab = ref (*(VTab*)vtab).vtab;
            if (nativeVtab.zErrMsg != IntPtr.Zero)
                Sqlite.GetAPI().free(nativeVtab.zErrMsg);

            nativeVtab.zErrMsg = (IntPtr)Sqlite.StringToSqliteUtf8(message, out _);
        }

        public sqlite3_module module;
        private const int defaultCreationArgc = 3;

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct VTab
        {
            internal sqlite3_vtab vtab;

            public IntPtr tableHandle;
        };

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Cursor
        {
            private sqlite3_vtab_cursor cursor;

            public IntPtr cursorHandle;
        };
    }
}
