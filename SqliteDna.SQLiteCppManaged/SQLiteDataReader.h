#pragma once

using namespace System;

namespace SqliteDna
{
	namespace SQLiteCppManaged
	{
		public ref class SQLiteDataReader
		{
		public:
			SQLiteDataReader(const SQLite::Database& db, String^ queryText);
			bool Read();
			Object^ GetItem(String^ name);

		private:
			msclr::auto_handle<msclr::interop::marshal_context> mcontext;
			AutoPtr<SQLite::Statement> query;
		};
	}
}