using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceStorageChain.Interfaces;

public class WebServiceStorage<T> : IStorage<T>
{
    private readonly string _url;
    private readonly Func<string, T?> _parser;
    
    public bool CanWrite => false;
    public TimeSpan Expiration => TimeSpan.Zero;
    public DateTime? LastUpdate { get; private set; }

    private HttpClient _httpClient = new HttpClient();

    public WebServiceStorage(string url, Func<string, T?> parser)
    {
        _url = url;
        _parser = parser;
    }

    public async Task<T?> Get()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(_url);
            var result = _parser(response);

            if (result != null)
                LastUpdate = DateTime.Now;

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while calling GetStringAsync: {ex.Message}");
            return default;
        }
    }

    public Task Set(T value)
    {
        throw new NotSupportedException("WebServiceStorage is read-only.");
    }
}