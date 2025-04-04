using InvoiceApp.Database;
using InvoiceApp.Services.Currencies;
using Microsoft.EntityFrameworkCore;
using System.IO.Packaging;
using System.IO;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows;
using InvoiceApp.Services.Money;

namespace InvoiceApp.Services;

public class DatabaseService
{
    private readonly ApplicationDbContext _context;
    private readonly decimal _bigQuantityDiscount;
    private readonly decimal _bigTotalPriceDiscount;
    private readonly decimal _taxCoeficient;

    public DatabaseService(ApplicationDbContext context, decimal bigQuantityDiscount, decimal bigTotalPriceDiscount, decimal taxCoeficient)
    {
        _context = context;
        _context.Database.EnsureCreated();
        LoadData();
        _bigQuantityDiscount = bigQuantityDiscount;
        _bigTotalPriceDiscount = bigTotalPriceDiscount;
        _taxCoeficient = taxCoeficient;
    }

    private void LoadData()
    {
        // Load items with their currencies
        _context.Items.Load();

        // Load customers
        _context.Customers.Load();
    }

    public ICollection<Item> Items => _context.Items.Local.ToList();

    public ICollection<InvoiceLine> InvoiceLines => _context.InvoiceLines.Local.ToList();

    public ICollection<Customer> Customers => _context.Customers.Local.ToList();

    public ICollection<Invoice> Invoices => _context.Invoices.Local.ToList();

    public ICollection<Currency> Currencies => _context.Currencies;

    public ICollection<ExchangeRate> ExchangeRates => _context.ExchangeRates.ToList();

    public async Task RegisterInvoice(ViewModelInvoice invoice)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        { 
            var dbInvoice = new Invoice
            {
                Number = invoice.InvoiceNumber,
                Date = invoice.InvoiceDate,
                Customer = invoice.Customer,
                InvoiceCurrency = invoice.Currency.GetType(),
                InvoiceLines = []
            };

            _context.Invoices.Add(dbInvoice);
            await _context.SaveChangesAsync();

            var invoiceLinesWithInvoiceId = invoice.InvoiceLines.Select(x => new InvoiceLine
            {
                InvoiceId = dbInvoice.Id,
                Item = x.InvoiceItem,
                Quantity = x.Quantity
            }).ToList();

            _context.InvoiceLines.AddRange(invoiceLinesWithInvoiceId);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public void GeneratePdf(string filename)
    {
        /*
        *  Convert WPF -> XPS -> PDF
        */
        MemoryStream lMemoryStream = new MemoryStream();
        Package package = Package.Open(lMemoryStream, FileMode.Create);
        XpsDocument doc = new XpsDocument(package);
        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

        writer.Write(Application.Current.MainWindow);

        doc.Close();
        package.Close();

        // Convert 
        MemoryStream outStream = new MemoryStream();
        PdfSharp.Xps.XpsConverter.Convert(lMemoryStream, outStream, false);

        // Write pdf file
        FileStream fileStream = new FileStream(filename + ".pdf", FileMode.Create);
        outStream.CopyTo(fileStream);

        // Clean up
        outStream.Flush();
        outStream.Close();
        fileStream.Flush();
        fileStream.Close();
    }

    internal IMoney getTotalPriceFor(IMoney unitPrice, decimal quantity)
    {
        var result = unitPrice * quantity;
        if (quantity > 10) result = result * (1 - _bigQuantityDiscount);
        return result;
    }
    internal IMoney getSumPriceFor(IMoney sumPrice)
    {
        var result = sumPrice;
        if (result.Amount > 10000) result = result * (1 - _bigTotalPriceDiscount);
        return result;
    }
    internal IMoney getPriceWithTax(IMoney price)
    {
        return price * (1 + _taxCoeficient);
    }
}
