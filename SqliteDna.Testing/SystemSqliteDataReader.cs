using System.Data.SQLite;

namespace SqliteDna.Testing
{
    internal class SystemSqliteDataReader : ISqliteDataReader
    {
        private SQLiteDataReader dataReader;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dataReader.Dispose();
                }

                disposedValue = true;
            }
        }

        public SystemSqliteDataReader(SQLiteDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        public bool Read()
        {
            return dataReader.Read();
        }

        public T GetItem<T>(string name)
        {
            return (T)dataReader[name];
        }
    }
}
