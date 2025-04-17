using InvoiceApp.Services.Currencies;

namespace InvoiceApp.Services.Money
{
    public abstract partial class MoneyBase
    {
        public decimal Amount { get; set; }
        public abstract Currency CurrencyObj { get; }
        public static MoneyBase operator *(MoneyBase money, decimal mul)
        {
            return money?.Multiply(mul);
        }
        protected abstract MoneyBase Multiply(decimal mul);
    }
}
