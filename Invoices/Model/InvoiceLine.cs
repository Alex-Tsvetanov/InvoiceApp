using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceApp.Model
{
    public class InvoiceLine : InvoiceApp.Database.InvoiceLine
    {
        private decimal? _convertedCost;

        public decimal? ConvertedCost
        {
            get => _convertedCost;
            set
            {
                SetProperty(ref _convertedCost, value);
                SetProperty(ref _convertedCostWithTax, value * 1.2m);
            }
        }

        private decimal? _convertedCostWithTax;

        public decimal? ConvertedCostWithTax
        {
            get => _convertedCostWithTax;
            set
            {
                SetProperty(ref _convertedCostWithTax, value);
            }
        }
    }
}
