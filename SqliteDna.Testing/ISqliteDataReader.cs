using System;

namespace SqliteDna.Testing
{
    public interface ISqliteDataReader : IDisposable
    {
        bool Read();
        T GetItem<T>(string name);
    }
}
