using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceStorageChain.Interfaces;

public class ChainResource<T> where T : class
{
    private readonly List<IStorage<T>> _storages;
    private const int MaxCount = 1;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(MaxCount, MaxCount);

    public ChainResource(IEnumerable<IStorage<T>> storages)
    {
        _storages = storages.ToList();
    }

    public async Task<T?> GetValue()
    {
        await _semaphore.WaitAsync();
        try
        {
            for (int i = 0; i < _storages.Count; i++)
            {
                var storage = _storages[i];
                var value = await storage.Get();

                if (value != null)
                {
                    for (int j = 0; j < i; j++)
                    {
                        var s = _storages[j];
                        if (s.CanWrite)
                        {
                            await s.Set(value);
                        }
                    }

                    return value;
                }
            }

            return default;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}