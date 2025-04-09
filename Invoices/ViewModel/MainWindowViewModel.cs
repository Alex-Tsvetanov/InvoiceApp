using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using InvoiceApp.Database;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using InvoiceApp.Services.Currencies;
using InvoiceApp.Services;
using System.Windows.Threading;
using System.ComponentModel;

namespace InvoiceApp.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly DatabaseService databaseService;

        /// <summary>
        /// The invoice data as it is being filled in the UI
        /// </summary>
        public ViewModelInvoice Invoice { get; }
        
        /// <summary>
        /// The items available for selection in the UI
        /// </summary>
        public ObservableCollection<Item> Items { get; } = new();
        
        /// <summary>
        /// The customers available for selection in the UI
        /// </summary>
        public ObservableCollection<Customer> Customers { get; } = new();
        
        /// <summary>
        /// The invoice currencies available for selection in the UI
        /// </summary>
        public ObservableCollection<Currency> InvoiceCurrencies { get; } = new();

        public ObservableCollection<ViewModelInvoice.ViewModelInvoiceLine> InvoiceLinesToDisplay
        {
            get
            {
                return new ObservableCollection<ViewModelInvoice.ViewModelInvoiceLine>(Invoice.InvoiceLines.Where(line => line != null && !line.IsEmpty()).ToList());
            }
        }

        [ObservableProperty]
        public Item _selectedItem;

        [ObservableProperty]
        private Customer _selectedCustomer;

        [ObservableProperty]
        public decimal _quantity;

        /// <summary>
        /// The currently selected invoice line item in the UI
        /// </summary>
        private ViewModelInvoice.ViewModelInvoiceLine? CurrentlyDropDownOpened = null;

        public MainWindowViewModel()
        {
            databaseService = new DatabaseService(new ApplicationDbContext(), -0.2m, -0.1m, 0.2m);
            Invoice = new ViewModelInvoice(databaseService);
            Invoice.InvoiceLines.ListChanged += InvoiceLines_ListChanged;
            LoadData();
            AddEmptyInvoiceLine();
        }
        private void InvoiceLines_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                ViewModelInvoice.ViewModelInvoiceLine newItem = Invoice.InvoiceLines[e.NewIndex];
                newItem.PropertyChanged += InvoiceLines_PropertyChanged;
            }
            OnChange();
        }

        private void InvoiceLines_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModelInvoice.ViewModelInvoiceLine changedItem = (ViewModelInvoice.ViewModelInvoiceLine)sender;
            OnChange();
        }

        private void OnChange()
        {
            OnPropertyChanged(nameof(InvoiceLinesToDisplay));
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(EnsureEmptyInvoiceLine));
        }

        private void EnsureEmptyInvoiceLine()
        {
            if (Invoice.InvoiceLines.Count == 0 || !Invoice.InvoiceLines[Invoice.InvoiceLines.Count - 1].IsEmpty())
            {
                AddEmptyInvoiceLine();
            }
        }

        private void AddEmptyInvoiceLine()
        {
            Invoice.InvoiceLines.Add(new ViewModelInvoice.ViewModelInvoiceLine(Invoice));
        }

        private void LoadData()
        {
            foreach (var item in databaseService.Items)
            {
                Items.Add(item);
            }

            foreach (var customer in databaseService.Customers)
            {
                Customers.Add(customer);
            }

            foreach (var currency in databaseService.Currencies)
            {
                InvoiceCurrencies.Add(currency);
            }

            // Set default currency
            Invoice.Currency = InvoiceCurrencies.FirstOrDefault();
        }

        [RelayCommand]
        private void AddLine()
        {
            var line = new InvoiceLine
            {
                Item = SelectedItem,
                Quantity = Quantity
            };
            Invoice.AddInvoiceLine(line);
            Quantity = 0; // Reset quantity after adding line
        }

        [RelayCommand]
        internal async void GenerateInvoice()
        {
            try
            {
                await databaseService.RegisterInvoice(Invoice);
                databaseService.GeneratePdf("invoice-" + Invoice.InvoiceNumber);
                Invoice.Clear();
                LoadData();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        internal void InvoiceLineItemChange(ViewModelInvoice.ViewModelInvoiceLine selectedItem, Item newSelection)
        {
            if (newSelection is null) return;
            if (selectedItem is null) return;
            selectedItem.InvoiceItem = newSelection;
        }
        internal void InvoiceLineContextMenuOpening(ViewModelInvoice.ViewModelInvoiceLine? invoiceLine)
        {
            CurrentlyDropDownOpened = invoiceLine;
        }
        internal void QunatityTextBox_TextChanged(TextBox sender, ViewModelInvoice.ViewModelInvoiceLine invoiceLine)
        {
            try
            {
                invoiceLine.Quantity = decimal.Parse(sender.Text);
            }
            catch(FormatException)
            {
                invoiceLine.Quantity = 0;
            }
        }
        internal void InvoiceCurrency_SelectionChanged(Currency? currency)
        {
            if (currency != null)
                Invoice.Currency = currency;
        }
    }
}

