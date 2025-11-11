// |===============================================|
// |ТЕСТЫ ДЛЯ СЕРВИСА FINANCESERVICE               |
// |===============================================|
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Models.Enums;
using PersonalFinanceTracker.Services;

public class FinanceServiceTests
{
    // ТЕСТ: Проверка корректной группировки транзакций по типу за месяц
    [Fact]
    public void GetMonthlyTransactionsGrouped_GroupsByTypeCorrectly()
    {
        // Arrange
        var service = new FinanceService { Wallets = new List<Wallet>() };
        var wallet = new Wallet { Id = 1, Name = "Test", Transactions = new List<Transaction>() };

        var incomeTransaction = new Transaction(1, new DateTime(2026, 1, 15), 100, TransactionType.Income, "Income", 1);
        var expenseTransaction = new Transaction(2, new DateTime(2026, 1, 20), 50, TransactionType.Expense, "Expense", 1);

        wallet.Transactions.Add(incomeTransaction);
        wallet.Transactions.Add(expenseTransaction);
        service.Wallets.Add(wallet);

        // Act
        var result = service.GetMonthlyTransactionsGrouped(2026, 1);

        // Assert
        Assert.Equal(2, result.Count); // Должно быть 2 группы: Income и Expense
                                       // Проверяем, что есть обе группы
        Assert.Contains(TransactionType.Income, result.Select(g => g.Key));
        Assert.Contains(TransactionType.Expense, result.Select(g => g.Key));

        // Убеждаемся, что в каждой группе правильное количество элементов
        Assert.Single(result.First(g => g.Key == TransactionType.Income));
        Assert.Single(result.First(g => g.Key == TransactionType.Expense));
    }

    // ТЕСТ: Проверка выборки Топ-3 расходов по кошельку
    [Fact]
    public void GetTop3ExpensesPerWallet_ReturnsCorrectNumberOfExpenses()
    {
        // Arrange
        var service = new FinanceService { Wallets = new List<Wallet>() };
        var wallet = new Wallet { Id = 1, Name = "Test", Transactions = new List<Transaction>() };

        // Добавляем 5 трат (расходы - TransactionType.Expense)
        for (int i = 1; i <= 5; i++)
        {
            // Самая большая трата: i=5, Amount=50
            wallet.Transactions.Add(new Transaction(i, new DateTime(2026, 1, i), i * 10, TransactionType.Expense, $"Expense {i}", 1));
        }
        wallet.Transactions.Add(new Transaction(6, new DateTime(2026, 1, 6), 100, TransactionType.Income, "Income, should be ignored", 1));

        service.Wallets.Add(wallet);

        // Act
        var result = service.GetTop3ExpensesPerWallet(2026, 1);

        // Assert
        Assert.Single(result);
        var walletExpenses = result["Test"];

        Assert.Equal(3, walletExpenses.Count);
        // Проверка сортировки по убыванию суммы
        Assert.Equal(50, walletExpenses[0].Amount);
        Assert.Equal(40, walletExpenses[1].Amount);
        Assert.Equal(30, walletExpenses[2].Amount);
    }

    // ТЕСТ: Проверка корректного расчета общей месячной сводки
    [Fact]
    public void GetTotalMonthlySummary_CalculatesCorrectTotals()
    {
        // Arrange
        var service = new FinanceService { Wallets = new List<Wallet>() };
        var wallet = new Wallet { Id = 1, Name = "Test", Transactions = new List<Transaction>() };

        // Транзакции за Январь
        wallet.Transactions.Add(new Transaction(1, new DateTime(2026, 1, 15), 200, TransactionType.Income, "Income Jan", 1));
        wallet.Transactions.Add(new Transaction(2, new DateTime(2026, 1, 20), 100, TransactionType.Expense, "Expense Jan", 1));
        // Транзакция за Февраль (не должна учитываться)
        wallet.Transactions.Add(new Transaction(3, new DateTime(2026, 2, 1), 50, TransactionType.Income, "Next month", 1));

        service.Wallets.Add(wallet);

        // Act
        var summary = service.GetTotalMonthlySummary(2026, 1); // Запрашиваем сводку за Январь

        // Assert
        Assert.Equal(200, summary.TotalIncome);
        Assert.Equal(100, summary.TotalExpense);
        // Добавлена проверка итогового баланса
        Assert.Equal(100, summary.TotalIncome - summary.TotalExpense);
    }
}
