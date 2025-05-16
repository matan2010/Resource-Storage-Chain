using System;
using System.Text.Json;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("No data available.!!!!!!!!!!!!!!!!!!!!!!");
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
                    return JsonSerializer.Deserialize<ExchangeRateList>(json);
                }
                catch
                {
                    return null;
                }
            }
        );

        ////test
        ///

        ////test


        var chain = new ChainResource<ExchangeRateList>(new List<IStorage<ExchangeRateList>>
        {
            memoryStorage,
            fileStorage,
            webServiceStorage
        });

        var result = chain.GetValue();

        if (result != null)
        {
            //Console.WriteLine($"Base: {result.Base}");
            //Console.WriteLine($"Timestamp: {result.Timestamp}");
        }
        else
        {
            Console.WriteLine("No data available.");
        }
    }

}
