using SqliteDna.Testing;

namespace ExtensionTest;

public class MyFunctionsTest
{
    [Theory, MemberData(nameof(ConnectionData))]
    public void Functions(string extensionFile, SqliteProvider provider)
    {
        using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
        {
        }
    }

    public static IEnumerable<object[]> ConnectionData => 
        SqliteConnection.GenerateConnectionParameters(new string[] { "MyExtensionFileName" });
}

