using Backend.Models.CompanyViewModels;
using Backend.Models.ClientViewModels;
using Backend.Models.MilestoneViewModels;

namespace Backend.Models.LineOfBusinessViewModels
{
    public class LineOfBusinessViewModel
    {
        public LineOfBusinessViewModel()
        {
            this.Clients = new HashSet<ClientViewModel>();
        }

        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRetainer { get; set; }
        public virtual CompanyViewModel? Company { get; set; }

        public virtual CompanyGroupViewModel? CompanyGroup { get; set; }

        public int? CompanyGroupId { get; set; }

        public ICollection<ClientViewModel>? Clients { get; set; }

        public ICollection<MilestoneViewModel>? Milestones { get; set; }

        public int? Group { get; set; }
        public int? TotalCount { get; set; }

    }
}