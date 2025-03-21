// See https://aka.ms/new-console-template for more information
using InvoiceApp.Services.Currencies;
using InvoiceApp.Services.Money;

Console.WriteLine("Hello, World!");

Money<BGN> money1 = new(100);
Money<BGN> money2 = new(100);
Money<EUR> money3 = new(100);

var sum1 = money1 + money2;
//var sum2 = money1 + money3;


Money<T> getMoney<T>(T currency, decimal amount) where T : class, Currency
{
    Console.WriteLine(currency.GetType().FullName);
    return new Money<T>(amount);
}

var money4 = getMoney((Currency)new BGN(), 100);

Money<Currency> money5 = getMoney((Currency)new BGN(), 100);

Console.WriteLine(money4.GetType().FullName);
Console.WriteLine(money5.GetType().FullName);