

namespace Backend.Models.ProjectViewModels
{
    public class ProjectViewModelsList
    {
        public ProjectViewModelsList()
        {
            this.Projects = new HashSet<ProjectViewModel>();
        }
        public ICollection<ProjectViewModel> Projects { get; set; }
        public int TotalCount { get; set; }

    }
}