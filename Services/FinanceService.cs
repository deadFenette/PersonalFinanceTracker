using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinanceTracker.Services
{
    /// <summary>
    /// Сервис для работы с финансовыми данными
    /// Содержит бизнес-логику приложения
    /// </summary>
    public class FinanceService
    {
        public List<Wallet> Wallets { get; set; } = new List<Wallet>();

        /// <summary>
        /// Возвращает все транзакции за указанный месяц, сгруппированные по типу
        /// </summary>
        public List<IGrouping<TransactionType, Transaction>> GetMonthlyTransactionsGrouped(int year, int month)
        {
            var allTransactions = Wallets
                .SelectMany(w => w.Transactions)
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();

            // Группируем по типу и сортируем по общей сумме (убывание)
            return allTransactions
                .GroupBy(t => t.Type)
                .OrderByDescending(g => g.Sum(t => t.Amount))
                .ToList();
        }

        /// <summary>
        /// Возвращает топ-3 самых больших трат для каждого кошелька за указанный месяц
        /// </summary>
        public Dictionary<string, List<Transaction>> GetTop3ExpensesPerWallet(int year, int month)
        {
            var result = new Dictionary<string, List<Transaction>>();

            foreach (var wallet in Wallets)
            {
                var topExpenses = wallet.Transactions
                    .Where(t => t.Type == TransactionType.Expense &&
                               t.Date.Year == year &&
                               t.Date.Month == month)
                    .OrderByDescending(t => t.Amount)  // Сначала сортируем по убыванию суммы
                    .Take(3)                           // Берем только 3 самых больших
                    .ToList();

                // Убедимся, что в каждой группе тоже отсортировано по убыванию
                result[wallet.Name] = topExpenses.OrderByDescending(t => t.Amount).ToList();
            }

            return result;
        }

        /// <summary>
        /// Возвращает название кошелька по его ID
        /// </summary>
        public string GetWalletName(int walletId)
        {
            return Wallets.FirstOrDefault(w => w.Id == walletId)?.Name ?? "Неизвестно";
        }

        public (decimal TotalIncome, decimal TotalExpense) GetTotalMonthlySummary(int year, int month)
        {
            var allTransactions = Wallets
                .SelectMany(w => w.Transactions)
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();

            var totalIncome = allTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var totalExpense = allTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            return (totalIncome, totalExpense);
        }

      public bool TryAddTransaction(int walletId, Transaction transaction)
        {
            var wallet = Wallets.FirstOrDefault(w => w.Id == walletId);
            if (wallet == null) return false;

            return wallet.AddTransaction(transaction);
        }

    }
}