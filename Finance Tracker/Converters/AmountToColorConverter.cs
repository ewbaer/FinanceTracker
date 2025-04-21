using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FinanceTracker.Converters
{
    public class AmountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double amount)
            {
                return amount < 0 ? Brushes.Red : Brushes.Green;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}