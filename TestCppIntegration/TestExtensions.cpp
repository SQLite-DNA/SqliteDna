#include "pch.h"
#include "CppUnitTest.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace TestCppIntegration
{
	TEST_CLASS(TestExtensions)
	{
	public:

		TEST_METHOD(FunctionsAOT)
		{
			Functions("TestAOT.dll");
		}

		TEST_METHOD(FunctionsDNNE)
		{
			Functions("TestDNNENE.dll");
		}

	private:

		void Functions(const std::string& extensionFile)
		{
			SQLite::Database db(":memory:");
			db.loadExtension(extensionFile.c_str(), nullptr);
			{
				SQLite::Statement   query(db, "SELECT Foo42()");
				query.executeStep();
				Assert::AreEqual(42, query.getColumn(0).getInt());
			}
		}
	};
}
