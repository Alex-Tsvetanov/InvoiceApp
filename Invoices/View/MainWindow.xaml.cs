using InvoiceApp.Database;
using InvoiceApp.ViewModel;
using System.IO.Packaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

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
            (this.DataContext as MainWindowViewModel).InvoiceLineItemChange((FindName("InvoiceLines") as DataGrid), (sender as ComboBox).SelectedItem as Item);
        }
        private void InvoiceLineContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).InvoiceLineContextMenuOpening((sender as DataGrid).SelectedItem as Model.InvoiceLine);
        }
    }
}
