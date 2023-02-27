using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.ClientViewModels;
using Backend.Models.OpportunityViewModels;


namespace Backend.Models.ProjectViewModels
{
    public class ProjectViewModel
    {

        public ProjectViewModel()
        {
            this.ProjectMilestones = new HashSet<ProjectMilestoneViewModel>();
        }

        public int? Id { get; set; }
        public string? ProjectName { get; set; }

        public int? LineOfBusinessId { get; set; }
        public virtual LineOfBusinessViewModel? LineOfBusiness { get; set; }
        public int? OpportunityId { get; set; }
        public virtual OpportunityViewModel? Opportunity { get; set; }

        public int? ClientId { get; set; }
        public virtual ClientViewModel? Client { get; set; }

        public int? Scope { get; set; }
        public int? Status { get; set; }

        public DateTime? ContractSignatureDate { get; set; }
        public double? ContractValue { get; set; }
        public int? ContractValueCurrency { get; set; }
        public DateTime? KickOffDateScheduled { get; set; }

        public DateTime? KickOffDateActual { get; set; }
        //public DateTime? ClientApprovalDate { get; set; }

        public DateTime? CompletionDateScheduled { get; set; }
        public DateTime? CompletionDateActual { get; set; }

        public int? RetainerValidatity { get; set; }

        public int? CurrentProjectMilestoneId { get; set; }
        public int? CurrentProjectMilestoneIndex { get; set; }

        public int? MilestoneCount { get; set; }
        public virtual ProjectMilestoneViewModel? ProjectMilestone { get; set; }

        public string? Note { get; set; }

        public ICollection<ProjectMilestoneViewModel>? ProjectMilestones { get; set; }



    }
}