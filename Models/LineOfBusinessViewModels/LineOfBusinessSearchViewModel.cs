
using System.Linq.Expressions;
using Backend.Models;

namespace Backend.Models.LineOfBusinessViewModels
{
    public class LineOfBusinessSearchViewModel
    {
        public string? name { get; set; }
        public int? companyId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public Expression<Func<LineOfBusiness, object>>? orderBy { get; set; }

    }
}