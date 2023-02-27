using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Invoice : BaseEntity
    {

        public int Id { get; set; }

        [ForeignKey(nameof(ProjectMilestone))]
        public int ProjectMilestoneId { get; set; }
        public virtual ProjectMilestone ProjectMilestone { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }

        [ForeignKey(nameof(Client))]
        public int? ClientId { get; set; }

        public virtual Client? Client { get; set; }

        public string? ClientName { get; set; }

        public int? InvoiceDateFW { get; set; }

        public int? InvoiceDateFM { get; set; }

        public int? OriginReference { get; set; }

        public double? UntaxedAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public double? AmountDue { get; set; }
        public int Currency { get; set; }

        public int InvoiceStatus { get; set; }

        public int? PaymentStatus { get; set; }



    }
}