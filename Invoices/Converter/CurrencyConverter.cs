using InvoiceApp.Database;
using InvoiceApp.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace InvoiceApp.Converter
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "N/A";
            }

            if (parameter is null)
            {
                parameter = 3;
            }

            return ((decimal)value).ToString("N" + parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
