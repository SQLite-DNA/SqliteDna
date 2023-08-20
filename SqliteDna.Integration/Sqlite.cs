using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace SqliteDna.Integration
{
    public class Sqlite
    {
        public const int SQLITE_OK = 0;
        public const int SQLITE_ERROR = 1;

        public const int SQLITE_INTEGER = 1;
        public const int SQLITE_FLOAT = 2;
        public const int SQLITE_TEXT = 3;
        public const int SQLITE_BLOB = 4;
        public const int SQLITE_NULL = 5;

        public static IntPtr SQLITE_STATIC = new IntPtr(0);
        public static IntPtr SQLITE_TRANSIENT = new IntPtr(-1);

        public static unsafe int Init(IntPtr db, byte** pzErrMsg, IntPtr pApi)
        {
            Sqlite.db = db;
            sqliteApi = *(sqlite3_api_routines*)pApi;
            return 0;
        }

        internal static sqlite3_api_routines GetAPI()
        {
            return sqliteApi;
        }

        public static unsafe int CreateFunction(string name, int argc, delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int> func)
        {
            return sqliteApi.create_function_v2(
                db,
                StringToSqliteUtf8(name, out _),
                argc,
                TextEncodings.SQLITE_UTF8,
                IntPtr.Zero,
                func,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );
        }

        public static unsafe int ValueInt(IntPtr* values, int i)
        {
            return sqliteApi.value_int(values[i]);
        }

        public static unsafe long ValueInt64(IntPtr* values, int i)
        {
            return sqliteApi.value_int64(values[i]);
        }

        public static unsafe bool ValueBool(IntPtr* values, int i)
        {
            return sqliteApi.value_int(values[i]) != 0;
        }

        public static unsafe double ValueDouble(IntPtr* values, int i)
        {
            return sqliteApi.value_double(values[i]);
        }

        public static unsafe DateTime ValueDateTime(IntPtr* values, int i)
        {
            switch (sqliteApi.value_type(values[i]))
            {
                case SQLITE_INTEGER:
                    long unixDate = ValueInt64(values, i);
                    return DateTime.UnixEpoch.AddSeconds(unixDate);

                case SQLITE_FLOAT:
                    double julianDate = ValueDouble(values, i);
                    return new DateTime((long)((julianDate - 1721425.5) * TimeSpan.TicksPerDay), DateTimeKind.Utc);

                default:
                    return DateTime.Parse(ValueString(values, i)!, CultureInfo.InvariantCulture);
            }
        }

        public static unsafe Guid ValueGuid(IntPtr* values, int i)
        {
            switch (sqliteApi.value_type(values[i]))
            {
                case SQLITE_BLOB:
                    return new Guid(ValueBlob(values, i)!);

                default:
                    return new Guid(ValueString(values, i)!);
            }
        }

        public static unsafe string? ValueString(IntPtr* values, int i)
        {
            byte* text = sqliteApi.value_text(values[i]);
            if (text == null)
                return null;

            return Encoding.UTF8.GetString(text, sqliteApi.value_bytes(values[i]));
        }

        public static unsafe byte[]? ValueBlob(IntPtr* values, int i)
        {
            byte* pBytes = sqliteApi.value_blob(values[i]);
            if (pBytes == null)
                return null;

            byte[] result = new byte[sqliteApi.value_bytes(values[i])];
            Marshal.Copy(new IntPtr(pBytes), result, 0, result.Length);
            return result;
        }

        public static unsafe int ArgumentInt(string[] arguments, int i)
        {
            return int.Parse(arguments[i]);
        }

        public static unsafe long ArgumentInt64(string[] arguments, int i)
        {
            return long.Parse(arguments[i]);
        }

        public static unsafe bool ArgumentBool(string[] arguments, int i)
        {
            return bool.Parse(arguments[i]);
        }

        public static unsafe double ArgumentDouble(string[] arguments, int i)
        {
            return double.Parse(arguments[i]);
        }

        public static unsafe string ArgumentString(string[] arguments, int i)
        {
            return arguments[i];
        }

        public static unsafe DateTime ArgumentDateTime(string[] arguments, int i)
        {
            return DateTime.Parse(arguments[i], CultureInfo.InvariantCulture);
        }

        public static unsafe void ResultInt(IntPtr context, int i)
        {
            sqliteApi.result_int(context, i);
        }

        public static unsafe void ResultInt64(IntPtr context, long i)
        {
            sqliteApi.result_int64(context, i);
        }

        public static unsafe void ResultBool(IntPtr context, bool b)
        {
            sqliteApi.result_int(context, b ? 1 : 0);
        }

        public static unsafe void ResultDouble(IntPtr context, double d)
        {
            sqliteApi.result_double(context, d);
        }

        public static unsafe void ResultString(IntPtr context, string? s)
        {
            if (s == null)
            {
                sqliteApi.result_null(context);
                return;
            }

            byte* text = StringToSqliteUtf8(s, out int length);
            sqliteApi.result_text(context, text, length, SQLITE_TRANSIENT);
            sqliteApi.free(new IntPtr(text));
        }

        public static unsafe void ResultDateTime(IntPtr context, DateTime dt)
        {
            ResultString(context, dt.ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture));
        }

        public static unsafe void ResultGuid(IntPtr context, Guid g)
        {
            ResultString(context, g.ToString());
        }

        public static unsafe void ResultBlob(IntPtr context, byte[]? bytes)
        {
            if (bytes == null)
            {
                sqliteApi.result_null(context);
                return;
            }

            var pBytes = (byte*)sqliteApi.malloc(bytes.Length);
            Marshal.Copy(bytes, 0, (nint)pBytes, bytes.Length);

            sqliteApi.result_blob(context, pBytes, bytes.Length, SQLITE_TRANSIENT);

            sqliteApi.free(new IntPtr(pBytes));
        }

        public static unsafe void ResultError(IntPtr context, string s)
        {
            byte* text = StringToSqliteUtf8(s, out int length);
            sqliteApi.result_error(context, text, length);
            sqliteApi.free(new IntPtr(text));
        }

        public static unsafe int CreateStaticModule(string name, System.Reflection.PropertyInfo[] properties, Func<string[]?, IEnumerable> func)
        {
            return CreateModule(name, new StaticFunctionModuleParams(func, properties));
        }

        public static unsafe int CreateDynamicModule(string name, Func<string[]?, DynamicTable> func)
        {
            return CreateModule(name, new DynamicFunctionModuleParams(func));
        }

        private static unsafe int CreateModule(string name, BaseFunctionModuleParams moduleParams)
        {
            FunctionModule functionModule = new();
            GCHandle.Alloc(functionModule);
            GCHandle gch = GCHandle.Alloc(moduleParams);
            return sqliteApi.create_module(db, StringToSqliteUtf8(name, out _), ref functionModule.module, GCHandle.ToIntPtr(gch));
        }

        internal static unsafe void ResultObject(IntPtr context, object? o)
        {
            switch (o)
            {
                case null:
                    sqliteApi.result_null(context);
                    break;
                case int into:
                    ResultInt(context, into);
                    break;
                case long longo:
                    ResultInt64(context, longo);
                    break;
                case double doubleo:
                    ResultDouble(context, doubleo);
                    break;
                case string stringo:
                    ResultString(context, stringo);
                    break;
                case DateTime datetimeo:
                    ResultDateTime(context, datetimeo);
                    break;
                case byte[] byteo:
                    ResultBlob(context, byteo);
                    break;
                default:
                    ResultString(context, o.ToString());
                    break;
            }
        }

        internal static unsafe byte* StringToSqliteUtf8(string s, out int length)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var pzString = (byte*)sqliteApi.malloc(bytes.Length + 1);
            Marshal.Copy(bytes, 0, (nint)pzString, bytes.Length);
            pzString[bytes.Length] = 0;
            length = bytes.Length;
            return pzString;
        }

        private static sqlite3_api_routines sqliteApi;
        private static IntPtr db;
    }
}
