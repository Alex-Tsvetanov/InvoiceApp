using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Database
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalAmountCurrencyId { get; set; }
        public Currency TotalAmountCurrency { get; set; }
        
        [ForeignKey("CustomerId")] 
        public Customer Customer { get; set; }
        public ICollection<InvoiceLine> InvoiceLines { get; set; }
    }
}
