namespace PersonalFinanceTracker.Models.Enums
{
    /// <summary>
    /// Тип транзакции - доход или расход
    /// Enum помогает избежать "магических строк"/чисел в коде
    /// </summary>
    public enum TransactionType
    {
        Income,   // Доход
        Expense   // Расход
    }
}