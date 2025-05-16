using System;
using System.Threading.Tasks;

public interface IStorage<T>
{
    Task<T?> Get();
    Task Set(T value);
    bool CanWrite { get; }
    TimeSpan Expiration { get; }
    DateTime? LastUpdate { get; }
}
