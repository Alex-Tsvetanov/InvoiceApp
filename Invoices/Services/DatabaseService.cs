using System;
using InvoiceApp.Model;

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
        _context.Items.Include(i => i.Currency).Load();

        // Load customers
        _context.Customers.Load();

        // Load currencies
        _context.Currencies.Load();
    }

    ICollection<Item> Items
    {
        get
        {
            return _context.Items.Local.Select(x => new Item()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = new PriceAmount()
                {
                    Amount = x.Price,
                    PriceCurrency = x.Currency
                },
                Unit = x.Unit,
            }).ToList();
        }
    }
}
