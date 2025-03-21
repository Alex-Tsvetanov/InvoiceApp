using InvoiceApp.Services.Currencies;
using System.Globalization;
using System.Windows.Data;

namespace InvoiceApp.Converter
{
    public class CurrencyPresenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine(value);
            if (value is null)
            {
                return "N/A";
            }

            Console.WriteLine(((Currency)value));
            Console.WriteLine(((Currency)value).Code);
            return ((Currency)value).Code;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
