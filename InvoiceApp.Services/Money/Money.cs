using InvoiceApp.Services.Currencies;

namespace InvoiceApp.Services.Money
{
    public class Money<T> : MoneyBase where T : Currency, new()
    {
        public override Currency CurrencyObj => new T();

        public Money(decimal amount)
        {
            Amount = amount;
        }

        static public Money<T> operator +(Money<T> a, Money<T> b)
        {
            return new Money<T>(a.Amount + b.Amount);
        }
        static public Money<T> operator *(Money<T> a, decimal mul)
        {
            return new Money<T>(a.Amount * mul);
        }

        protected override MoneyBase Multiply(decimal mul)
        {
            return this * mul;
        }
    }
}
