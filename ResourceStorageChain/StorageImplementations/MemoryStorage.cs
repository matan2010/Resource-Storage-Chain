using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceStorageChain.Interfaces;

public class MemoryStorage<T> : IStorage<T>
{
    private T? _value;
    public DateTime? LastUpdate { get; private set; }
    public bool CanWrite => true;
    public TimeSpan Expiration { get; }
    public MemoryStorage(TimeSpan expiration)
    {
        Expiration = expiration;
    }
    public Task<T?> Get()
    {
        if (_value != null && LastUpdate.HasValue)
        {
            var age = DateTime.Now - LastUpdate.Value;
            if (age < Expiration)
            {
                return Task.FromResult<T?>(_value);
            }
        }
        return Task.FromResult<T?>(default);
    }

    public Task Set(T value)
    {
        _value = value;
        LastUpdate = DateTime.Now;
        return Task.CompletedTask;
    }
}
