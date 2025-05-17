using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceStorageChain.Interfaces;

namespace ResourceStorageChain.Services
{
    public class ChainResource<T> where T : class
    {
        private readonly List<IStorage<T>> _storages;
        private const int MaxCount = 1;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(MaxCount, MaxCount);
        private static ChainResource<T>? _instance;
        private static readonly object _lock = new();

        private ChainResource(IEnumerable<IStorage<T>> storages)
        {
            _storages = storages.ToList();
        }

        public static void Initialize(IEnumerable<IStorage<T>> storages)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ChainResource<T>(storages);
                    }
                }
            }
        }

        public static ChainResource<T> Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("ChainResource has not been initialized. Call Initialize() first.");
                return _instance;
            }
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
}