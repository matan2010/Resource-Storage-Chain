using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceStorageChain.StorageImplementations;
using Xunit;

namespace ResourceStorageChain.Tests.StorageImplementations
{
    public class FileSystemStorageTests
    {
        [Fact]
        public async Task SetAndGet_ShouldReturnSameValue_IfNotExpired()
        {
            string filePath = Path.GetTempFileName();
            var storage = new FileSystemStorage<string>(filePath, TimeSpan.FromMinutes(5));
            string expected = "Hello";

            await storage.Set(expected);
            var actual = await storage.Get();

            Assert.Equal(expected, actual);

            File.Delete(filePath);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_IfFileDoesNotExist()
        {
            string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
            var storage = new FileSystemStorage<string>(filePath, TimeSpan.FromMinutes(5));

            var result = await storage.Get();

            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_IfDataExpired()
        {
            string filePath = Path.GetTempFileName();
            var wrapper = new
            {
                Value = "Outdated",
                Timestamp = DateTime.Now - TimeSpan.FromMinutes(10)
            };
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(wrapper));

            var storage = new FileSystemStorage<string>(filePath, TimeSpan.FromMinutes(5));

            var result = await storage.Get();

            Assert.Null(result);

            File.Delete(filePath);
        }

        [Fact]
        public async Task Set_ShouldUpdateLastUpdate()
        {
            string filePath = Path.GetTempFileName();
            var storage = new FileSystemStorage<string>(filePath, TimeSpan.FromMinutes(5));

            await storage.Set("Test");
            var timestamp = storage.LastUpdate;

            Assert.NotNull(timestamp);
            Assert.True((DateTime.Now - timestamp.Value).TotalSeconds < 2);

            File.Delete(filePath);
        }
    }
}
