using System;
using System.Threading.Tasks;

namespace ResourceStorageChain.Interfaces
{
    public interface IStorage<T>
    {
        Task<T?> Get();
        Task Set(T value);
        bool CanWrite { get; }
        TimeSpan Expiration { get; }
        DateTime? LastUpdate { get; }
    }
}
