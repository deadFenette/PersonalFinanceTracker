using PersonalFinanceTracker.Models.Enums;
using System;

namespace PersonalFinanceTracker.Models
{
    /// <summary>
    /// Класс, представляющий финансовую транзакцию
    /// Содержит все данные об одной операции (доход/расход)
    /// </summary>
    public class Transaction
    {
        public int Id { get; set; }                   // Уникальный идентификатор
        public DateTime Date { get; set; }           // Дата транзакции
        public decimal Amount { get; set; }          // Сумма
        public TransactionType Type { get; set; }    // Тип (Income/Expense)
        public string Description { get; set; }      // Описание
        public int WalletId { get; set; }            // ID кошелька, к которому относится

        /// <summary>
        /// Конструктор для удобного создания транзакций
        /// </summary>
        public Transaction(int id, DateTime date, decimal amount, TransactionType type, string description, int walletId)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Type = type;
            Description = description;
            WalletId = walletId;
        }
    }
}