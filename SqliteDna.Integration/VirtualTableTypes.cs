using System;
using System.Collections.Generic;

namespace SqliteDna.Integration
{
    public enum SqliteVirtualTableUpdateKind
    {
        Delete = 0,
        Insert = 1,
        Update = 2,
    }

    public enum SqliteValueKind
    {
        Null = 0,
        Integer = 1,
        Float = 2,
        Text = 3,
        Blob = 4,
    }

    public readonly struct SqliteValue
    {
        private readonly IntPtr value;

        internal SqliteValue(IntPtr value)
        {
            this.value = value;
        }

        public SqliteValueKind Kind
        {
            get
            {
                return Sqlite.ValueType(value) switch
                {
                    Sqlite.SQLITE_INTEGER => SqliteValueKind.Integer,
                    Sqlite.SQLITE_FLOAT => SqliteValueKind.Float,
                    Sqlite.SQLITE_TEXT => SqliteValueKind.Text,
                    Sqlite.SQLITE_BLOB => SqliteValueKind.Blob,
                    _ => SqliteValueKind.Null,
                };
            }
        }

        public bool IsNull => Kind == SqliteValueKind.Null;

        public long GetInt64() => Sqlite.ValueInt64(value);

        public double GetDouble() => Sqlite.ValueDouble(value);

        public string? GetString() => Sqlite.ValueString(value);

        public byte[]? GetBlob() => Sqlite.ValueBlob(value);

        public object? ToObject()
        {
            return Kind switch
            {
                SqliteValueKind.Integer => GetInt64(),
                SqliteValueKind.Float => GetDouble(),
                SqliteValueKind.Text => GetString(),
                SqliteValueKind.Blob => GetBlob(),
                _ => null,
            };
        }
    }

    public sealed record SqliteVirtualTableFilter(
        int IndexNumber,
        string? IndexString,
        IReadOnlyList<SqliteValue> Arguments);

    public sealed record SqliteVirtualTableUpdate(
        SqliteVirtualTableUpdateKind Kind,
        long? OldRowId,
        long? NewRowId,
        IReadOnlyList<SqliteValue> ColumnValues)
    {
        public object?[] GetColumnObjects()
        {
            var result = new object?[ColumnValues.Count];
            for (var i = 0; i < ColumnValues.Count; i++)
            {
                result[i] = ColumnValues[i].ToObject();
            }
            return result;
        }
    }

    public readonly struct SqliteVirtualTableUpdateResult
    {
        public SqliteVirtualTableUpdateResult(long rowId)
        {
            RowId = rowId;
        }

        public long RowId { get; }
    }

    public interface ISqliteVirtualTable
    {
        // Column list, e.g. "\"id\" INTEGER, \"name\" TEXT"
        string Schema { get; }

        ISqliteVirtualTableCursor OpenCursor();
    }

    public interface ISqliteWritableVirtualTable : ISqliteVirtualTable
    {
        SqliteVirtualTableUpdateResult Update(SqliteVirtualTableUpdate update);
    }

    public interface ISqliteVirtualTableTransaction
    {
        void Begin();
        void Commit();
        void Rollback();
    }

    public interface ISqliteVirtualTableCursor : IDisposable
    {
        void Filter(SqliteVirtualTableFilter filter);

        void Next();

        bool Eof { get; }

        long RowId { get; }

        object? GetColumnValue(int index);
    }
}

