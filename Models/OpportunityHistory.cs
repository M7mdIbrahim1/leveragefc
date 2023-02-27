using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class OpportunityHistory : BaseEntity
    {

        public int Id { get; set; }

        [ForeignKey(nameof(Opportunity))]
        public int OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        public virtual Client Client { get; set; }

        public int ClientStatus { get; set; }

        [ForeignKey(nameof(LineOfBusiness))]
        public int LineOfBusinessId { get; set; }
        public virtual LineOfBusiness LineOfBusiness { get; set; }

        public string? ProjectName { get; set; }

        [ForeignKey(nameof(Project))]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }


        public int Source { get; set; }
        public int Scope { get; set; }

        public int status { get; set; }

        public DateTime? FirstContactDate { get; set; }

        public DateTime? FirstProposalDate { get; set; }

        public double? FirstProposalValue { get; set; }
        public int? FirstProposalValueCurrency { get; set; }

        public double? CurrentProposalValue { get; set; }
        public int? CurrentProposalValueCurrency { get; set; }

        public DateTime? ContractSignatureDate { get; set; }
        public double? FinalContractValue { get; set; }
        public int? FinalProposalValueCurrency { get; set; }

        public int? RetainerValidatity { get; set; }
        public string? Note { get; set; }
        public DateTime HistoryRowCreatedDate { get; set; }


    }
}