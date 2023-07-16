#include "pch.h"
#include "Util.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

System::Object^ GetValue(const SQLite::Column& column)
{
	switch (column.getType())
	{
	case 1:
		return column.getInt64();

	case 2:
		return column.getDouble();

	case 4:
	{
		array<Byte>^ result = gcnew array<Byte>(column.getBytes());
		Marshal::Copy(*gcnew IntPtr(const_cast<void*>(column.getBlob())), result, 0, result->Length);
		return result;
	}

	case 5:
		return DBNull::Value;

	default:
		return marshal_as<System::String^>(column.getString().c_str());
	}
}