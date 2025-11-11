using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// ViewModel для отображения месячной статистики
    /// </summary>
    public class MonthlySummaryViewModel : INotifyPropertyChanged
    {
        private decimal _totalIncome;
        private decimal _totalExpense;
        private decimal _totalBalance;

        public decimal TotalIncome
        {
            get => _totalIncome;
            set
            {
                _totalIncome = value;
                OnPropertyChanged();
                // Пересчитываем баланс при изменении дохода
                TotalBalance = _totalIncome - _totalExpense;
            }
        }

        public decimal TotalExpense
        {
            get => _totalExpense;
            set
            {
                _totalExpense = value;
                OnPropertyChanged();
                // Пересчитываем баланс при изменении расхода
                TotalBalance = _totalIncome - _totalExpense;
            }
        }

        public decimal TotalBalance
        {
            get => _totalBalance;
            private set
            {
                _totalBalance = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}