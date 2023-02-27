
using System.Linq.Expressions;

namespace Backend.Models.OpportunityViewModels
{
    public class OpportunitySearchViewModel
    {

        public ICollection<int> lineOfBusinessesIds { get; set; }
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
        public int? status { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public Expression<Func<Opportunity, object>>? orderBy { get; set; }

    }
}