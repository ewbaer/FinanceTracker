using System;
using System.IO;
using System.Text;
using System.Windows;
using FinanceTracker.ViewModels;
using Microsoft.Win32;

namespace FinanceTracker.Views
{
    public partial class ExportOptionsWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public ExportOptionsWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "Transactions",
                DefaultExt = ".csv",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var csv = new StringBuilder();
                csv.AppendLine("Id,Title,Amount,Category,Date,Time");

                foreach (var t in _viewModel.Transactions)
                {
                    string date = t.Date.ToString("MM/dd/yyyy");
                    string time = t.Time.ToString("hh:mm tt");
                    csv.AppendLine($"{t.Id},{Escape(t.Title)},{t.Amount},{t.Category},{date},{time}");
                }

                File.WriteAllText(dialog.FileName, csv.ToString());
                MessageBox.Show("Export successful!", "CSV Export", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            var prompt = new EmailPromptWindow(_viewModel);
            prompt.Owner = this;
            prompt.ShowDialog();
        }

        private string Escape(string input)
        {
            if (input.Contains(",") || input.Contains("\""))
                return $"\"{input.Replace("\"", "\"\"")}\"";
            return input;
        }
    }
}

