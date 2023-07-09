namespace SqliteDna.Testing
{
    internal class MicrosoftSqliteDataReader : ISqliteDataReader
    {
        private Microsoft.Data.Sqlite.SqliteDataReader dataReader;
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

        public MicrosoftSqliteDataReader(Microsoft.Data.Sqlite.SqliteDataReader dataReader)
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
