using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SqliteDna
{
    public class Extension
    {
        const int SQLITE_OK = 0; /* Successful result */

        [UnmanagedCallersOnly(EntryPoint = "sqlite3_dotnet_init", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static int sqlite3_dotnet_init( /* <== Change this name, maybe */
            /* sqlite3* */ IntPtr db,
            /* char** */ IntPtr pzErrMsg,
            /* const sqlite3_api_routines* */ IntPtr pApi
            )
        {
            int rc = SQLITE_OK;
            //        SQLITE_EXTENSION_INIT2(pApi);
            /* insert code to initialize your extension here */
            Console.WriteLine("Hello from SqliteDna!");
            return rc;
        }
    }
}