using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace InvoiceApp.Database
{
    public class Invoice : ObservableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public int CustomerId { get; set; }
        private int InvoiceCurrencyId { get; set; }
        
        [ForeignKey("CustomerId")] 
        public Customer Customer { get; set; }
        public ICollection<InvoiceLine> InvoiceLines { get; set; }
        public Currency InvoiceCurrency { get; set; }
        [NotMapped]
        public decimal TotalAmount { get { } }
    }
}
