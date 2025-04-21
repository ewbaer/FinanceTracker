using System;
using System.Windows;
using System.Windows.Controls;
using FinanceTracker.Models;
using FinanceTracker.ViewModels;

namespace FinanceTracker.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel vm => (MainViewModel)DataContext;

        public string LoggedInUser { get; set; }
        public MainWindow(string username)
        {
            InitializeComponent();
            LoggedInUser = username;
            UsernameText.Text = LoggedInUser;
        }

        private void AddIncome_Click(object sender, RoutedEventArgs e)
        {
            var form = new AddTransactionWindow(vm, isExpense: false);
            form.ShowDialog();
        }


        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTransactionWindow(vm, isExpense: true);
            addWindow.ShowDialog();
        }
        private void OpenExportOptions_Click(object sender, RoutedEventArgs e)
        {
            var exportWindow = new ExportOptionsWindow(vm);
            exportWindow.Owner = this;
            exportWindow.ShowDialog();
        }



        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedTransaction != null)
            {
                vm.Transactions.Remove(vm.SelectedTransaction);
                vm.Save();
                vm.OnPropertyChanged(nameof(vm.TotalIncome));
                vm.OnPropertyChanged(nameof(vm.TotalExpense));
                vm.OnPropertyChanged(nameof(vm.Balance));
                vm.SelectedTransaction = null;
            }
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedTransaction != null)
            {
                var editWindow = new AddTransactionWindow(vm, vm.SelectedTransaction);
                editWindow.ShowDialog();
                vm.OnPropertyChanged(nameof(vm.TotalIncome));
                vm.OnPropertyChanged(nameof(vm.TotalExpense));
                vm.OnPropertyChanged(nameof(vm.Balance));

            }
        }
        

    }
}