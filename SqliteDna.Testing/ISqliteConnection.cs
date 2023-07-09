using System;

namespace SqliteDna.Testing
{
    public interface ISqliteConnection : IDisposable
    {
        T ExecuteScalar<T>(string commandText);
        int ExecuteNonQuery(string commandText);
        ISqliteDataReader ExecuteReader(string commandText);
    }
}
