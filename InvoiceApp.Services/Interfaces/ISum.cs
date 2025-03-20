using System;

namespace InvoiceApp.Services.Interfaces;

public interface ICurrency
{
    static abstract string Code { get; }
    static abstract string Name { get; }
}

public class USD : ICurrency
{
    static string ICurrency.Name { get; } = "US Dollar";
    static string ICurrency.Code { get; } = "USD";
}

public class EUR : ICurrency
{
    static string ICurrency.Name { get; } = "Euro";
    static string ICurrency.Code { get; } = "EUR";
}

public class BGN : ICurrency
{
    static string ICurrency.Name { get; } = "Bulgarian Lev";
    static string ICurrency.Code { get; } = "BGN";
}

public class Cost<T> where T : class, ICurrency
{
    public decimal Amount { get; set; }

    public Cost(decimal amount)
    {
        Amount = amount;
    }
}

public class Sum
{
    public decimal Amount { get; set; }
    public Type Currency { get; set; }

    public dynamic ToCost()
    {
        if (Currency == typeof(USD))
        {
            Currency.GetConstructor([typeof(decimal)]).Invoke([Amount]);
            return new Cost<USD>(Amount);
        }
        else if (Currency == typeof(EUR))
        {
            return new Cost<EUR>(Amount);
        }
        else
        {
            throw new InvalidCastException("Currency not found.");
        }
    }
}
