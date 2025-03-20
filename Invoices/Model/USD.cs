using System;

namespace InvoiceApp.Model;

public class USD : ICurrency
{
    static string ICurrency.Code()
    {
        return "USD";
    }
    static string ICurrency.Name()
    {
        return "US Dollar";
    }
}
