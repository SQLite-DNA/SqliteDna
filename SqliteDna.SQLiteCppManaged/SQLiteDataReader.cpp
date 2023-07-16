#include "pch.h"

#include "SQLiteDataReader.h"
#include "Util.h"

using namespace msclr::interop;
using namespace System::Runtime::InteropServices;

namespace SqliteDna
{
	namespace SQLiteCppManaged
	{
		SQLiteDataReader::SQLiteDataReader(const SQLite::Database& db, String^ queryText) : mcontext(gcnew marshal_context()), query(new SQLite::Statement(db, mcontext->marshal_as<const char*>(queryText)))
		{
		}

		bool SQLiteDataReader::Read()
		{
			return query->executeStep();
		}

		Object^ SQLiteDataReader::GetItem(String^ name)
		{
			return GetValue(query->getColumn(mcontext->marshal_as<const char*>(name)));
		}
	}
}