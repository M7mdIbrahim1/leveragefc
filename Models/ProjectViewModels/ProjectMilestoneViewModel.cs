using Backend.Models.MilestoneViewModels;

namespace Backend.Models.ProjectViewModels
{
    public class ProjectMilestoneViewModel
    {

        public int? Id { get; set; }


        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? NeedPayment { get; set; }

        public int? ProjectId { get; set; }
        public virtual ProjectViewModel? Project { get; set; }

        public int? Status { get; set; }

        public int? MilestoneId { get; set; }
        public virtual MilestoneViewModel? Milestone { get; set; }
        public double? PaymentValue { get; set; }
        public int? PaymentValueCurrency { get; set; }
        public DateTime? DateScheduled { get; set; }
        public DateTime? DateActual { get; set; }

        public int? MilestoneIndex { get; set; }

        public bool? Start { get; set; }

        public bool? End { get; set; }


        public int? InvoiceId { get; set; }
        public InvoiceViewModel? Invoice { get; set; }
        public string? Note { get; set; }



    }
}