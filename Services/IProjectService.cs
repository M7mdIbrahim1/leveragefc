
using Backend.Models.ProjectViewModels;
using Backend.Helper;
using Backend.Auth;

namespace Backend.Services
{
    public interface IProjectService
    {
        Task<ProjectViewModel> GetProjectById(ProjectViewModel model);

        Task<ProjectViewModel> GetProjectByOpportunityId(ProjectViewModel model);


        Task<ICollection<ProjectViewModel>> GetProjectsByIds(ICollection<int> projectsIds);
        Task<ProjectViewModelsList> GetAllProjects(ProjectSearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ApiResponse<bool>> AddProject(ProjectViewModel model, ApplicationUser? user);
        Task<ApiResponse<bool>> DeleteProject(ProjectViewModel deleteProject, ApplicationUser? user);
        Task<ApiResponse<bool>> UpdateProject(ProjectViewModel updateProject, ApplicationUser? user);
        // Task<ApiResponse<bool>> ExportProjects(ProjectSearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ICollection<bool>> ImportProjects(ICollection<ProjectViewModel> models, ApplicationUser? user);
    }
}