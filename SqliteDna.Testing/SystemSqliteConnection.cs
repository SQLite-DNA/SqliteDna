using System.Data.SQLite;

namespace SqliteDna.Testing
{
    internal class SystemSqliteConnection : ISqliteConnection
    {
        private SQLiteConnection connection;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }

        public SystemSqliteConnection(string connectionString, string extensionPath)
        {
            connection = new SQLiteConnection(connectionString);
            try
            {
                connection.Open();
                connection.LoadExtension(extensionPath);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        public T ExecuteCommand<T>(string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return (T)command.ExecuteScalar();
        }
    }
}
