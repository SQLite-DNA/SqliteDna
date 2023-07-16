// SqliteDna.Testing targets netstandard2.0 and can't directly reference Windows only SqliteDna.SQLiteCppManaged. Using reflection instead.

using System;

namespace SqliteDna.Testing
{
    internal class SqliteCppConnection : ISqliteConnection
    {
        private IDisposable/*SqliteDna.SQLiteCppManaged.SQLiteConnection*/ connection;
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

        public SqliteCppConnection(string connectionString, string extensionPath)
        {
            ////connection = new SQLiteCppManaged.SQLiteConnection(connectionString.Replace("Data Source=", ""), extensionPath);
            connection = (IDisposable)Activator.CreateInstance(Type.GetType("SqliteDna.SQLiteCppManaged.SQLiteConnection, SqliteDna.SQLiteCppManaged, Version=1.0.0, Culture=neutral, PublicKeyToken=null"), new object[] { connectionString.Replace("Data Source=", ""), extensionPath });
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        public T ExecuteScalar<T>(string commandText)
        {
            ////return (T)connection.ExecuteScalar(commandText);
            return (T)connection.GetType().GetMethod("ExecuteScalar").Invoke(connection, new object[] { commandText });
        }

        public int ExecuteNonQuery(string commandText)
        {
            ////return connection.ExecuteNonQuery(commandText);
            return (int)connection.GetType().GetMethod("ExecuteNonQuery").Invoke(connection, new object[] { commandText });
        }

        public ISqliteDataReader ExecuteReader(string commandText)
        {
            ////return new SqliteCppDataReader(connection.ExecuteReader(commandText));
            return new SqliteCppDataReader((IDisposable)connection.GetType().GetMethod("ExecuteReader").Invoke(connection, new object[] { commandText }));
        }
    }
}
