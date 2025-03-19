using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using InvoiceApp.Database;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.IO.Packaging;
using System.IO;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace InvoiceApp.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;
        private readonly decimal bigQuantityDiscount = -0.10m;
        private readonly decimal bigSaleDiscount = 0.80m;

        public ObservableCollection<Item> Items { get; } = new();
        public ObservableCollection<Customer> Customers { get; } = new();
        public BindingList<Model.InvoiceLine> InvoiceLines { get; } = new();

        [ObservableProperty]
        private Item _selectedItem;

        [ObservableProperty]
        private InvoiceLine _selectedInvoiceLine;

        [ObservableProperty]
        private Customer _selectedCustomer;

        [ObservableProperty]
        private decimal _quantity;

        [ObservableProperty]
        private string _invoiceNumber;

        [ObservableProperty]
        private DateTime _invoiceDate = DateTime.Now;

        [ObservableProperty]
        private decimal _totalAmount;
        public ObservableCollection<Currency> InvoiceCurrencies { get; } = new();

        private Currency _selectedInvoiceCurrency;

        public Currency SelectedInvoiceCurrency
        {
            get 
            {
                return _selectedInvoiceCurrency;
            }
            set 
            {
                if (_selectedInvoiceCurrency != value)
                {
                    SetProperty(ref _selectedInvoiceCurrency, value);
                    updateConversions(null, null);
                }
            }
        }

        public ObservableCollection<Item> AvailableItems { get; } = new();

        private InvoiceLine? CurrentlyDropDownOpened = null;

        public ObservableCollection<Item> AvailableItemsWithCurrent { get; } = new();

        public MainWindowViewModel()
        {
            _context = new ApplicationDbContext();
            _context.Database.EnsureCreated();
            LoadData();
            UpdateNewItemsSelection();
            InvoiceLines.ListChanged += updateConversions;
        }

        private void updateTotalAmount(object? sender, ListChangedEventArgs e)
        {
            TotalAmount = InvoiceLines.Select(x => x.ConvertedCost == null ? 0 : (decimal)x.ConvertedCost).Sum();
            if (TotalAmount > 10000m)
            {
                TotalAmount = TotalAmount * bigSaleDiscount;
            }
        }

        private void updateConversions(object? sender, ListChangedEventArgs e)
        {
            foreach (var line in InvoiceLines)
            {
                if (line.Item is null || line.Item.Currency is null)
                {
                    line.ConvertedCost = null;
                }
                else
                {
                    line.ConvertedCost = ConvertedAmount(line.TotalPrice, line.Item.Currency, SelectedInvoiceCurrency);
                }
            }
            updateTotalAmount(null, null);
        }

        private void LoadData()
        {
            _context.Items.Include(i => i.Currency).Load();

            foreach (var item in _context.Items.Local.ToObservableCollection())
            {
                Items.Add(item);
            }

            _context.Customers.Load();
            foreach (var customer in _context.Customers.Local.ToObservableCollection())
            {
                Customers.Add(customer);
            }

            // Load currencies
            _context.Currencies.Load();
            foreach (var currency in _context.Currencies.Local.ToObservableCollection())
            {
                InvoiceCurrencies.Add(currency);
            }

            // Set default currency
            SelectedInvoiceCurrency = InvoiceCurrencies.FirstOrDefault();
        }
        private void UpdateNewItemsSelection()
        {
            AvailableItems.Clear();
            var addedItemIds = InvoiceLines.Where(line => line is not null && line.Item is not null).Select(line => line.Item.Id).ToList();
            var availableItems = _context.Items.Local.Where(item => !addedItemIds.Contains(item.Id)).ToList();

            foreach (var item in availableItems)
            {
                AvailableItems.Add(item);
            }
            OnPropertyChanged(nameof(AvailableItems));
            UpdateAddedItemsSelection();
            OnPropertyChanged(nameof(AvailableItemsWithCurrent));
        }

        private void UpdateAddedItemsSelection()
        {
            AvailableItemsWithCurrent.Clear();

            foreach (var item in AvailableItems)
            {
                AvailableItemsWithCurrent.Add(item);
            }

            if (CurrentlyDropDownOpened is not null && AvailableItemsWithCurrent.Where(item => item is not null && item.Id != CurrentlyDropDownOpened?.Id).Count() == 0)
            {
                AvailableItemsWithCurrent.Add(CurrentlyDropDownOpened.Item);
            }

            OnPropertyChanged(nameof(AvailableItemsWithCurrent));
        }

        [RelayCommand]
        private void AddLine()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select an item.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Quantity <= 0)
            {
                MessageBox.Show("Quantity must be greater than zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal totalPrice = SelectedItem.Price * Quantity * (Decimal.Parse((Quantity >= 10).ToString()) * bigQuantityDiscount + 1);

            var line = new Model.InvoiceLine
            {
                Item = SelectedItem,
                Quantity = Quantity,
                UnitPrice = SelectedItem.Price,
                TotalPrice = totalPrice,
                ConvertedCost = ConvertedAmount(totalPrice, SelectedItem.Currency, SelectedInvoiceCurrency)
            };
            InvoiceLines.Add(line);
            UpdateNewItemsSelection();
            Quantity = 0; // Reset quantity after adding line
        }

        [RelayCommand]
        internal void GenerateInvoice()
        {
            if (SelectedCustomer != null && InvoiceLines.Any())
            {
                var invoice = new Invoice
                {
                    Number = InvoiceNumber,
                    Date = InvoiceDate,
                    CustomerId = SelectedCustomer.Id,
                    TotalAmount = TotalAmount,
                    TotalAmountCurrencyId = SelectedInvoiceCurrency.Id,
                    InvoiceLines = new List<InvoiceLine>()
                };

                _context.Invoices.Add(invoice);
                _context.SaveChanges();

                var invoiceLines = InvoiceLines.Select(x => new InvoiceLine()
                {
                    InvoiceId = invoice.Id,
                    ItemId = x.Item.Id,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TotalPrice = x.TotalPrice
                }).ToList();

                _context.InvoiceLines.AddRange(invoiceLines);
                _context.SaveChanges();
            
                /*
                  *  Convert WPF -> XPS -> PDF
                  */
                MemoryStream lMemoryStream = new MemoryStream();
                Package package = Package.Open(lMemoryStream, FileMode.Create);
                XpsDocument doc = new XpsDocument(package);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

                // This is your window
                writer.Write(Application.Current.MainWindow);

                doc.Close();
                package.Close();

                // Convert 
                MemoryStream outStream = new MemoryStream();
                PdfSharp.Xps.XpsConverter.Convert(lMemoryStream, outStream, false);

                // Write pdf file
                FileStream fileStream = new FileStream("invoice-" + getInvoiceNumber() + ".pdf", FileMode.Create);
                outStream.CopyTo(fileStream);

                // Clean up
                outStream.Flush();
                outStream.Close();
                fileStream.Flush();
                fileStream.Close();
                // Допълнителна логика за печат или експорт на фактурата
                // ...

                InvoiceLines.Clear();
                TotalAmount = 0;
                InvoiceNumber = "";
            }
        }
        internal void InvoiceLineItemChange(DataGrid grid, Item newSelection)
        {
            if (newSelection is null) return;
            InvoiceLine selectedItem = grid.CurrentItem as Model.InvoiceLine;
            if (selectedItem is null) return;
            decimal oldCost = selectedItem.TotalPrice;
            selectedItem.Item = newSelection;
            UpdateNewItemsSelection();
        }
        internal void InvoiceLineContextMenuOpening(Model.InvoiceLine? invoiceLine)
        {
            CurrentlyDropDownOpened = invoiceLine;
            UpdateAddedItemsSelection();
        }
        internal decimal? ConvertedAmount(decimal amount, Currency inputCurrency, Currency outputCurrency)
        {
            if (inputCurrency == outputCurrency)
            {
                return amount;
            }
            if (inputCurrency == null || outputCurrency == null)
            {
                return null;
            }
            var exchangeRate = _context.ExchangeRates
                .FirstOrDefault(er => er.FromCurrency == inputCurrency && er.ToCurrency == outputCurrency);
            if (exchangeRate == null)
            {
                return null;
            }
            return Math.Round(amount * exchangeRate.Rate, 3);
        }

        internal string getInvoiceNumber()
        {
            return InvoiceNumber;
        }
    }
}

