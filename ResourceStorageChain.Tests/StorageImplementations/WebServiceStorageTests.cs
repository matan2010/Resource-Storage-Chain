using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ResourceStorageChain.StorageImplementations;
using Xunit;

namespace ResourceStorageChain.Tests.StorageImplementations
{
    public class WebServiceStorageTests
    {
        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseContent;
            public MockHttpMessageHandler(string responseContent)
            {
                _responseContent = responseContent;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseContent)
                };
                return Task.FromResult(response);
            }
        }

        [Fact]
        public async Task Get_ShouldParseValidResponse()
        {
            string mockJson = "\"Hello World\"";
            var handler = new MockHttpMessageHandler(mockJson);
            var httpClient = new HttpClient(handler);

            var storage = new WebServiceStorage<string>(
                "http://fake.url",
                stream => JsonSerializer.Deserialize<string>(stream)
            );

            typeof(WebServiceStorage<string>)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(storage, httpClient);

            var result = await storage.Get();

            Assert.Equal("Hello World", result);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_OnException()
        {
            var faultyHandler = new MockHttpMessageHandlerThrowing();
            var httpClient = new HttpClient(faultyHandler);
            var storage = new WebServiceStorage<string>("http://bad.url", _ => null);
            typeof(WebServiceStorage<string>)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(storage, httpClient);

            var result = await storage.Get();

            Assert.Null(result);
        }

        [Fact]
        public void Set_ShouldThrowNotSupportedException()
        {
            var storage = new WebServiceStorage<string>("http://fake.url", _ => "parsed");

            Assert.ThrowsAsync<NotSupportedException>(() => storage.Set("data"));
        }

        private class MockHttpMessageHandlerThrowing : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Mock failure");
            }
        }
    }
}
