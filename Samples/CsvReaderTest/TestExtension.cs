using SqliteDna.Testing;

namespace CsvReaderTest
{
    public class TestExtension
    {
        [Theory, MemberData(nameof(ConnectionData))]
        public void Catalonia(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE Catalonia USING csv(crash_catalonia.csv, \"Day_of_Week text, Number_of_Crashes integer\")"));
                using (var reader = connection.ExecuteReader("SELECT * FROM Catalonia"))
                {
                    Assert.True(reader.Read());
                    Assert.Equal("Sunday", reader.GetItem<string>("Day_of_Week"));
                    Assert.Equal(13664, reader.GetItem<long>("Number_of_Crashes"));
                    Assert.True(reader.Read());
                    Assert.Equal("Monday", reader.GetItem<string>("Day_of_Week"));
                    Assert.Equal(17279, reader.GetItem<long>("Number_of_Crashes"));
                }
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        public void Freshman(string extensionFile, SqliteProvider provider)
        {
            using (var connection = SqliteConnection.Create("Data Source=:memory:", extensionFile, provider))
            {
                Assert.Equal(0, connection.ExecuteNonQuery("CREATE VIRTUAL TABLE Freshman USING csv(freshman_lbs.csv, \"Sex text, Weight_Sep integer, Weight_Apr integer, BMI_Sep real, BMI_Apr real\")"));
                using (var reader = connection.ExecuteReader("SELECT * FROM Freshman"))
                {
                    Assert.True(reader.Read());
                    Assert.Equal("M", reader.GetItem<string>("Sex"));
                    Assert.Equal(159, reader.GetItem<long>("Weight_Sep"));
                    Assert.Equal(22.02, reader.GetItem<double>("BMI_Sep"));
                    Assert.True(reader.Read());
                    Assert.Equal(190, reader.GetItem<long>("Weight_Apr"));
                    Assert.Equal(17.44, reader.GetItem<double>("BMI_Apr"));
                }
            }
        }

        public static IEnumerable<object[]> ConnectionData => SqliteConnection.GenerateConnectionParameters(new string[] { "CsvReader" });
    }
}