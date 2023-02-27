
using System.Linq.Expressions;

namespace Backend.Models.ProjectViewModels
{
    public class ProjectSearchViewModel
    {
        public string? projectName { get; set; }
        public ICollection<int>? clientIds { get; set; }
        public ICollection<int>? lineOfBusinessesIds { get; set; }
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
        public int? status { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public Expression<Func<Project, object>>? orderBy { get; set; }
    }
}