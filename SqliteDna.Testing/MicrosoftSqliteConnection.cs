namespace SqliteDna.Testing
{
    internal class MicrosoftSqliteConnection : ISqliteConnection
    {
        private Microsoft.Data.Sqlite.SqliteConnection connection;
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

        public MicrosoftSqliteConnection(string connectionString, string extensionPath)
        {
            connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
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

        public T ExecuteScalar<T>(string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return (T)command.ExecuteScalar();
        }

        public int ExecuteNonQuery(string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteNonQuery();
        }
    }
}
