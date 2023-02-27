using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class ProjectMilestone : BaseEntity
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool NeedPayment { get; set; }

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [ForeignKey(nameof(Milestone))]
        public int MilestoneId { get; set; }
        public virtual Milestone Milestone { get; set; }
        public double? PaymentValue { get; set; }

        public int? PaymentValueCurrency { get; set; }

        public int Status { get; set; }

        public int? MilestoneIndex { get; set; }

        public bool? Start { get; set; }

        public bool? End { get; set; }


        public DateTime? DateScheduled { get; set; }
        public DateTime? DateActual { get; set; }

        [ForeignKey(nameof(Invoice))]
        public int? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public string? Note { get; set; }



    }
}