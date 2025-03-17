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
            }
        }
    }
}
