using InvoiceApp.Services.Currencies;

namespace InvoiceApp.Services.Money
{
    public abstract partial class IMoney
    {
        public decimal Amount { get; set; }
        public abstract Currency CurrencyObj { get; }
        public static IMoney operator *(IMoney money, decimal mul)
        {
            return money?.Multiply(mul);
        }
        protected abstract IMoney Multiply(decimal mul);
    }
}
