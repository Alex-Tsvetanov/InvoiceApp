using System;

namespace InvoiceApp.Model;

public class Price<T> : IPrice<T> where T : ICurrency, class
{
    static Price<T> operator+ (Price<T> a, Price<T> b)
    {

    }
}

Price<BGN> a = new InvoiceApp.Model.Price<InvoiceApp.Model.BGN>(5);
Price<USD> b = new InvoiceApp.Model.Price<InvoiceApp.Model.USD>(5);

new Price<EUR>(a + b);