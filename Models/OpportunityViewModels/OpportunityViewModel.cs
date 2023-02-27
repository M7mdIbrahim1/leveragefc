using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.ClientViewModels;
using Backend.Models.ProjectViewModels;


namespace Backend.Models.OpportunityViewModels
{
    public class OpportunityViewModel
    {

        public int? Id { get; set; }

        public int? ClientId { get; set; }

        public virtual ClientViewModel? Client { get; set; }

        public int? ClientStatus { get; set; }

        public int? LineOfBusinessId { get; set; }

        public virtual LineOfBusinessViewModel? LineOfBusiness { get; set; }

        public string? ProjectName { get; set; }

        public int? ProjectId { get; set; }
        public virtual ProjectViewModel? Project { get; set; }

        public int? Source { get; set; }
        public int? Scope { get; set; }
        public int? Status { get; set; }

        public DateTime? FirstContactDate { get; set; }

        public DateTime? FirstProposalDate { get; set; }

        public double? FirstProposalValue { get; set; }
        public int? FirstProposalValueCurrency { get; set; }

        public double? CurrentProposalValue { get; set; }
        public int? CurrentProposalValueCurrency { get; set; }

        public DateTime? ContractSignatureDate { get; set; }
        public double? FinalContractValue { get; set; }
        public int? FinalContractValueCurrency { get; set; }

        public int? RetainerValidatity { get; set; }
        public string? Note { get; set; }

    }
}