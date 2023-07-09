using System;

namespace SqliteDna.Testing
{
    public interface ISqliteConnection : IDisposable
    {
        T ExecuteCommand<T>(string commandText);
    }
}
