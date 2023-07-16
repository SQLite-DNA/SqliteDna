namespace SqliteDna.Testing
{
    internal class SqliteCppDataReader : ISqliteDataReader
    {
        private System.IDisposable/*SqliteDna.SQLiteCppManaged.SQLiteDataReader*/ dataReader;
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

        public SqliteCppDataReader(System.IDisposable/*SQLiteDataReader*/ dataReader)
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
            ////return dataReader.Read();
            return (bool)dataReader.GetType().GetMethod("Read").Invoke(dataReader, null);
        }

        public T GetItem<T>(string name)
        {
            ////return (T)dataReader.GetItem(name);
            return (T)dataReader.GetType().GetMethod("GetItem").Invoke(dataReader, new object[] { name });
        }
    }
}
