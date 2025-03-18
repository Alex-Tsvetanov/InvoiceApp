using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace InvoiceApp.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InvoiceDB.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                        .HasOne(i => i.Currency)
                        .WithMany()
                        .HasForeignKey(i => i.CurrencyId);

            modelBuilder.Entity<ExchangeRate>()
                        .HasOne(i => i.FromCurrency)
                        .WithMany()
                        .HasForeignKey(i => i.FromCurrencyId);

            modelBuilder.Entity<ExchangeRate>()
                        .HasOne(i => i.ToCurrency)
                        .WithMany()
                        .HasForeignKey(i => i.ToCurrencyId);

            modelBuilder.Entity<Invoice>()
                        .HasOne(i => i.TotalAmountCurrency)
                        .WithMany()
                        .HasForeignKey(i => i.TotalAmountCurrencyId);

            modelBuilder.Entity<Invoice>()
                        .HasMany(i => i.InvoiceLines)
                        .WithOne()
                        .HasForeignKey(i => i.InvoiceId);

            modelBuilder.Entity<Invoice>()
                        .HasOne(i => i.Customer)
                        .WithMany()
                        .HasForeignKey(i => i.CustomerId);

            modelBuilder.Entity<Invoice>()
                        .HasOne(i => i.Customer)
                        .WithMany()
                        .HasForeignKey(i => i.CustomerId);

            modelBuilder.Entity<InvoiceLine>()
                        .HasOne(il => il.Item)
                        .WithMany()
                        .HasForeignKey(il => il.ItemId);

            modelBuilder.Entity<Item>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Currency>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExchangeRate>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Customer>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Invoice>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<InvoiceLine>().Property(e => e.Id).ValueGeneratedOnAdd();

            // Добавяме валути
            foreach (var data in new List<Currency> {
                new Currency { Id = 1, Code = "USD", Name = "US Dollar" },
                new Currency { Id = 2, Code = "EUR", Name = "Euro" },
                new Currency { Id = 3, Code = "BGN", Name = "Bulgarian Lev" }
            })
                modelBuilder.Entity<Currency>().HasData(data);

            // Добавяме валутни курсове (примерни)
            foreach (var data in new List<ExchangeRate> {
                new ExchangeRate { Id = 1, FromCurrencyId = 1, ToCurrencyId = 2, Rate = 0.85m, Date = DateTime.Now },
                new ExchangeRate { Id = 2, FromCurrencyId = 1, ToCurrencyId = 3, Rate = 1.80m, Date = DateTime.Now },
                new ExchangeRate { Id = 3, FromCurrencyId = 2, ToCurrencyId = 3, Rate = 1.06m, Date = DateTime.Now }
            })
                modelBuilder.Entity<ExchangeRate>().HasData(data);

            // Добавяме артикули
            foreach (var data in new List<Item> {
                new Item { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1200, CurrencyId = 1, Unit = "pcs" },
                new Item { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 25, CurrencyId = 2, Unit = "pcs" },
                new Item { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 80, CurrencyId = 3, Unit = "pcs" }
            })
                modelBuilder.Entity<Item>().HasData(data);

            // Добавяме клиенти

            foreach (var data in new List<Customer> {
                new Customer { Id = 1, Name = "Acme Corp", Address = "123 Main St", Bulstat = "123456789" },
                new Customer { Id = 2, Name = "Beta Inc", Address = "456 Oak Ave", Bulstat = "987654321" }
            }) 
                modelBuilder.Entity<Customer>().HasData(data);
        }
    }
}
