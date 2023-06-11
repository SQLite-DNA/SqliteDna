# Sqlite-DNA

Sqlite-DNA supports writing [SQLite extensions](https://www.sqlite.org/loadext.html) with C# and .NET 7.

We use the [DNNE library](https://github.com/AaronRobinsonMSFT/DNNE), [AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/) and [Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) to create a native library exporting the .NET functions.

### Getting started

Create a C# class library .NET 7 project and reference the SqliteDna package (or use Samples\Minimal\Minimal.csproj):

```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net7.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="SqliteDna" Version="*-*" />
	</ItemGroup>

</Project>
```

Write your custom function and mark it with the [SqliteFunction] attribute:

```csharp
using SqliteDna.Integration;

namespace Minimal
{
    public class MyFunctions
    {
        [SqliteFunction]
        public static int Foo2()
        {
            return 2;
        }

        [SqliteFunction]
        public static int Foo42()
        {
            return 42;
        }
    }
}
```

Build the project. SqliteDna will use DNNE mode and produce 4 files comprising your extension: [ProjectName]NE.dll (native wrapper), [ProjectName].dll (main .NET dll), [ProjectName].runtimeconfig.json, SqliteDna.Integration.dll.

You can now load the extension in SQLite and call the functions:

![](Doc/minimal-extension.png)

To use the AOT mode, add PublishAOT and RuntimeIdentifier properties to the project (or use Samples\MinimalAOT\MinimalAOT.csproj):

```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net7.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>

		<PublishAOT>true</PublishAOT>
		<RuntimeIdentifier>win-x64</RuntimeIdentifier>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="SqliteDna" Version="*-*" />
	</ItemGroup>

</Project>
```

Publishing the AOT project will produce the single [ProjectName].dll native extension that you can load in SQLite:

![](Doc/minimal-aot-extension.png)

### Features

You can use int, long, double, string, string?, DateTime, byte[], byte[]? types for your function parameters and return value. They will be automatically converted to corresponding SQLite types.

You can throw an exception from your function and it will be converted to an SQLite error.

### Virtual tables

You can write a function generating table data returning IEnumerable. The following function generates a table with a single column Value and two rows: 

```csharp
[SqliteTableFunction]
public static IEnumerable<string> MyStringTable()
{
    List<string> result = new List<string> { "str1", "str2" };
    return result;
}
```

Then you can invoke it from SQLite:

```sql
CREATE VIRTUAL TABLE StringTable USING MyStringTable
```

Returning a custom type from SqliteTableFunction allows you to create a table with a column for each public property of the type:

```csharp
public record CustomRecord(string Name, int Id);

[SqliteTableFunction]
public static IEnumerable<CustomRecord> MyRecordTable()
{
    List<CustomRecord> result = new List<CustomRecord> { new CustomRecord("n42", 420), new CustomRecord("n50", 5) };
    return result;
}
```

![](Doc/virtual-table.png)

You can accept parameters in SqliteTableFunction:

```csharp
[SqliteTableFunction]
public static IEnumerable<CustomRecord> MyRecordParamsTable(string name, int id)
{
    List<CustomRecord> result = new List<CustomRecord> { new CustomRecord(name, id) };
    return result;
}
```

And provide parameters from SQLite:

```sql
CREATE VIRTUAL TABLE RecordParamsTable USING MyRecordParamsTable("Hello, world!", 100)
```

### Related projects

* https://observablehq.com/@asg017/introducing-sqlite-loadable-rs
* https://github.com/nalgeon/sqlean
* https://github.com/x2bool/xlite
* https://sqlsharp.com/features/
* https://github.com/tcdi/pgx - Build PostgreSQL extensions with Rust
* https://learn.microsoft.com/en-us/azure/postgresql/single-server/concepts-extensions - List of PostgreSQL extensions available in Azure
* https://observablehq.com/@asg017/introducing-sqlite-vss
