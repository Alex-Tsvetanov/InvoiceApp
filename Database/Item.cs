using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityToolkit.Mvvm.ComponentModel;

namespace InvoiceApp.Database;

public class Item : ObservableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    private decimal Price { get; set; }
    private int CurrencyId { get; set; }
    private Currency Currency { get; set; }
    public string Unit { get; set; }

    [NotMapped]
    public PriceAmount Amount
    {
        get
        {
            return new PriceAmount()
            {
                Amount = Price, PriceCurrency = Currency
            }; 
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
            builder.HasOne(i => i.Currency).WithMany().HasForeignKey(i => i.CurrencyId);
            builder.Property(x => x.Unit).IsRequired();
            builder.Ignore(x => x.Amount);
        }
    }
}
