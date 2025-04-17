using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityToolkit.Mvvm.ComponentModel;
using InvoiceApp.Services.Money;
using InvoiceApp.Services.Currencies;

namespace InvoiceApp.Database;

public class Item : ObservableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    private decimal Price { get; set; }
    private string CurrencyName { get; set; }
    public string Unit { get; set; }

    [NotMapped]
    public Currency Currency
    {
        get
        {
            return CurrencyFactory.fromName(CurrencyName);
        }
        private set
        {
            CurrencyName = value.Name;
        }
    }

    [NotMapped]
    public MoneyBase UnitPrice
    {
        get
        {
            return CurrencyFactory.TryConvertGeneric(Currency, Price);
        }
        set
        {
            Currency = value.CurrencyObj;
            Price = value.Amount;
        }
    }

    public class Mapper : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(i => i.CurrencyName).HasColumnName("CurrencyName").IsRequired();
            builder.Property(x => x.Unit).IsRequired();
            builder.Ignore(x => x.Currency);
        }
    }
}
