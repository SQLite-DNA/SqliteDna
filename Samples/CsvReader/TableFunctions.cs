using SqliteDna.Integration;

namespace CsvReader
{
    public class TableFunctions
    {
        [SqliteTableFunction]
        public static DynamicTable csv(string filename, string schema)
        {
            var types = schema.Split(',').Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last()).ToArray();

            List<object[]> dataResult = new();
            foreach (string line in File.ReadAllLines(filename))
            {
                var values = line.Split(',').ToArray();

                var valuesArray = new object[values.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    switch (types[i])
                    {
                        case "integer":
                            valuesArray[i] = long.Parse(values[i]);
                            break;
                        case "real":
                            valuesArray[i] = double.Parse(values[i]);
                            break;
                        case "text":
                            valuesArray[i] = values[i].Replace("\"","");
                            break;
                    }
                }
                dataResult.Add(valuesArray);
            }

            return new(schema, dataResult);
        }
    }
}