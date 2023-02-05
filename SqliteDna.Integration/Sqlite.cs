using System.Runtime.InteropServices;
using System.Text;

namespace SqliteDna.Integration
{
    public class Sqlite
    {
        public const int SQLITE_OK = 0;
        public const int SQLITE_ERROR = 1;

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
                StringToSqliteUtf8(name),
                argc,
                TextEncodings.SQLITE_UTF8,
                IntPtr.Zero,
                func,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );
        }

        public static unsafe void ResultDouble(IntPtr context, double d)
        {
            sqliteApi.result_double(context, d);
        }

        private static unsafe byte* StringToSqliteUtf8(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var pzString = (byte*)sqliteApi.malloc(bytes.Length + 1);
            Marshal.Copy(bytes, 0, (nint)pzString, bytes.Length);
            pzString[bytes.Length] = 0;
            return pzString;
        }

        private static sqlite3_api_routines sqliteApi;
        private static IntPtr db;
    }
}
