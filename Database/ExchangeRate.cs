using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using InvoiceApp.Services.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceApp.Database;

public class ExchangeRate : ObservableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    private string FromCurrencyName { get; set; }
    private string ToCurrencyName { get; set; }
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }

    [NotMapped]
    public Currency FromCurrency
    {
        get
        {
            if (!string.IsNullOrEmpty(FromCurrencyName))
            {
                return CurrencyFactory.fromName(FromCurrencyName);
            }
            return null;
        }
        set
        {
            FromCurrencyName = value.Name;
        }
    }

    [NotMapped]
    public Currency ToCurrency
    {
        get
        {
            if (!string.IsNullOrEmpty(ToCurrencyName))
            {
                return CurrencyFactory.fromName(ToCurrencyName);
            }
            return null;
        }
        set
        {
            ToCurrencyName = value.Name;
        }
    }

    public class Mapper : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FromCurrencyName).IsRequired();
            builder.Property(x => x.ToCurrencyName).IsRequired();
            builder.Property(x => x.Rate).IsRequired();
            builder.Property(x => x.Date).IsRequired();
        }
    }
}
