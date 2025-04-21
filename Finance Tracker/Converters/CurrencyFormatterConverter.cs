using System;
using System.Globalization;
using System.Windows.Data;

namespace FinanceTracker.Converters
{
    public class CurrencyFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double amount)
                return amount.ToString("C2"); 
            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}