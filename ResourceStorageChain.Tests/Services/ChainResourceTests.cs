using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ResourceStorageChain.Interfaces;
using ResourceStorageChain.Services;
using Moq;

namespace ResourceStorageChain.Tests.Services
{
    public class ChainResourceTests
    {
        public ChainResourceTests()
        {
            typeof(ChainResource<string>)
                .GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(null, null);
        }

        [Fact]
        public void Instance_WithoutInitialization_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = ChainResource<string>.Instance;
            });
        }

        [Fact]
        public async Task GetValue_ReturnsValueFromFirstAvailableStorage()
        {
            var mock1 = new Mock<IStorage<string>>();
            var mock2 = new Mock<IStorage<string>>();

            mock1.Setup(x => x.Get()).ReturnsAsync((string?)null);
            mock2.Setup(x => x.Get()).ReturnsAsync("FromStorage2");

            mock1.Setup(x => x.CanWrite).Returns(true);
            mock1.Setup(x => x.Set(It.IsAny<string>())).Returns(Task.CompletedTask);

            ChainResource<string>.Initialize(new[] { mock1.Object, mock2.Object });

            var result = await ChainResource<string>.Instance.GetValue();

            Assert.Equal("FromStorage2", result);
            mock1.Verify(x => x.Set("FromStorage2"), Times.Once);
        }

        [Fact]
        public async Task GetValue_ReturnsNullIfAllStoragesEmpty()
        {
            var mock1 = new Mock<IStorage<string>>();
            var mock2 = new Mock<IStorage<string>>();

            mock1.Setup(x => x.Get()).ReturnsAsync((string?)null);
            mock2.Setup(x => x.Get()).ReturnsAsync((string?)null);

            ChainResource<string>.Initialize(new[] { mock1.Object, mock2.Object });

            var result = await ChainResource<string>.Instance.GetValue();

            Assert.Null(result);
        }
    }
}