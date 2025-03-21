using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InvoiceApp.Services.Money;
using InvoiceApp.Services.Currencies;

namespace InvoiceApp.Database;

public class InvoiceLine : ObservableObject
{
    private Item _item;
    private int _itemId;
    private decimal _quantity;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    private int ItemId
    {
        get => _itemId;
        set => SetProperty(ref _itemId, value);
    }
    public Item Item
    {
        get => _item;
        set
        {
            SetProperty(ref _item, value);
            if (_item != null)
            {
                ItemId = _item.Id;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            SetProperty(ref _quantity, value);
            if (Item is not null)
            {
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    [NotMapped]
    public IMoney TotalPrice
    {
        get
        {
            return Item.UnitPrice switch
            {
                Money<BGN> bgnMoney => bgnMoney * Quantity,
                Money<EUR> eurMoney => eurMoney * Quantity,
                Money<USD> usdMoney => usdMoney * Quantity,
                _ => throw new Exception("Unknown currency"),
            };
        }
    }

    public class Mapper : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.InvoiceId).IsRequired();
            builder.HasOne(i => i.Item).WithMany().HasForeignKey(i => i.ItemId);
            builder.Property(x => x.Quantity).IsRequired();
        }
    }
}
