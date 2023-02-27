
using System.Linq.Expressions;
using Backend.Models;

namespace Backend.Models.MilestoneViewModels
{
    public class MilestoneSearchViewModel
    {
        public string? name { get; set; }
        public int? lineOfBusinessId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public Expression<Func<Milestone, object>>? orderBy { get; set; }

    }
}