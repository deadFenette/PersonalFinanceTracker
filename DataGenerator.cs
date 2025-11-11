using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Models.Enums;
using System;
using System.Collections.Generic;

namespace PersonalFinanceTracker
{
    /// <summary>
    /// Генератор тестовых данных для демонстрации приложения
    /// В реальном приложении данные бы хранились в базе данных
    /// </summary>
    public static class DataGenerator
    {
        private static readonly Random random = new Random();

        // Массивы с тестовыми данными
        private static readonly string[] Currencies = { "RUB", "USD", "EUR" };
        private static readonly string[] IncomeDescriptions = { "Зарплата", "Фриланс", "Инвестиции", "Подарок", "Возврат долга", "Успешный трейдинг" };
        private static readonly string[] ExpenseDescriptions = { "Продукты", "Транспорт", "Развлечения", "Аренда", "Одежда", "Ресторан", "Такси", "Здоровье" };

        /// <summary>
        /// Создает тестовые кошельки с транзакциями
        /// </summary>
        public static List<Wallet> GenerateTestData()
        {
            var wallets = new List<Wallet>();

            // Создаем 3 тестовых кошелька
            for (int i = 1; i <= 3; i++)
            {
                var wallet = new Wallet
                {
                    Id = i,
                    Name = $"Кошелек {i}",
                    Currency = Currencies[random.Next(Currencies.Length)],
                    InitialBalance = random.Next(1000, 5000)
                };

                // Генерируем транзакции за последние 3 месяца
                var transactions = new List<Transaction>();
                int transactionId = 1;

                for (int j = 0; j < 20; j++)
                {
                    var date = DateTime.Now.AddDays(-random.Next(1, 90));
                    var type = random.Next(2) == 0 ? TransactionType.Income : TransactionType.Expense;
                    var amount = type == TransactionType.Income ?
                        random.Next(100, 2000) :
                        random.Next(10, 500);

                    transactions.Add(new Transaction(
                        id: transactionId++,
                        date: date,
                        amount: amount,
                        type: type,
                        description: type == TransactionType.Income ?
                            IncomeDescriptions[random.Next(IncomeDescriptions.Length)] :
                            ExpenseDescriptions[random.Next(ExpenseDescriptions.Length)],
                        walletId: wallet.Id
                    ));
                }

                wallet.Transactions = transactions;
                wallets.Add(wallet);
            }

            return wallets;
        }
    }
}