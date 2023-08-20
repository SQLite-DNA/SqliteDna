using SqliteDna.Integration;

namespace FileIO
{
    public static class MyFunctions
    {
        [SqliteTableFunction]
        public static IEnumerable<string> ReadAllLines(string file)
        {
            return File.ReadAllLines(file);
        }

        [SqliteTableFunction]
        public static IEnumerable<FileInfo> ListFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern).Select(i => new FileInfo(i));
        }
    }
}
