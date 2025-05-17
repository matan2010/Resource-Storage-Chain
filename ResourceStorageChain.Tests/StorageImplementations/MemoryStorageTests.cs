using ResourceStorageChain.StorageImplementations;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ResourceStorageChain.Tests.StorageImplementations
{
    public class MemoryStorageTests
    {
        [Fact]
        public async Task Set_Then_Get_ShouldReturnStoredValue_WhenNotExpired()
        {
            var storage = new MemoryStorage<string>(TimeSpan.FromMinutes(5));

            await storage.Set("hello");
            var result = await storage.Get();

            Assert.Equal("hello", result);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenExpired()
        {
            var storage = new MemoryStorage<string>(TimeSpan.FromMilliseconds(1));

            await storage.Set("test");
            await Task.Delay(10);
            var result = await storage.Get();

            Assert.Null(result);
        }

        [Fact]
        public void CanWrite_ShouldBeTrue()
        {
            var storage = new MemoryStorage<int>(TimeSpan.FromMinutes(1));

            Assert.True(storage.CanWrite);
        }
    }
}