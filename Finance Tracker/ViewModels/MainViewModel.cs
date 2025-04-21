using FinanceTracker.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using LiveChartsCore.SkiaSharpView.Painting;

namespace FinanceTracker.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Transaction> Transactions { get; set; }

        private Transaction _selectedTransaction;
        public Transaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged();
            }
        }

        public double TotalIncome => Transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        public double TotalExpense => Transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
        public double Balance => TotalIncome - TotalExpense;

        public ISeries[] LineSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        private string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "transactions.json");

        public MainViewModel()
        {
            Transactions = LoadTransactions();
            SortTransactions();
            UpdateChart();
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            Save();
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpense));
            OnPropertyChanged(nameof(Balance));
            SortTransactions();
            UpdateChart();
        }

        public void SortTransactions()
        {
            var sorted = Transactions.OrderByDescending(t => t.Date).ThenByDescending(t => t.Time).ToList();
            Transactions.Clear();
            foreach (var t in sorted)
                Transactions.Add(t);
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Transactions, options);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving transactions: {ex.Message}");
            }
        }

        public ObservableCollection<Transaction> LoadTransactions()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    var loaded = JsonSerializer.Deserialize<List<Transaction>>(json);
                    return new ObservableCollection<Transaction>(loaded);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}");
            }
            return new ObservableCollection<Transaction>();
        }

        public void UpdateChart()
        {
            var balancePoints = new List<double>();
            var labels = new List<string>();
            double runningBalance = 0;

            var groupedByDate = Transactions
                .GroupBy(t => t.Date.Date)
                .OrderBy(g => g.Key);

            foreach (var group in groupedByDate)
            {
                double dailyTotal = group.Sum(t => t.Amount);
                runningBalance += dailyTotal;
                balancePoints.Add(runningBalance);
                labels.Add(group.Key.ToString("MM/dd"));
            }

            LineSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = balancePoints.ToArray(),
                    Name = "Balance",
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 3),
                    Fill = null,
                    GeometrySize = 10,
                    TooltipLabelFormatter = (chartPoint) => $"{labels[chartPoint.Context.Index]}\n${chartPoint.PrimaryValue:F2}"

                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels.ToArray(),
                    LabelsRotation = 0,
                    Name = "Date",
                    TextSize = 12
                }
            };

            YAxes = new Axis[]
            {
                new Axis { Name = "Balance ($)", TextSize = 14 }
            };

            OnPropertyChanged(nameof(LineSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
