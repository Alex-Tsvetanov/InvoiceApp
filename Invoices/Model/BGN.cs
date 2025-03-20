using System;

namespace InvoiceApp.Model;

public class BGN : ICurrency
{
    static String ICurrency.Name()
    {
        return "Bulgarian Lev";
    }
    static String ICurrency.Code()
    {
        return "BGN";
    }
}
