using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Database;

public class Invoice : ObservableObject
{
    // DB columns
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Number { get; set; }
    public DateTime Date { get; set; }
    private int CustomerId { get; set; }
    private string InvoiceCurrencyName { get; set; }

    // Mapped properties
    [ForeignKey("CustomerId")] 
    public Customer Customer { get; set; }
    public ICollection<InvoiceLine> InvoiceLines { get; set; }
    
    [NotMapped]
    public Type InvoiceCurrency
    {
        get
        {
            if (!string.IsNullOrEmpty(InvoiceCurrencyName))
            {
                return Type.GetType(InvoiceCurrencyName);
            }
            return null;
        }
        set
        {
            InvoiceCurrencyName = value?.FullName;
        }
    }

    public class Mapper : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Number).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).IsRequired();
            builder.Property(x => x.InvoiceCurrencyName).IsRequired();
            builder.HasMany(x => x.InvoiceLines).WithOne(x => x.Invoice).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(x => x.InvoiceCurrency);
        }
    }
}
