using System;
using Xunit;
using ResourceStorageChain.Services;

namespace ResourceStorageChain.Tests.Services
{
    public class ExchangeRateServiceInitializerTests
    {
        [Fact]
        public void Initialize_WithoutApiKey_ThrowsException()
        {
            Environment.SetEnvironmentVariable("EXCHANGE_RATES_API_KEY", null);

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                ExchangeRateServiceInitializer.Initialize();
            });

            Assert.Contains("Missing API key", ex.Message);
        }

        [Fact]
        public void Initialize_WithApiKey_DoesNotThrow()
        {
            Environment.SetEnvironmentVariable("EXCHANGE_RATES_API_KEY", "dummy");

            var ex = Record.Exception(() => ExchangeRateServiceInitializer.Initialize());

            Assert.Null(ex);
        }
    }
}