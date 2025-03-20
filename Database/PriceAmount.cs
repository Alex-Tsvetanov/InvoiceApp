using CommunityToolkit.Mvvm.ComponentModel;

namespace InvoiceApp.Database;

public class PriceAmount : ObservableObject
{
    public PriceAmount(decimal price, Currency currency) {
        Amount = price;
        PriceCurrency = currency;
    }

    public decimal Amount { get; set; }
    public Currency PriceCurrency { get; set; }

    public static PriceAmount operator+ (PriceAmount amount1, PriceAmount amount2)
    {
        if (amount1 == null || amount2 == null || amount1.PriceCurrency != amount2.PriceCurrency)
        {
            throw new InvalidOperationException("Cannot sum up prices of different currencies");
        }
        return new PriceAmount (amount1.Amount + amount2.Amount, amount1.PriceCurrency);
    }

    public static PriceAmount operator* (PriceAmount amount1, decimal mul)
    {
        if (amount1 == null)
        {
            throw new InvalidOperationException("Cannot sum up prices of different currencies");
        }
        return new PriceAmount(amount1.Amount * mul, amount1.PriceCurrency);
    }
}

public interface Price
{
    decimal Amount { get; set;}
}