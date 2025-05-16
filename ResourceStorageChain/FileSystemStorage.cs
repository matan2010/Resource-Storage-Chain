using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class FileSystemStorage<T> : IStorage<T>
{
    private readonly string _filePath;
    public TimeSpan Expiration { get; }
    public bool CanWrite => true;
    public DateTime? LastUpdate { get; private set; }

    public FileSystemStorage(string filePath, TimeSpan expiration)
    {
        _filePath = filePath;
        Expiration = expiration;
    }

    public async Task<T?> Get()
    {
        if (!File.Exists(_filePath))
            return default;

        try
        {
            using FileStream fs = File.OpenRead(_filePath);
            var wrapper = await JsonSerializer.DeserializeAsync<StorageWrapper<T>>(fs);
            if (wrapper == null)
                return default;

            LastUpdate = wrapper.Timestamp;

            if (LastUpdate + Expiration < DateTime.Now)
                return default;

            return wrapper.Value;
        }
        catch
        {
            return default;
        }
    }

    public async Task Set(T value)
    {
        var wrapper = new StorageWrapper<T>
        {
            Value = value,
            Timestamp = DateTime.Now
        };

        using FileStream fs = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(fs, wrapper);
        LastUpdate = wrapper.Timestamp;
    }

    private class StorageWrapper<TValue>
    {
        public TValue? Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}