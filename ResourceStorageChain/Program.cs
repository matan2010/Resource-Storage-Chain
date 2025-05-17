using System;
using System.Text.Json;
using ResourceStorageChain.Models;
using ResourceStorageChain.Interfaces;
using ResourceStorageChain.Services;
using ResourceStorageChain.StorageImplementations;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "exchange_rates.json";
        var memoryStorage = new MemoryStorage<ExchangeRateList>(TimeSpan.FromHours(1));
        var fileStorage = new FileSystemStorage<ExchangeRateList>(
           filePath,
           TimeSpan.FromHours(4)
       );
        string apiKey = "d097f4e65b5d45ea925dbaa6021faa2a";
        string url = $"https://openexchangerates.org/api/latest.json?app_id={apiKey}";
        var webServiceStorage = new WebServiceStorage<ExchangeRateList>(
            url,
            json =>
            {
                try
                {
                    return JsonSerializer.Deserialize<ExchangeRateList>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch
                {
                    return null;
                }
            }
        );

        ChainResource<ExchangeRateList>.Initialize(new List<IStorage<ExchangeRateList>>
        {
            memoryStorage,
            fileStorage,
            webServiceStorage
        });
        var chain = ChainResource<ExchangeRateList>.Instance;

        var result = await chain.GetValue();
        if (result == null)
        {
            Console.WriteLine("No data available.");
        }
        
    }

}
