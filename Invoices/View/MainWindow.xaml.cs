using InvoiceApp.Database;
using InvoiceApp.Services;
using InvoiceApp.Services.Currencies;
using InvoiceApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using static InvoiceApp.Services.ViewModelInvoice;

namespace InvoiceApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void InvoiceLineItemChange(object sender, SelectionChangedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).InvoiceLineItemChange((FindName("InvoiceLines") as DataGrid).CurrentItem as ViewModelInvoice.ViewModelInvoiceLine, (sender as ComboBox).SelectedItem as Item);
        }
        private void InvoiceLineContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).InvoiceLineContextMenuOpening((sender as DataGrid).SelectedItem as ViewModelInvoiceLine);
        }

        private void QunatityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle the text changed event here
            TextBox textBox = (TextBox)sender;
            // Access the DataGrid row's data context if needed
            ViewModelInvoice.ViewModelInvoiceLine invoiceLine = (ViewModelInvoice.ViewModelInvoiceLine)textBox.DataContext;

            (this.DataContext as MainWindowViewModel).QunatityTextBox_TextChanged(textBox, invoiceLine);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Handle the text changed event here
            Button button = (Button)sender;
            // Access the DataGrid row's data context if needed
            ViewModelInvoice.ViewModelInvoiceLine invoiceLine = (ViewModelInvoice.ViewModelInvoiceLine)button.DataContext;
            invoiceLine.Remove();
        }
        private void OpenAlternativeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            AlternativeMainWindow alternativeWindow = new AlternativeMainWindow(this.DataContext);
            alternativeWindow.Show();
        }
    }
}
