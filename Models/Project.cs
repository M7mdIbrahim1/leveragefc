using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Project : BaseEntity
    {

        public Project()
        {
            this.ProjectMilestones = new HashSet<ProjectMilestone>();
        }

        public int Id { get; set; }
        public string ProjectName { get; set; }

        [ForeignKey(nameof(LineOfBusiness))]
        public int LineOfBusinessId { get; set; }
        public virtual LineOfBusiness LineOfBusiness { get; set; }

        [ForeignKey(nameof(Opportunity))]
        public int OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        public int Scope { get; set; }
        public int Status { get; set; }

        public DateTime? ContractSignatureDate { get; set; }
        public double? ContractValue { get; set; }
        public int? ContractValueCurrency { get; set; }
        public DateTime? KickOffDateScheduled { get; set; }
        public DateTime? KickOffDateActual { get; set; }
        // public DateTime? ClientApprovalDate { get; set; }

        public DateTime? CompletionDateScheduled { get; set; }
        public DateTime? CompletionDateActual { get; set; }

        public int? RetainerValidatity { get; set; }

        [ForeignKey(nameof(ProjectMilestone))]
        public int? CurrentProjectMilestoneId { get; set; }
        public int? CurrentProjectMilestoneIndex { get; set; }
        //[ForeignKey("CurrentProjectMilestoneId")]
        // public virtual ProjectMilestone ProjectMilestone { get; set; }

        public int? MilestoneCount { get; set; }

        public string? Note { get; set; }

        public ICollection<ProjectMilestone> ProjectMilestones { get; set; }

    }
}