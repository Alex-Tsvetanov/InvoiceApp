using InvoiceApp.Services.Money;

namespace InvoiceApp.Services.Currencies
{
    public static class CurrencyFactory
    {
        static readonly Dictionary<Type, Func<decimal, object>> currencyFactories = new()
        {
            { typeof(BGN), p => new Money<BGN>(p) },
            { typeof(EUR), p => new Money<EUR>(p) },
            { typeof(USD), p => new Money<USD>(p) }
        };

        public static Currency fromName(string name)
        {
            foreach (var currency in currencyFactories.Keys)
            {
                Currency currency1 = (Currency)Activator.CreateInstance(currency);
                if (currency1.Name == name)
                {
                    return currency1;
                }
            }
            return null;
        }

        public static Money<T> TryConvert<T>(this IMoney money) where T : Currency, new()
        {
            if (money == null)
            {
                return new Money<T>(0);
            }
            else if (currencyFactories.TryGetValue(typeof(T), out var factory))
            {
                return (Money<T>)factory(money.Amount);
            }
            else
            {
                throw new ArgumentException("Unknown currency", nameof(money.CurrencyObj));
            }
        }

        public static Money<T> TryConvert<T>(T Currency, decimal Amount) where T : Currency, new()
        {
            if (currencyFactories.TryGetValue(typeof(T), out var factory))
            {
                return (Money<T>)factory(Amount);
            }
            else
            {
                return null;
            }
        }

        public static IMoney TryConvertGeneric(Currency Currency, decimal Amount)
        {
            if (Currency is BGN)
            {
                return new Money<BGN>(Amount);
            }
            else if (Currency is EUR)
            {
                return new Money<EUR>(Amount);
            }
            else if (Currency is USD)
            {
                return new Money<USD>(Amount);
            }
            else
            {
                return null;
            }
        }

    }
}
