using PersonalFinanceTracker.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinanceTracker.Models
{
    /// <summary>
    /// Класс, представляющий кошелек пользователя
    /// Управляет транзакциями и вычисляет балансы
    /// </summary>
    public class Wallet
    {
        public int Id { get; set; } // Уникальный идентификатор
        public required string Name { get; set; } // Название кошелька
        public string Currency { get; set; } // Валюта (RUB, USD, EUR)
        public decimal InitialBalance { get; set; } // Начальный баланс
        public List<Transaction> Transactions { get; set; } = new List<Transaction>(); // Список всех транзакций

        /// <summary>
        /// Текущий баланс кошелька (вычисляемое свойство)
        /// Начальный баланс + все доходы - все расходы
        /// </summary>
        public decimal CurrentBalance
        {
            get
            {
                var totalIncome = Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                var totalExpense = Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                return InitialBalance + totalIncome - totalExpense;
            }
        }

        /// <summary>
        /// Добавляет транзакцию с проверкой достаточности средств
        /// </summary>
        /// <returns>True если транзакция успешно добавлена, false если недостаточно средств</returns>
        public bool AddTransaction(Transaction transaction)
        {
            // Проверяем, что расход не превышает текущий баланс
            if (transaction.Type == TransactionType.Expense && transaction.Amount > CurrentBalance)
            {
                return false; // Недостаточно средств
            }

            Transactions.Add(transaction);
            return true;
        }

        /// <summary>
        /// Возвращает сумму доходов и расходов за указанный месяц
        /// </summary>
        public (decimal MonthlyIncome, decimal MonthlyExpense) GetMonthlySummary(int year, int month)
        {
            var monthlyTransactions = Transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();

            var income = monthlyTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = monthlyTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            return (income, expense);
        }




    }
}