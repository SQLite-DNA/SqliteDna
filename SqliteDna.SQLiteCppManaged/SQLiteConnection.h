#pragma once

#include "SQLiteDataReader.h"

using namespace System;

namespace SqliteDna
{
	namespace SQLiteCppManaged
	{
		public ref class SQLiteConnection
		{
		public:
			SQLiteConnection(String^ connectionString, String^ extensionPath);
			Object^ ExecuteScalar(String^ commandText);
			Int32 ExecuteNonQuery(String^ commandText);
			SQLiteDataReader^ ExecuteReader(String^ commandText);

		private:
			msclr::auto_handle<msclr::interop::marshal_context> mcontext;
			AutoPtr<SQLite::Database> db;
		};
	}
}