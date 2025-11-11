using Xunit;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Tests
{
    public class SimpleTest
    {
        [Fact]
        public void CanCreateWallet()
        {
            // Arrange & Act
            var wallet = new Wallet { Name = "Test" };

            // Assert
            Assert.NotNull(wallet);
            Assert.Equal("Test", wallet.Name);
        }
    }
}