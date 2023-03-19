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
				SQLite::Statement query(db, "SELECT Foo2()");
				query.executeStep();
				Assert::AreEqual(2, query.getColumn(0).getInt());
			}
			{
				SQLite::Statement query(db, "SELECT Foo42()");
				query.executeStep();
				Assert::AreEqual(42, query.getColumn(0).getInt());
			}
			{
				SQLite::Statement query(db, "SELECT FooHello()");
				query.executeStep();
				Assert::AreEqual(std::string("Hello"), query.getColumn(0).getString());
			}
			{
				SQLite::Statement query(db, "SELECT MyIntSum(2, 3)");
				query.executeStep();
				Assert::AreEqual(5, query.getColumn(0).getInt());
			}
			{
				SQLite::Statement query(db, "SELECT MyInt64Sum(223372036854775807, 2)");
				query.executeStep();
				Assert::AreEqual(223372036854775809, query.getColumn(0).getInt64());
			}
			{
				SQLite::Statement query(db, "SELECT MyDoubleSum(1.2, 1.3)");
				query.executeStep();
				Assert::AreEqual(2.5, query.getColumn(0).getDouble());
			}
			{
				SQLite::Statement query(db, "SELECT MyConcat('Hello', 'world')");
				query.executeStep();
				Assert::AreEqual(std::string("Hello world"), query.getColumn(0).getString());
			}
			{
				Assert::AreEqual(0, db.exec("SELECT Nop()"));
			}
			{
				Assert::AreEqual(0, db.exec("SELECT IncrementInternalCounter()"));

				SQLite::Statement query(db, "SELECT GetInternalCounter()");
				query.executeStep();
				Assert::AreEqual(1, query.getColumn(0).getInt());
			}
			{
				SQLite::Statement query1(db, "SELECT MyNullableConcat('Hello', 'world')");
				query1.executeStep();
				Assert::AreEqual(std::string("Hello world"), query1.getColumn(0).getString());

				SQLite::Statement query2(db, "SELECT MyNullableConcat(NULL, NULL)");
				query2.executeStep();
				Assert::IsTrue(query2.getColumn(0).isNull());

				SQLite::Statement query3(db, "SELECT MyNullableConcat('Hello', NULL)");
				query3.executeStep();
				Assert::AreEqual(std::string("Hello"), query3.getColumn(0).getString());

				SQLite::Statement query4(db, "SELECT MyNullableConcat(NULL, 'world')");
				query4.executeStep();
				Assert::AreEqual(std::string("world"), query4.getColumn(0).getString());
			}
			{
				Assert::ExpectException<SQLite::Exception>([&]() { db.exec("SELECT Noo1()"); });
			}
		}
	};
}
