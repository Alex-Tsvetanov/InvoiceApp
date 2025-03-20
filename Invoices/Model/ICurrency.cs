using System;

namespace InvoiceApp.Model;

public interface ICurrency
{
    static abstract string Name();
    static abstract string Code();
}
