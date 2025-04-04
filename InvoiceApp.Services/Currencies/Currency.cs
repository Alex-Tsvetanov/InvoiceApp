using CommunityToolkit.Mvvm.ComponentModel;


namespace InvoiceApp.Services.Currencies
{
    public abstract partial class Currency : ObservableObject
    {
        [ObservableProperty]
        private string _code;

        [ObservableProperty]
        private string _name;

        protected Currency(string code, string name)
        {
            _code = code;
            _name = name;
        }
    }
}
