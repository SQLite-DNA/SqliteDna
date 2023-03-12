using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace SqliteDna.Integration
{
    public class Sqlite
    {
        public const int SQLITE_OK = 0;
        public const int SQLITE_ERROR = 1;

        public static IntPtr SQLITE_STATIC = new IntPtr(0);
        public static IntPtr SQLITE_TRANSIENT = new IntPtr(-1);

        public static unsafe int Init(IntPtr db, byte** pzErrMsg, IntPtr pApi)
        {
            Sqlite.db = db;
            sqliteApi = *(sqlite3_api_routines*)pApi;
            return 0;
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

        public static unsafe double ValueDouble(IntPtr* values, int i)
        {
            return sqliteApi.value_double(values[i]);
        }

        public static unsafe DateTime ValueDateTime(IntPtr* values, int i)
        {
            return DateTime.Parse(ValueString(values, i)!, CultureInfo.InvariantCulture);
        }

        public static unsafe string? ValueString(IntPtr* values, int i)
        {
            byte* text = sqliteApi.value_text(values[i]);
            if (text == null)
                return null;

            int length = 0;
            while (text[length] != 0)
            {
                ++length;
            }

            return Encoding.UTF8.GetString(text, length);
        }

        public static unsafe void ResultInt(IntPtr context, int i)
        {
            sqliteApi.result_int(context, i);
        }

        public static unsafe void ResultInt64(IntPtr context, long i)
        {
            sqliteApi.result_int64(context, i);
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

        private static unsafe byte* StringToSqliteUtf8(string s, out int length)
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
