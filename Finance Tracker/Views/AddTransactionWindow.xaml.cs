using System;
using System.Windows;
using FinanceTracker.Models;
using FinanceTracker.ViewModels;

namespace FinanceTracker.Views
{
    public partial class AddTransactionWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly bool _isExpense;
        private readonly bool _isEditing;
        private readonly Transaction _originalTransaction;

        public AddTransactionWindow(MainViewModel viewModel, bool isExpense)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _isExpense = isExpense;
            _isEditing = false;
            Title = isExpense ? "Add Expense" : "Add Income";
            DatePicker.SelectedDate = DateTime.Today;
        }

        public AddTransactionWindow(MainViewModel viewModel, Transaction transactionToEdit)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _isEditing = true;
            _originalTransaction = transactionToEdit;
            _isExpense = transactionToEdit.Amount < 0;

            Title = _isExpense ? "Edit Expense" : "Edit Income";

            TitleBox.Text = transactionToEdit.Title;
            AmountBox.Text = Math.Abs(transactionToEdit.Amount).ToString();
            DatePicker.SelectedDate = transactionToEdit.Date;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Please enter a title.");
                return;
            }

            if (!double.TryParse(AmountBox.Text, out double amount))
            {
                MessageBox.Show("Please enter a valid amount.");
                return;
            }

            if (_isExpense && amount > 0)
                amount *= -1;

            if (_isEditing)
            {
                _originalTransaction.Title = TitleBox.Text;
                _originalTransaction.Amount = amount;
                _originalTransaction.Date = DatePicker.SelectedDate ?? DateTime.Today;
                _originalTransaction.Time = DateTime.Now; 

                _viewModel.Save();
                _viewModel.SortTransactions(); 
            }
            else
            {
                var now = DateTime.Now;

                var newTransaction = new Transaction
                {
                    Id = _viewModel.Transactions.Count + 1,
                    Title = TitleBox.Text,
                    Amount = amount,
                    Category = _isExpense ? "Expense" : "Income",
                    Date = DatePicker.SelectedDate ?? now,
                    Time = now
                };

                _viewModel.AddTransaction(newTransaction);
            }

            Close();
        }
    }
}
