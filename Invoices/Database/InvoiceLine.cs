using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Database
{
    public class InvoiceLine : ObservableObject
    {
        private Item _item;

        private int _itemId;

        private decimal _quantity;

        private decimal _unitPrice;

        private decimal _totalPrice;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }
        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                SetProperty(ref _unitPrice, value);
                if (Item is not null)
                    TotalPrice = Item.Price * Quantity;
            }
        }
        public decimal Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
                if (Item is not null)
                    TotalPrice = Item.Price * _quantity;
            }
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
                    UnitPrice = _item.Price;
                    TotalPrice = UnitPrice * Quantity;
                }
            }
        }
    }
}
