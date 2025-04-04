using CommunityToolkit.Mvvm.ComponentModel;
using InvoiceApp.Database;
using InvoiceApp.Services.Currencies;
using InvoiceApp.Services.Money;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace InvoiceApp.Services;

public partial class ViewModelInvoice : ObservableObject
{
    public partial class ViewModelInvoiceLine : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedForAttribute(nameof(TotalPrice))]
        [NotifyPropertyChangedForAttribute(nameof(ConvertedTotalPrice))]
        [NotifyPropertyChangedForAttribute(nameof(ConvertedTotalPriceWithTax))]
        private Item _invoiceItem;

        [ObservableProperty]
        [NotifyPropertyChangedForAttribute(nameof(TotalPrice))]
        [NotifyPropertyChangedForAttribute(nameof(ConvertedTotalPrice))]
        [NotifyPropertyChangedForAttribute(nameof(ConvertedTotalPriceWithTax))]
        private decimal _quantity;

        [ObservableProperty]
        [NotifyPropertyChangedForAttribute(nameof(TotalPrice))]
        [NotifyPropertyChangedForAttribute(nameof(ConvertedTotalPrice))]
        [NotifyPropertyChangedForAttribute(nameof(ConvertedTotalPriceWithTax))]
        private ViewModelInvoice _parent;

        [DependsOn(nameof(InvoiceItem))]
        [DependsOn(nameof(Quantity))]
        public IMoney TotalPrice => Parent.databaseService.getTotalPriceFor(InvoiceItem?.UnitPrice, Quantity);

        [DependsOn(nameof(TotalPrice))]
        public IMoney ConvertedTotalPrice => ExchangeService.Convert(TotalPrice, Parent.Currency, Parent.databaseService.ExchangeRates);
        
        [DependsOn(nameof(ConvertedTotalPrice))]
        public IMoney ConvertedTotalPriceWithTax => Parent.databaseService.getPriceWithTax(ConvertedTotalPrice);

        public ViewModelInvoiceLine(ViewModelInvoice parent)
        {
            Parent = parent;
        }
        public void Remove()
        {
            Parent.InvoiceLines.Remove(this);
        }
        public bool IsEmpty()
        {
            return InvoiceItem == null && Quantity == 0;
        }

        public void refreshCalc()
        {
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(ConvertedTotalPrice));
            OnPropertyChanged(nameof(ConvertedTotalPriceWithTax));
        }

        partial void OnQuantityChanged(decimal value)
        {
            refreshCalc();
            Parent.refreshCalc();
        }

        partial void OnInvoiceItemChanged(Item value)
        {
            refreshCalc();
            Parent.refreshCalc();
        }
    }

    private readonly DatabaseService databaseService;

    [ObservableProperty]
    private BindingList<ViewModelInvoiceLine> _invoiceLines;

    [ObservableProperty]
    private Customer _customer;

    [ObservableProperty]
    private string _invoiceNumber;

    [ObservableProperty]
    private DateTime _invoiceDate;

    private Currency _currency;

    public Currency Currency
    {
        get { return _currency; }
        set
        {
            SetProperty(ref _currency, value);
            refreshCalc();
        }
    }

    [ObservableProperty]
    private IMoney _totalAmount;

    void updateTotalAmount()
    {
        dynamic sum = new Money<BGN>(0);
        if (Currency.GetType() == typeof(BGN))
        {
            sum = new Money<BGN>(0);
            foreach (var item in InvoiceLines)
            {
                var val = item.ConvertedTotalPriceWithTax.TryConvert<BGN>();
                if (val != null)
                    sum = sum + val;
            }
        }
        else if (Currency.GetType() == typeof(USD))
        {
            sum = new Money<USD>(0);
            foreach (var item in InvoiceLines)
            {
                var val = item.ConvertedTotalPriceWithTax.TryConvert<USD>();
                if (val != null)
                    sum = sum + val;
            }
        }
        else if (Currency.GetType() == typeof(EUR))
        {
            sum = new Money<EUR>(0);
            foreach (var item in InvoiceLines)
            {
                var val = item.ConvertedTotalPriceWithTax.TryConvert<EUR>();
                if (val != null)
                    sum = sum + val;
            }
        }
        SetProperty(ref _totalAmount, (IMoney)sum);
        OnPropertyChanged(nameof(TotalAmount)); // Force UI update
    }

    private void refreshCalc()
    {
        foreach (var item in InvoiceLines)
        {
            item.refreshCalc();
        }
        updateTotalAmount();
    }
    public ViewModelInvoice(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        this.InvoiceLines = new();
    }

    internal void Clear()
    {
        InvoiceNumber = "";
        InvoiceDate = DateTime.Now;
        Customer = null;
        Currency = null;
        InvoiceLines.Clear();
    }

    internal void AddInvoiceLine(InvoiceLine line)
    {
        InvoiceLines.Add(new ViewModelInvoiceLine(this)
        {
            InvoiceItem = line.Item,
            Quantity = line.Quantity
        });
    }
}
