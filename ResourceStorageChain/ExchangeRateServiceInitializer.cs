using System;
using System.Collections.Generic;
using System.Text.Json;
using ResourceStorageChain.Models;
using ResourceStorageChain.Interfaces;
using ResourceStorageChain.Services;
using ResourceStorageChain.StorageImplementations;
using dotenv.net;

public static class ExchangeRateServiceInitializer
{
    public static void Initialize()
    {
        DotEnv.Load(new DotEnvOptions(
            envFilePaths: new[] { "./.env" }, 
            probeForEnv: true
        ));
        string filePath = "exchange_rates.json";

        var memoryStorage = new MemoryStorage<ExchangeRateList>(TimeSpan.FromHours(1));
        var fileStorage = new FileSystemStorage<ExchangeRateList>(filePath, TimeSpan.FromHours(4));

        string? apiKey = Environment.GetEnvironmentVariable("EXCHANGE_RATES_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Missing API key. Please set EXCHANGE_RATES_API_KEY in your environment variables or .env file.");
        }

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
            });

        ChainResource<ExchangeRateList>.Initialize(new List<IStorage<ExchangeRateList>>
        {
            memoryStorage,
            fileStorage,
            webServiceStorage
        });
    }
}
