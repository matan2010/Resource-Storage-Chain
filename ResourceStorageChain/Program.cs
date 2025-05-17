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
        try
        {
            ExchangeRateServiceInitializer.Initialize();

            var chain = ChainResource<ExchangeRateList>.Instance;
            var result = await chain.GetValue();

            if (result == null)
            {
                Console.WriteLine("No data available.");
            }
            else
            {
                Console.WriteLine("Exchange rate data loaded successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during initialization or data retrieval: {ex.Message}");
        }

    }

}
