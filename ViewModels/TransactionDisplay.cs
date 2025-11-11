using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// Модель представления для отображения транзакций в UI
    /// Расширяет Transaction дополнительными свойствами для интерфейса
    /// </summary>
    public class TransactionDisplay : Transaction
    {
        public string WalletName { get; set; }

        /// <summary>
        /// Создает модель представления на основе существующей транзакции
        /// </summary>
        public TransactionDisplay(Transaction transaction)
            : base(transaction.Id, transaction.Date, transaction.Amount,
                  transaction.Type, transaction.Description, transaction.WalletId)
        {
            // Наследуем все свойства базовой транзакции
        }
    }
}