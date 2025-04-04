using InvoiceApp.Services.Currencies;
using InvoiceApp.Services.Money;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }

        public ICollection<Currency> Currencies = new List<Currency> { new BGN(), new EUR(), new USD() };

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InvoiceDB.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<KeyValuePair<Type, Type>> entityTypes =
            [
                new KeyValuePair<Type, Type>(typeof(Item), typeof(Item.Mapper)),
                new KeyValuePair<Type, Type>(typeof(ExchangeRate), typeof(ExchangeRate.Mapper)),
                new KeyValuePair<Type, Type>(typeof(Invoice), typeof(Invoice.Mapper)),
                new KeyValuePair<Type, Type>(typeof(InvoiceLine), typeof(InvoiceLine.Mapper)),
                new KeyValuePair<Type, Type>(typeof(Customer), typeof(Customer.Mapper))
            ];
            foreach (var itemMapper in entityTypes)
            {
                var mapperInstance = Activator.CreateInstance(itemMapper.Value);
                var configureMethod = itemMapper.Value.GetMethod("Configure") ?? throw new Exception("");

                // Use the generic version of modelBuilder.Entity
                var entityTypeBuilderInstance = modelBuilder.Entity(itemMapper.Key);
                var genericEntityTypeBuilder = typeof(ModelBuilder).GetMethod("Entity", Type.EmptyTypes)
                    .MakeGenericMethod(itemMapper.Key).Invoke(modelBuilder, null);

                configureMethod.Invoke(mapperInstance, new object[] { genericEntityTypeBuilder });
            }
            
            // Добавяме валутни курсове (примерни)
            foreach (var data in new List<ExchangeRate> {
                new() { Id = 1, FromCurrency = new BGN(), ToCurrency = new EUR(), Rate = 0.51m, Date = DateTime.Now },
                new() { Id = 2, FromCurrency = new BGN(), ToCurrency = new USD(), Rate = 0.55m, Date = DateTime.Now },
                new() { Id = 3, FromCurrency = new EUR(), ToCurrency = new BGN(), Rate = 1.96m, Date = DateTime.Now },
                new() { Id = 4, FromCurrency = new EUR(), ToCurrency = new USD(), Rate = 1.08m, Date = DateTime.Now },
                new() { Id = 5, FromCurrency = new USD(), ToCurrency = new BGN(), Rate = 1.80m, Date = DateTime.Now },
                new() { Id = 6, FromCurrency = new USD(), ToCurrency = new EUR(), Rate = 0.92m, Date = DateTime.Now }
            })
                modelBuilder.Entity<ExchangeRate>().HasData(data);

            // Добавяме артикули
            foreach (var data in new List<Item> {
                new() { Id = 1, Name = "Laptop",   Description = "High-performance laptop", UnitPrice = new Money<BGN>(1200), Unit = "pcs" },
                new() { Id = 2, Name = "Mouse",    Description = "Wireless mouse"         , UnitPrice = new Money<EUR>(25)  , Unit = "pcs" },
                new() { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard"    , UnitPrice = new Money<USD>(80)  , Unit = "pcs" }
            })
                modelBuilder.Entity<Item>().HasData(data);

            // Добавяме клиенти

            foreach (var data in new List<Customer> {
                new() { Id = 1, Name = "Acme Corp", Address = "123 Main St", Bulstat = "123456789" },
                new() { Id = 2, Name = "Beta Inc", Address = "456 Oak Ave", Bulstat = "987654321" }
            }) 
                modelBuilder.Entity<Customer>().HasData(data);
        }
    }
}
