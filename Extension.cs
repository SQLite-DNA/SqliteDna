using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SqliteDna
{
    public class Extension
    {
        static sqlite3_api_routines _sqliteApi;

        const int SQLITE_OK = 0; /* Successful result */
        const int SQLITE_ERROR = 1;

        // When using PublishAOT and not DNNE, the final .dll is called SqliteDna.dll instead of SqliteDnaNE.dll.
        // By default, SQLite looks for a function with a name derived from the .dll name.
        [UnmanagedCallersOnly(EntryPoint = "sqlite3_sqlitedna_init", CallConvs = new[] { typeof(CallConvCdecl) })]
        public unsafe static int sqlite3_sqlitedna_init( /* <== Change this name, maybe */
            /* sqlite3* */ IntPtr db,
            /* char** */ byte** pzErrMsg,
            /* sqlite3_api_routines* */ IntPtr pApi
            )
        {
            _sqliteApi = *(sqlite3_api_routines*)pApi;

            var create_result = _sqliteApi.create_function_v2(
                db,
                StringToSqliteUtf8("addthem"),
                2,
                TextEncodings.SQLITE_UTF8,
                IntPtr.Zero,
                &AddThem,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );

            return create_result;

            Console.WriteLine("Hello from SqliteDna!");

            *pzErrMsg = StringToSqliteUtf8("Things went wrong!");
            return SQLITE_ERROR;

            //int rc = SQLITE_OK;
            ////        SQLITE_EXTENSION_INIT2(pApi);
            ///* insert code to initialize your extension here */
            //Console.WriteLine("Hello from SqliteDna!");
            //return rc;
        }

        static unsafe byte* StringToSqliteUtf8(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var pzString = (byte*)_sqliteApi.malloc(bytes.Length + 1);
            Marshal.Copy(bytes, 0, (nint)pzString, bytes.Length);
            pzString[bytes.Length] = 0;
            return pzString;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static unsafe int AddThem(IntPtr /* sqlite3_context* */ context, int argc, IntPtr /* sqlite3_value** */ argv)
        {
            IntPtr* pValues = (IntPtr*)argv;

            double a = _sqliteApi.value_double(pValues[0]);
            double b = _sqliteApi.value_double(pValues[1]);
            double c = a + b;
            _sqliteApi.result_double(context, c);
            return SQLITE_OK;
        }
    }
}