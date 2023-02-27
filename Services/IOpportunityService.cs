
using Backend.Models.OpportunityViewModels;
using Backend.Models;
using Backend.Helper;
using Backend.Auth;

namespace Backend.Services
{
    public interface IOpportunityService
    {
        Task<OpportunityViewModel> GetOpportunityById(OpportunityViewModel opportunityViewModel);
        Task<Opportunity> GetOpportunityByProjecName(string projecName);
        Task<ICollection<OpportunityViewModel>> GetOpportunitiesByIds(ICollection<int> opportunitiesIds);
        Task<OpportunityViewModelsList> GetAllOpportunities(OpportunitySearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ApiResponse<bool>> AddOpportunity(OpportunityViewModel model, ApplicationUser? user);
        Task<ApiResponse<bool>> DeleteOpportunity(OpportunityViewModel deleteOpportunity, ApplicationUser? user);
        Task<ApiResponse<bool>> UpdateOpportunity(OpportunityViewModel updateOpportunity, ApplicationUser? user);
        Task<ApiResponse<bool>> CompleteOpportunity(OpportunityViewModel updateOpportunity, ApplicationUser? user);
        Task<ICollection<ApiResponse<bool>>> ImportOpportunities(ICollection<OpportunityViewModel> models, ApplicationUser? user);
    }
}