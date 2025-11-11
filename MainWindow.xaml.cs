using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Models.Enums;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace PersonalFinanceTracker
{
    /// <summary>
    /// Главное окно приложения - связывает интерфейс с бизнес-логикой
    /// </summary>
    public partial class MainWindow : Window
    {
        private FinanceService financeService = new FinanceService();
        private int selectedYear;
        private int selectedMonth;
        private readonly FileDataService fileDataService = new FileDataService();
        private MonthlySummaryViewModel monthlySummaryVM = new MonthlySummaryViewModel();
        public MainWindow()
        {
            InitializeComponent();
            StatisticsPanel.DataContext = monthlySummaryVM;
            Loaded += MainWindow_Loaded;

        }

        /// <summary>
        /// Загрузка данных при запуске приложения
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeDateControls();

                // Пытаемся загрузить из файла
                financeService.Wallets = fileDataService.LoadData();

                // Если данных нет, спрашиваем пользователя
                if (!financeService.Wallets.Any())
                {
                    var result = MessageBox.Show(
                        "Сохраненных данных не найдено. Создать тестовые данные?",
                        "Нет данных",
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        financeService.Wallets = DataGenerator.GenerateTestData();
                    }
                }

                InitializeAddTransactionForm();

                RefreshData();
                StatusTextBlock.Text = "Приложение загружено";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Инициализация комбобоксов для выбора даты
        /// </summary>
        private void InitializeDateControls()
        {
            // Заполняем годы (текущий и предыдущий)
            var currentYear = DateTime.Now.Year;
            YearComboBox.Items.Add(currentYear - 1);
            YearComboBox.Items.Add(currentYear);
            YearComboBox.SelectedItem = currentYear;

            // Заполняем месяцы
            for (int month = 1; month <= 12; month++)
            {
                var date = new DateTime(2000, month, 1);
                MonthComboBox.Items.Add(new ComboBoxItem
                {
                    Content = date.ToString("MMMM"),
                    Tag = month
                });
            }

            // Выбираем текущий месяц
            MonthComboBox.SelectedIndex = DateTime.Now.Month - 1;
        }

        /// <summary>
        /// Обновление всех данных при изменении даты
        /// </summary>
        private void RefreshData()
        {
            // Обновляем данные кошельков
            WalletsDataGrid.ItemsSource = null;
            WalletsDataGrid.ItemsSource = financeService.Wallets;

            // Получаем сгруппированные транзакции
            var groupedTransactions = financeService.GetMonthlyTransactionsGrouped(selectedYear, selectedMonth);

            // Преобразуем для отображения в DataGrid
            var displayTransactions = new List<TransactionDisplay>();
            foreach (var group in groupedTransactions)
            {
                foreach (var transaction in group.OrderBy(t => t.Date))
                {
                    displayTransactions.Add(new TransactionDisplay(transaction)
                    {
                        WalletName = financeService.GetWalletName(transaction.WalletId)
                    });
                }

                WalletsDataGrid.ItemsSource = financeService.Wallets;

            }
            TransactionsDataGrid.ItemsSource = null;
            TransactionsDataGrid.ItemsSource = displayTransactions;

            // Получаем топ-3 траты
            var topExpenses = financeService.GetTop3ExpensesPerWallet(selectedYear, selectedMonth);
            var topExpensesDisplay = new List<TransactionDisplay>();

            foreach (var walletExpenses in topExpenses)
            {
                foreach (var expense in walletExpenses.Value)
                {
                    topExpensesDisplay.Add(new TransactionDisplay(expense)
                    {
                        WalletName = walletExpenses.Key
                    });
                }
            }

            TopExpensesDataGrid.ItemsSource = null;
            TopExpensesDataGrid.ItemsSource = topExpensesDisplay;

            // Обновляем статистику
            var monthlySummary = financeService.GetTotalMonthlySummary(selectedYear, selectedMonth);
            monthlySummaryVM.TotalIncome = monthlySummary.TotalIncome;
            monthlySummaryVM.TotalExpense = monthlySummary.TotalExpense;
        }

        /// <summary>
        /// Обработчик изменения месяца
        /// </summary>
        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthComboBox.SelectedItem is ComboBoxItem selectedMonthItem)
            {
                selectedMonth = (int)selectedMonthItem.Tag;
                if (YearComboBox.SelectedItem != null)
                {
                    selectedYear = (int)YearComboBox.SelectedItem;
                    RefreshData();
                }
            }
        }

        /// <summary>
        /// Обработчик изменения года
        /// </summary>
        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearComboBox.SelectedItem != null && MonthComboBox.SelectedItem != null)
            {
                selectedYear = (int)YearComboBox.SelectedItem;
                selectedMonth = (int)((ComboBoxItem)MonthComboBox.SelectedItem).Tag;
                RefreshData();
            }
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            fileDataService.SaveData(financeService.Wallets);
            StatusTextBlock.Text = "Данные сохранены!";

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            financeService.Wallets = fileDataService.LoadData();
            RefreshData();
            StatusTextBlock.Text = "Данные загружены!";
        }
        private void AddTestDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (financeService.Wallets.Count == 0)
            {
                financeService.Wallets = DataGenerator.GenerateTestData();
                RefreshData();
                StatusTextBlock.Text = "Тестовые данные добавлены!";
            }
        }


        private void InitializeAddTransactionForm()
        {
            // Заполняем комбобокс кошельками
            WalletComboBox.ItemsSource = financeService.Wallets;
            WalletComboBox.DisplayMemberPath = "Name"; // Показывать название кошелька

            // Выбираем первый кошелек по умолчанию (если есть)
            if (WalletComboBox.Items.Count > 0)
                WalletComboBox.SelectedIndex = 0;

            // Выбираем "Расход" по умолчанию
            TypeComboBox.SelectedIndex = 1;
        }




        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            // Объявляем переменные в начале метода
            Wallet selectedWallet = null;
            TransactionType transactionType;
            decimal amount = 0;
            string description = null;

            try
            {
                // 1. ПОЛУЧАЕМ ВЫБРАННЫЙ КОШЕЛЕК
                selectedWallet = WalletComboBox.SelectedItem as Wallet;
                if (selectedWallet == null)
                {
                    ErrorTextBlock.Text = "Ошибка: выберите кошелек";
                    return;
                }

                // 2. ПОЛУЧАЕМ ТИП ОПЕРАЦИИ
                ComboBoxItem selectedTypeItem = TypeComboBox.SelectedItem as ComboBoxItem;
                if (selectedTypeItem == null)
                {
                    ErrorTextBlock.Text = "Ошибка: выберите тип операции";
                    return;
                }

                // Преобразуем текст в TransactionType
                if (selectedTypeItem.Content.ToString() == "Доход")
                    transactionType = TransactionType.Income;
                else
                    transactionType = TransactionType.Expense;

                // 3. ПОЛУЧАЕМ СУММУ
                string amountText = AmountTextBox.Text;
                if (string.IsNullOrWhiteSpace(amountText))
                {
                    ErrorTextBlock.Text = "Ошибка: введите сумму";
                    return;
                }

                // Пробуем преобразовать текст в число
                if (!decimal.TryParse(amountText, out amount))
                {
                    ErrorTextBlock.Text = "Ошибка: сумма должна быть числом";
                    return;
                }

                if (amount <= 0)
                {
                    ErrorTextBlock.Text = "Ошибка: сумма должна быть больше 0";
                    return;
                }

                // 4. ПОЛУЧАЕМ ОПИСАНИЕ
                description = DescriptionTextBox.Text;
                if (string.IsNullOrWhiteSpace(description))
                {
                    ErrorTextBlock.Text = "Ошибка: введите описание";
                    return;
                }

                // 2. СОЗДАЕМ ТРАНЗАКЦИЮ

                // Находим максимальный ID для новой транзакции
                int newId = 1;
                foreach (var wallet in financeService.Wallets)
                {
                    foreach (var transaction in wallet.Transactions)
                    {
                        if (transaction.Id >= newId)
                            newId = transaction.Id + 1;
                    }
                }

                // Создаем новую транзакцию
                Transaction newTransaction = new Transaction(
                    id: newId,
                    date: DateTime.Now,
                    amount: amount,
                    type: transactionType,
                    description: description,
                    walletId: selectedWallet.Id
                );

                // 3. ДОБАВЛЯЕМ ТРАНЗАКЦИЮ В КОШЕЛЕК
                bool success = selectedWallet.AddTransaction(newTransaction);

                if (success)
                {
                    ErrorTextBlock.Text = "✅ Транзакция успешно добавлена!";

                    // Обновляем интерфейс
                    RefreshData();

                    // Очищаем поля
                    AmountTextBox.Text = "";
                    DescriptionTextBox.Text = "";
                }
                else
                {
                    ErrorTextBlock.Text = "❌ Ошибка: недостаточно средств на кошельке!";
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void UpdateDebugInfo(Wallet wallet, Transaction transaction)
        {
            string debugInfo = $"Добавлено в {wallet.Name}: {transaction.Amount} {transaction.Type} | ";
            debugInfo += $"Баланс: {wallet.CurrentBalance} | ";
            debugInfo += $"Транзакций: {wallet.Transactions.Count}";

            DebugTextBlock.Text = debugInfo;

            // Также выводим в консоль
            Console.WriteLine(debugInfo);

            // Выводим информацию по всем кошелькам
            Console.WriteLine("=== ВСЕ КОШЕЛЬКИ ===");
            foreach (var w in financeService.Wallets)
            {
                Console.WriteLine($"{w.Name}: {w.CurrentBalance} ({w.Transactions.Count} транзакций)");
            }

        }
    }
}


