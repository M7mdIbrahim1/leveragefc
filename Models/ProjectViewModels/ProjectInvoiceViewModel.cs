using Backend.Models.ClientViewModels;

namespace Backend.Models.ProjectViewModels
{
    public class InvoiceViewModel
    {

        public int? Id { get; set; }

        public int? ProjectMilestoneId { get; set; }
        public virtual ProjectMilestoneViewModel? ProjectMilestone { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? InvoiceNumber { get; set; }

        public int? ClientId { get; set; }

        public ClientViewModel? Client { get; set; }

        public string? ClientName { get; set; }

        public int? InvoiceDateFW { get; set; }

        public int? InvoiceDateFM { get; set; }

        public int? OriginReference { get; set; }

        public double? UntaxedAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public double? AmountDue { get; set; }

        public int? Currency { get; set; }

        public int? InvoiceStatus { get; set; }

        public int? PaymentStatus { get; set; }



    }
}