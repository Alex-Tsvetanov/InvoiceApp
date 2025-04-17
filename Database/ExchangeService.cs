using InvoiceApp.Services.Money;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceApp.Services.Currencies
{
    public class ExchangeService
    {
        public static MoneyBase Convert(MoneyBase money, Currency toCurrency, ICollection<Database.ExchangeRate> db)
        {
            if (money == null)
                return null;
            if (money.CurrencyObj.GetType() == toCurrency.GetType())
            {
                return money;
            }
            else
            {
                var moneyType = typeof(Money<>).MakeGenericType(toCurrency.GetType());
                var filter = db.Where(x => x.FromCurrency.GetType() == money.CurrencyObj.GetType() && x.ToCurrency.GetType() == toCurrency.GetType()).ToList();
                return (MoneyBase)moneyType.GetConstructor([typeof(decimal)])?.Invoke([filter.First()?.Rate * money.Amount]);
            }
        }
    }
}
