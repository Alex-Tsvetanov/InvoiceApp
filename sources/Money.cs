using InvoiceApp.Services.Currencies;

namespace InvoiceApp.Services.Money
{
    public class Money<T> : IMoney where T : Currency, new()
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

        protected override IMoney Multiply(decimal mul)
        {
            return this * mul;
        }
    }
}
