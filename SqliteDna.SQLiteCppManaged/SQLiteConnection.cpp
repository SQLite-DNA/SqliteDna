#include "pch.h"

#include "SQLiteConnection.h"
#include "Util.h"

using namespace msclr::interop;

namespace SqliteDna
{
	namespace SQLiteCppManaged
	{
		SQLiteConnection::SQLiteConnection(String^ connectionString, String^ extensionPath) : mcontext(gcnew marshal_context()), db(new SQLite::Database(mcontext->marshal_as<const char*>(connectionString), SQLite::OPEN_READWRITE))
		{
			db->loadExtension(mcontext->marshal_as<const char*>(extensionPath), nullptr);
		}

		Object^ SQLiteConnection::ExecuteScalar(String^ commandText)
		{
			SQLite::Statement query(*db, mcontext->marshal_as<const char*>(commandText));
			query.executeStep();
			return GetValue(query.getColumn(0));
		}

		Int32 SQLiteConnection::ExecuteNonQuery(String^ commandText)
		{
			return db->exec(mcontext->marshal_as<const char*>(commandText));
		}

		SQLiteDataReader^ SQLiteConnection::ExecuteReader(String^ commandText)
		{
			return gcnew SQLiteDataReader(*db, commandText);
		}
	}
}