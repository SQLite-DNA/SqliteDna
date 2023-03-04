using System.Runtime.InteropServices;

namespace SqliteDna.Integration
{
    [StructLayout(LayoutKind.Sequential)] // Just a list of function pointers, so we probably don't have to worry about alignment
    internal unsafe struct sqlite3_api_routines
    {
        IntPtr p000; //  void * (*aggregate_context)(sqlite3_context*,int nBytes);
        IntPtr p001; //  int  (*aggregate_count)(sqlite3_context*);
        IntPtr p002; //  int  (*bind_blob)(sqlite3_stmt*,int,const void*,int n,void(*)(void*));
        IntPtr p003; //  int  (*bind_double)(sqlite3_stmt*,int,double);
        IntPtr p004; //  int  (*bind_int)(sqlite3_stmt*,int,int);
        IntPtr p005; //  int  (*bind_int64)(sqlite3_stmt*,int,sqlite_int64);
        IntPtr p006; //  int  (*bind_null)(sqlite3_stmt*,int);
        IntPtr p007; //  int  (*bind_parameter_count)(sqlite3_stmt*);
        IntPtr p008; //  int  (*bind_parameter_index)(sqlite3_stmt*,const char*zName);
        IntPtr p009; //  const char * (*bind_parameter_name)(sqlite3_stmt*,int);
        IntPtr p010; //  int  (*bind_text)(sqlite3_stmt*,int,const char*,int n,void(*)(void*));
        IntPtr p011; //  int  (*bind_text16)(sqlite3_stmt*,int,const void*,int,void(*)(void*));
        IntPtr p012; //  int  (*bind_value)(sqlite3_stmt*,int,const sqlite3_value*);
        IntPtr p013; //  int  (*busy_handler)(sqlite3*,int(*)(void*,int),void*);
        IntPtr p014; //  int  (*busy_timeout)(sqlite3*,int ms);
        IntPtr p015; //  int  (*changes)(sqlite3*);
        IntPtr p016; //  int  (*close)(sqlite3*);
        IntPtr p017; //  int  (*collation_needed)(sqlite3*,void*,void(*)(void*,sqlite3*,
                     //                           int eTextRep,const char*));
        IntPtr p018; //  int  (*collation_needed16)(sqlite3*,void*,void(*)(void*,sqlite3*,
                     //                             int eTextRep,const void*));
        IntPtr p019; //  const void * (*column_blob)(sqlite3_stmt*,int iCol);
        IntPtr p020; //  int  (*column_bytes)(sqlite3_stmt*,int iCol);
        IntPtr p021; //  int  (*column_bytes16)(sqlite3_stmt*,int iCol);
        IntPtr p022; //  int  (*column_count)(sqlite3_stmt*pStmt);
        IntPtr p023; //  const char * (*column_database_name)(sqlite3_stmt*,int);
        IntPtr p024; //  const void * (*column_database_name16)(sqlite3_stmt*,int);
        IntPtr p025; //  const char * (*column_decltype)(sqlite3_stmt*,int i);
        IntPtr p026; //  const void * (*column_decltype16)(sqlite3_stmt*,int);
        IntPtr p027; //  double  (*column_double)(sqlite3_stmt*,int iCol);
        IntPtr p028; //  int  (*column_int)(sqlite3_stmt*,int iCol);
        IntPtr p029; //  sqlite_int64  (*column_int64)(sqlite3_stmt*,int iCol);
        IntPtr p030; //  const char * (*column_name)(sqlite3_stmt*,int);
        IntPtr p031; //  const void * (*column_name16)(sqlite3_stmt*,int);
        IntPtr p032; //  const char * (*column_origin_name)(sqlite3_stmt*,int);
        IntPtr p033; //  const void * (*column_origin_name16)(sqlite3_stmt*,int);
        IntPtr p034; //  const char * (*column_table_name)(sqlite3_stmt*,int);
        IntPtr p035; //  const void * (*column_table_name16)(sqlite3_stmt*,int);
        IntPtr p036; //  const unsigned char * (*column_text)(sqlite3_stmt*,int iCol);
        IntPtr p037; //  const void * (*column_text16)(sqlite3_stmt*,int iCol);
        IntPtr p038; //  int  (*column_type)(sqlite3_stmt*,int iCol);
        IntPtr p039; //  sqlite3_value* (*column_value)(sqlite3_stmt*,int iCol);
        IntPtr p040; //  void * (*commit_hook)(sqlite3*,int(*)(void*),void*);
        IntPtr p041; //  int  (*complete)(const char*sql);
        IntPtr p042; //  int  (*complete16)(const void*sql);
        IntPtr p043; //  int  (*create_collation)(sqlite3*,const char*,int,void*,
                     //                           int(*)(void*,int,const void*,int,const void*));
        IntPtr p044; //  int  (*create_collation16)(sqlite3*,const void*,int,void*,
                     //                             int(*)(void*,int,const void*,int,const void*));
        public readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int, int, IntPtr, IntPtr, IntPtr, IntPtr, int> create_function;
        //  int  (*create_function)(sqlite3*,const char*,int,int,void*,
        //                          void (*xFunc)(sqlite3_context*,int,sqlite3_value**),
        //                          void (*xStep)(sqlite3_context*,int,sqlite3_value**),
        //                          void (*xFinal)(sqlite3_context*));
        IntPtr p046; //  int  (*create_function16)(sqlite3*,const void*,int,int,void*,
                     //                            void (*xFunc)(sqlite3_context*,int,sqlite3_value**),
                     //                            void (*xStep)(sqlite3_context*,int,sqlite3_value**),
                     //                            void (*xFinal)(sqlite3_context*));
        IntPtr p047; //  int (*create_module)(sqlite3*,const char*,const sqlite3_module*,void*);
        IntPtr p048; //  int  (*data_count)(sqlite3_stmt*pStmt);
        IntPtr p049; //  sqlite3 * (*db_handle)(sqlite3_stmt*);
        IntPtr p050; //  int (*declare_vtab)(sqlite3*,const char*);
        IntPtr p051; //  int  (*enable_shared_cache)(int);
        IntPtr p052; //  int  (*errcode)(sqlite3*db);
        IntPtr p053; //  const char * (*errmsg)(sqlite3*);
        IntPtr p054; //  const void * (*errmsg16)(sqlite3*);
        IntPtr p055; //  int  (*exec)(sqlite3*,const char*,sqlite3_callback,void*,char**);
        IntPtr p056; //  int  (*expired)(sqlite3_stmt*);
        IntPtr p057; //  int  (*finalize)(sqlite3_stmt*pStmt);
        public readonly delegate* unmanaged[Cdecl]<IntPtr, void> free; //  void  (*free)(void*);
        IntPtr p059; //  void  (*free_table)(char**result);
        IntPtr p060; //  int  (*get_autocommit)(sqlite3*);
        IntPtr p061; //  void * (*get_auxdata)(sqlite3_context*,int);
        IntPtr p062; //  int  (*get_table)(sqlite3*,const char*,char***,int*,int*,char**);
        IntPtr p063; //  int  (*global_recover)(void);
        IntPtr p064; //  void  (*interruptx)(sqlite3*);
        IntPtr p065; //  sqlite_int64  (*last_insert_rowid)(sqlite3*);
        IntPtr p066; //  const char * (*libversion)(void);
        IntPtr p067; //  int  (*libversion_number)(void);
        public readonly delegate* unmanaged[Cdecl]<int, IntPtr> malloc; // void *(*malloc)(int);
        IntPtr p069; //  char * (*mprintf)(const char*,...);
        IntPtr p070; //  int  (*open)(const char*,sqlite3**);
        IntPtr p071; //  int  (*open16)(const void*,sqlite3**);
        IntPtr p072; //  int  (*prepare)(sqlite3*,const char*,int,sqlite3_stmt**,const char**);
        IntPtr p073; //  int  (*prepare16)(sqlite3*,const void*,int,sqlite3_stmt**,const void**);
        IntPtr p074; //  void * (*profile)(sqlite3*,void(*)(void*,const char*,sqlite_uint64),void*);
        IntPtr p075; //  void  (*progress_handler)(sqlite3*,int,int(*)(void*),void*);
        IntPtr p076; //  void *(*realloc)(void*,int);
        IntPtr p077; //  int  (*reset)(sqlite3_stmt*pStmt);
        IntPtr p078; //  void  (*result_blob)(sqlite3_context*,const void*,int,void(*)(void*));
        public readonly delegate* unmanaged[Cdecl]<IntPtr, double, void> result_double; //  void  (*result_double)(sqlite3_context*,double);
        IntPtr p080; //  void  (*result_error)(sqlite3_context*,const char*,int);
        IntPtr p081; //  void  (*result_error16)(sqlite3_context*,const void*,int);
        IntPtr p082; //  void  (*result_int)(sqlite3_context*,int);
        IntPtr p083; //  void  (*result_int64)(sqlite3_context*,sqlite_int64);
        IntPtr p084; //  void  (*result_null)(sqlite3_context*);
        public readonly delegate* unmanaged[Cdecl]<IntPtr, byte*, int, IntPtr, void> result_text; //  void  (*result_text)(sqlite3_context*,const char*,int,void(*)(void*));
        IntPtr p086; //  void  (*result_text16)(sqlite3_context*,const void*,int,void(*)(void*));
        IntPtr p087; //  void  (*result_text16be)(sqlite3_context*,const void*,int,void(*)(void*));
        IntPtr p088; //  void  (*result_text16le)(sqlite3_context*,const void*,int,void(*)(void*));
        IntPtr p089; //  void  (*result_value)(sqlite3_context*,sqlite3_value*);
        IntPtr p090; //  void * (*rollback_hook)(sqlite3*,void(*)(void*),void*);
        IntPtr p091; //  int  (*set_authorizer)(sqlite3*,int(*)(void*,int,const char*,const char*,
                     //                         const char*,const char*),void*);
        IntPtr p092; //  void  (*set_auxdata)(sqlite3_context*,int,void*,void (*)(void*));
        IntPtr p093; //  char * (*xsnprintf)(int,char*,const char*,...);
        IntPtr p094; //  int  (*step)(sqlite3_stmt*);
        IntPtr p095; //  int  (*table_column_metadata)(sqlite3*,const char*,const char*,const char*,
                     //                                char const**,char const**,int*,int*,int*);
        IntPtr p096; //  void  (*thread_cleanup)(void);
        IntPtr p097; //  int  (*total_changes)(sqlite3*);
        IntPtr p098; //  void * (*trace)(sqlite3*,void(*xTrace)(void*,const char*),void*);
        IntPtr p099; //  int  (*transfer_bindings)(sqlite3_stmt*,sqlite3_stmt*);
        IntPtr p100; //  void * (*update_hook)(sqlite3*,void(*)(void*,int ,char const*,char const*,
                     //                                         sqlite_int64),void*);
        IntPtr p101; //  void * (*user_data)(sqlite3_context*);
        IntPtr p102; //  const void * (*value_blob)(sqlite3_value*);
        IntPtr p103; //  int  (*value_bytes)(sqlite3_value*);
        IntPtr p104; //  int  (*value_bytes16)(sqlite3_value*);
        public readonly delegate* unmanaged[Cdecl]<IntPtr, double> value_double; //  double  (*value_double)(sqlite3_value*);
        IntPtr p106; //  int  (*value_int)(sqlite3_value*);
        IntPtr p107; //  sqlite_int64  (*value_int64)(sqlite3_value*);
        IntPtr p108; //  int  (*value_numeric_type)(sqlite3_value*);
        IntPtr p109; //  const unsigned char * (*value_text)(sqlite3_value*);
        IntPtr p110; //  const void * (*value_text16)(sqlite3_value*);
        IntPtr p111; //  const void * (*value_text16be)(sqlite3_value*);
        IntPtr p112; //  const void * (*value_text16le)(sqlite3_value*);
        IntPtr p113; //  int  (*value_type)(sqlite3_value*);
        IntPtr p114; //  char *(*vmprintf)(const char*,va_list);
        /* Added ??? */
        IntPtr p115; //  int (*overload_function)(sqlite3*, const char *zFuncName, int nArg);
        /* Added by 3.3.13 */
        IntPtr p116; //  int (*prepare_v2)(sqlite3*,const char*,int,sqlite3_stmt**,const char**);
        IntPtr p117; //  int (*prepare16_v2)(sqlite3*,const void*,int,sqlite3_stmt**,const void**);
        IntPtr p118; //  int (*clear_bindings)(sqlite3_stmt*);
        /* Added by 3.4.1 */
        IntPtr p119; //  int (*create_module_v2)(sqlite3*,const char*,const sqlite3_module*,void*,
                     //                          void (*xDestroy)(void *));
        /* Added by 3.5.0 */
        IntPtr p120; //  int (*bind_zeroblob)(sqlite3_stmt*,int,int);
        IntPtr p121; //  int (*blob_bytes)(sqlite3_blob*);
        IntPtr p122; //  int (*blob_close)(sqlite3_blob*);
        IntPtr p123; //  int (*blob_open)(sqlite3*,const char*,const char*,const char*,sqlite3_int64,
                     //                   int,sqlite3_blob**);
        IntPtr p124; //  int (*blob_read)(sqlite3_blob*,void*,int,int);
        IntPtr p125; //  int (*blob_write)(sqlite3_blob*,const void*,int,int);
        IntPtr p126; //  int (*create_collation_v2)(sqlite3*,const char*,int,void*,
                     //                             int(*)(void*,int,const void*,int,const void*),
                     //                             void(*)(void*));
        IntPtr p127; //  int (*file_control)(sqlite3*,const char*,int,void*);
        IntPtr p128; //  sqlite3_int64 (*memory_highwater)(int);
        IntPtr p129; //  sqlite3_int64 (*memory_used)(void);
        IntPtr p130; //  sqlite3_mutex *(*mutex_alloc)(int);
        IntPtr p131; //  void (*mutex_enter)(sqlite3_mutex*);
        IntPtr p132; //  void (*mutex_free)(sqlite3_mutex*);
        IntPtr p133; //  void (*mutex_leave)(sqlite3_mutex*);
        IntPtr p134; //  int (*mutex_try)(sqlite3_mutex*);
        IntPtr p135; //  int (*open_v2)(const char*,sqlite3**,int,const char*);
        IntPtr p136; //  int (*release_memory)(int);
        IntPtr p137; //  void (*result_error_nomem)(sqlite3_context*);
        IntPtr p138; //  void (*result_error_toobig)(sqlite3_context*);
        IntPtr p139; //  int (*sleep)(int);
        IntPtr p140; //  void (*soft_heap_limit)(int);
        IntPtr p141; //  sqlite3_vfs *(*vfs_find)(const char*);
        IntPtr p142; //  int (*vfs_register)(sqlite3_vfs*,int);
        IntPtr p143; //  int (*vfs_unregister)(sqlite3_vfs*);
        IntPtr p144; //  int (*xthreadsafe)(void);
        IntPtr p145; //  void (*result_zeroblob)(sqlite3_context*,int);
        IntPtr p146; //  void (*result_error_code)(sqlite3_context*,int);
        IntPtr p147; //  int (*test_control)(int, ...);
        IntPtr p148; //  void (*randomness)(int,void*);
        IntPtr p149; //  sqlite3 *(*context_db_handle)(sqlite3_context*);
        IntPtr p150; //  int (*extended_result_codes)(sqlite3*,int);
        IntPtr p151; //  int (*limit)(sqlite3*,int,int);
        IntPtr p152; //  sqlite3_stmt *(*next_stmt)(sqlite3*,sqlite3_stmt*);
        IntPtr p153; //  const char *(*sql)(sqlite3_stmt*);
        IntPtr p154; //  int (*status)(int,int*,int*,int);
        IntPtr p155; //  int (*backup_finish)(sqlite3_backup*);
        IntPtr p156; //  sqlite3_backup *(*backup_init)(sqlite3*,const char*,sqlite3*,const char*);
        IntPtr p157; //  int (*backup_pagecount)(sqlite3_backup*);
        IntPtr p158; //  int (*backup_remaining)(sqlite3_backup*);
        IntPtr p159; //  int (*backup_step)(sqlite3_backup*,int);
        IntPtr p160; //  const char *(*compileoption_get)(int);
        IntPtr p161; //  int (*compileoption_used)(const char*);
        public readonly delegate* unmanaged[Cdecl]<IntPtr, byte*, int, TextEncodings, IntPtr, delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int>, IntPtr, IntPtr, IntPtr, int> create_function_v2;
        //  int (*create_function_v2)(sqlite3*,const char*,int,int,void*,
        //                            void (*xFunc)(sqlite3_context*,int,sqlite3_value**),
        //                            void (*xStep)(sqlite3_context*,int,sqlite3_value**),
        //                            void (*xFinal)(sqlite3_context*),
        //                            void(*xDestroy)(void*));
        IntPtr p163; //  int (*db_config)(sqlite3*,int,...);
        IntPtr p164; //  sqlite3_mutex *(*db_mutex)(sqlite3*);
        IntPtr p165; //  int (*db_status)(sqlite3*,int,int*,int*,int);
        IntPtr p166; //  int (*extended_errcode)(sqlite3*);
        IntPtr p167; //  void (*log)(int,const char*,...);
        IntPtr p168; //  sqlite3_int64 (*soft_heap_limit64)(sqlite3_int64);
        IntPtr p169; //  const char *(*sourceid)(void);
        IntPtr p170; //  int (*stmt_status)(sqlite3_stmt*,int,int);
        IntPtr p171; //  int (*strnicmp)(const char*,const char*,int);
        IntPtr p172; //  int (*unlock_notify)(sqlite3*,void(*)(void**,int),void*);
        IntPtr p173; //  int (*wal_autocheckpoint)(sqlite3*,int);
        IntPtr p174; //  int (*wal_checkpoint)(sqlite3*,const char*);
        IntPtr p175; //  void *(*wal_hook)(sqlite3*,int(*)(void*,sqlite3*,const char*,int),void*);
        IntPtr p176; //  int (*blob_reopen)(sqlite3_blob*,sqlite3_int64);
        IntPtr p177; //  int (*vtab_config)(sqlite3*,int op,...);
        IntPtr p178; //  int (*vtab_on_conflict)(sqlite3*);
        /* Version 3.7.16 and later */
        IntPtr p179; //  int (*close_v2)(sqlite3*);
        IntPtr p180; //  const char *(*db_filename)(sqlite3*,const char*);
        IntPtr p181; //  int (*db_readonly)(sqlite3*,const char*);
        IntPtr p182; //  int (*db_release_memory)(sqlite3*);
        IntPtr p183; //  const char *(*errstr)(int);
        IntPtr p184; //  int (*stmt_busy)(sqlite3_stmt*);
        IntPtr p185; //  int (*stmt_readonly)(sqlite3_stmt*);
        IntPtr p186; //  int (*stricmp)(const char*,const char*);
        IntPtr p187; //  int (*uri_boolean)(const char*,const char*,int);
        IntPtr p188; //  sqlite3_int64 (*uri_int64)(const char*,const char*,sqlite3_int64);
        IntPtr p189; //  const char *(*uri_parameter)(const char*,const char*);
        IntPtr p190; //  char *(*xvsnprintf)(int,char*,const char*,va_list);
        IntPtr p191; //  int (*wal_checkpoint_v2)(sqlite3*,const char*,int,int*,int*);
        /* Version 3.8.7 and later */
        IntPtr p192; //  int (*auto_extension)(void(*)(void));
        IntPtr p193; //  int (*bind_blob64)(sqlite3_stmt*,int,const void*,sqlite3_uint64,
                     //                     void(*)(void*));
        IntPtr p194; //  int (*bind_text64)(sqlite3_stmt*,int,const char*,sqlite3_uint64,
                     //                      void(*)(void*),unsigned char);
        IntPtr p195; //  int (*cancel_auto_extension)(void(*)(void));
        IntPtr p196; //  int (*load_extension)(sqlite3*,const char*,const char*,char**);
        IntPtr p197; //  void *(*malloc64)(sqlite3_uint64);
        IntPtr p198; //  sqlite3_uint64 (*msize)(void*);
        IntPtr p199; //  void *(*realloc64)(void*,sqlite3_uint64);
        IntPtr p200; //  void (*reset_auto_extension)(void);
        IntPtr p201; //  void (*result_blob64)(sqlite3_context*,const void*,sqlite3_uint64,
                     //                        void(*)(void*));
        IntPtr p202; //  void (*result_text64)(sqlite3_context*,const char*,sqlite3_uint64,
                     //                         void(*)(void*), unsigned char);
        IntPtr p203; //  int (*strglob)(const char*,const char*);
        /* Version 3.8.11 and later */
        IntPtr p204; //  sqlite3_value *(*value_dup)(const sqlite3_value*);
        IntPtr p205; //  void (*value_free)(sqlite3_value*);
        IntPtr p206; //  int (*result_zeroblob64)(sqlite3_context*,sqlite3_uint64);
        IntPtr p207; //  int (*bind_zeroblob64)(sqlite3_stmt*, int, sqlite3_uint64);
        /* Version 3.9.0 and later */
        IntPtr p208; //  unsigned int (*value_subtype)(sqlite3_value*);
        IntPtr p209; //  void (*result_subtype)(sqlite3_context*,unsigned int);
        /* Version 3.10.0 and later */
        IntPtr p210; //  int (*status64)(int,sqlite3_int64*,sqlite3_int64*,int);
        IntPtr p211; //  int (*strlike)(const char*,const char*,unsigned int);
        IntPtr p212; //  int (*db_cacheflush)(sqlite3*);
        /* Version 3.12.0 and later */
        IntPtr p213; //  int (*system_errno)(sqlite3*);
        /* Version 3.14.0 and later */
        IntPtr p214; //  int (*trace_v2)(sqlite3*,unsigned,int(*)(unsigned,void*,void*,void*),void*);
        IntPtr p215; //  char *(*expanded_sql)(sqlite3_stmt*);
        /* Version 3.18.0 and later */
        IntPtr p216; //  void (*set_last_insert_rowid)(sqlite3*,sqlite3_int64);
        /* Version 3.20.0 and later */
        IntPtr p217; //  int (*prepare_v3)(sqlite3*,const char*,int,unsigned int,
                     //                    sqlite3_stmt**,const char**);
        IntPtr p218; //  int (*prepare16_v3)(sqlite3*,const void*,int,unsigned int,
                     //                      sqlite3_stmt**,const void**);
        IntPtr p219; //  int (*bind_pointer)(sqlite3_stmt*,int,void*,const char*,void(*)(void*));
        IntPtr p220; //  void (*result_pointer)(sqlite3_context*,void*,const char*,void(*)(void*));
        IntPtr p221; //  void *(*value_pointer)(sqlite3_value*,const char*);
        IntPtr p222; //  int (*vtab_nochange)(sqlite3_context*);
        IntPtr p223; //  int (*value_nochange)(sqlite3_value*);
        IntPtr p224; //  const char *(*vtab_collation)(sqlite3_index_info*,int);
        /* Version 3.24.0 and later */
        IntPtr p225; //  int (*keyword_count)(void);
        IntPtr p226; //  int (*keyword_name)(int,const char**,int*);
        IntPtr p227; //  int (*keyword_check)(const char*,int);
        IntPtr p228; //  sqlite3_str *(*str_new)(sqlite3*);
        IntPtr p229; //  char *(*str_finish)(sqlite3_str*);
        IntPtr p230; //  void (*str_appendf)(sqlite3_str*, const char *zFormat, ...);
        IntPtr p231; //  void (*str_vappendf)(sqlite3_str*, const char *zFormat, va_list);
        IntPtr p232; //  void (*str_append)(sqlite3_str*, const char *zIn, int N);
        IntPtr p233; //  void (*str_appendall)(sqlite3_str*, const char *zIn);
        IntPtr p234; //  void (*str_appendchar)(sqlite3_str*, int N, char C);
        IntPtr p235; //  void (*str_reset)(sqlite3_str*);
        IntPtr p236; //  int (*str_errcode)(sqlite3_str*);
        IntPtr p237; //  int (*str_length)(sqlite3_str*);
        IntPtr p238; //  char *(*str_value)(sqlite3_str*);
        /* Version 3.25.0 and later */
        IntPtr p239; //  int (*create_window_function)(sqlite3*,const char*,int,int,void*,
                     //                            void (*xStep)(sqlite3_context*,int,sqlite3_value**),
                     //                            void (*xFinal)(sqlite3_context*),
                     //                            void (*xValue)(sqlite3_context*),
                     //                            void (*xInv)(sqlite3_context*,int,sqlite3_value**),
                     //                            void(*xDestroy)(void*));
        /* Version 3.26.0 and later */
        IntPtr p240; //  const char *(*normalized_sql)(sqlite3_stmt*);
        /* Version 3.28.0 and later */
        IntPtr p241; //  int (*stmt_isexplain)(sqlite3_stmt*);
        IntPtr p242; //  int (*value_frombind)(sqlite3_value*);
        /* Version 3.30.0 and later */
        IntPtr p243; //  int (*drop_modules)(sqlite3*,const char**);
        /* Version 3.31.0 and later */
        IntPtr p244; //  sqlite3_int64 (*hard_heap_limit64)(sqlite3_int64);
        IntPtr p245; //  const char *(*uri_key)(const char*,int);
        IntPtr p246; //  const char *(*filename_database)(const char*);
        IntPtr p247; //  const char *(*filename_journal)(const char*);
        IntPtr p248; //  const char *(*filename_wal)(const char*);
        /* Version 3.32.0 and later */
        IntPtr p249; //  char *(*create_filename)(const char*,const char*,const char*,
                     //                           int,const char**);
        IntPtr p250; //  void (*free_filename)(char*);
        IntPtr p251; //  sqlite3_file *(*database_file_object)(const char*);
        /* Version 3.34.0 and later */
        IntPtr p252; //  int (*txn_state)(sqlite3*,const char*);
    };

    /*
    ** CAPI3REF: Text Encodings
    **
    ** These constant define integer codes that represent the various
    ** text encodings supported by SQLite.
    */
    internal enum TextEncodings : int
    {
        SQLITE_UTF8 = 1,    /* IMP: R-37514-35566 */
        SQLITE_UTF16LE = 2,    /* IMP: R-03371-37637 */
        SQLITE_UTF16BE = 3,    /* IMP: R-51971-34154 */
        SQLITE_UTF16 = 4,    /* Use native byte order */
        SQLITE_ANY = 5,    /* Deprecated */
        SQLITE_UTF16_ALIGNED = 8,    /* sqlite3_create_collation only */
    }
}
