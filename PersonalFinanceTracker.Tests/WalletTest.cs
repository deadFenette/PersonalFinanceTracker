using Xunit;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Models.Enums;

namespace PersonalFinanceTracker.Tests
{
    public class WalletTests
    {  
        //ТЕСТ: Баланс без транзакций равен начальному балансу
        [Fact]
        public void CurrentBalance_WithNoTransactions_ReturnsInitialBalance()
        {
            // Arrange
            var wallet = new Wallet { Name = "Тестовый Кошелек", InitialBalance = 1000 };

            // Act
            var balance = wallet.CurrentBalance;

            // Assert
            Assert.Equal(1000, balance);
        }
        //ТЕСТ: Доходная транзакция увеличивает баланс
        [Fact]
        public void CurrentBalance_WithIncomeTransaction_IncreasesBalance()
        {
            // Arrange
            var wallet = new Wallet { Name = "Тестовый Кошелек", InitialBalance = 1000 };
            var transaction = new Transaction(1, DateTime.Now, 500, TransactionType.Income, "Test", 1);

            // Act
            wallet.Transactions.Add(transaction);

            // Assert
            Assert.Equal(1500, wallet.CurrentBalance);
        }
        //ТЕСТ: Расходная транзакция уменьшает баланс
        [Fact]
        public void CurrentBalance_WithExpenseTransaction_DecreasesBalance()
        {
            // Arrange
            var wallet = new Wallet { Name = "Тестовый Кошелек", InitialBalance = 1000 };
            var transaction = new Transaction(1, DateTime.Now, 300, TransactionType.Expense, "Test", 1);

            // Act
            wallet.Transactions.Add(transaction);

            // Assert
            Assert.Equal(700, wallet.CurrentBalance);
        }
        //ТЕСТ: Не добавлять расход, превышающий баланс
        [Fact]
        public void AddTransaction_WithExpenseExceedingBalance_ReturnsFalse()
        {
            // Arrange
            var wallet = new Wallet { Name = "Тестовый Кошелек", InitialBalance = 100 };
            var transaction = new Transaction(1, DateTime.Now, 150, TransactionType.Expense, "Слишком много", 1);

            // Actа
            var result = wallet.AddTransaction(transaction);

            // Assert
            Assert.False(result);
            Assert.Empty(wallet.Transactions);

        }
        //ТЕСТ: Добавление валидного расхода
        [Fact]
        public void AddTransaction_WithValidExpense_ReturnsTrue()
        {
            // Arrangee
            var wallet = new Wallet { Name = "Тестовый Кошелек", InitialBalance = 200 };
            var transaction = new Transaction(1, DateTime.Now, 150, TransactionType.Expense, "Окей", 1);

            // Act
            var result = wallet.AddTransaction(transaction);

            // Assert
            Assert.True(result);
            Assert.Single(wallet.Transactions); // Должна быть одна добавленная транзакция
        
        }
    }
}