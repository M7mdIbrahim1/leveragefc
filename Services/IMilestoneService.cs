
using Backend.Models.MilestoneViewModels;
using Backend.Helper;
using Backend.Auth;
using Backend.Models;

namespace Backend.Services
{
    public interface IMilestoneService
    {
        Task<MilestoneViewModel> GetMilestoneById(int milestoneId);
        Task<ICollection<MilestoneViewModel>> GetMilestonesByIds(ICollection<int> milestonesIds);
        Task<ICollection<MilestoneViewModel>> GetAllMilestones(MilestoneSearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ApiResponse<bool>> AddMilestone(MilestoneViewModel model, ApplicationUser? user);
        Task<ApiResponse<bool>> DeleteMilestone(MilestoneViewModel deleteMilestone, ApplicationUser? user);
        Task<ApiResponse<bool>> UpdateMilestone(MilestoneViewModel updateMilestone, ApplicationUser? user);

        Task<ICollection<Milestone>> GetLOBMilestones(int lobId);
    }
}