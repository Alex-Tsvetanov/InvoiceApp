using System;

namespace InvoiceApp.Model;

public class Price
{
    decimal Price { get; set; }
    Type Currency {get;}
}

public interface IPrice<T> where T : ICurrency, class
{
    decimal Price { get; set; }
    static Type Currency { get => typeof(T); }
}
