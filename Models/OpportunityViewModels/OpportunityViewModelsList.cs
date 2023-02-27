

namespace Backend.Models.OpportunityViewModels
{
    public class OpportunityViewModelsList
    {
        public OpportunityViewModelsList()
        {
            this.Opportunities = new HashSet<OpportunityViewModel>();
        }
        public ICollection<OpportunityViewModel> Opportunities { get; set; }
        public int TotalCount { get; set; }

    }
}