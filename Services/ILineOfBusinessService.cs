using Backend.Models;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Helper;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models.CommonViewModels;
using Backend.Auth;

namespace Backend.Services
{
    public interface ILineOfBusinessService
    {
        Task<LineOfBusinessViewModel> GetLineOfBusinessById(int lineOfBusinessId);
        Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinessesByUserId(string userId);
        Task<LineOfBusinessViewModel> GetLOBByName(string lobName, string compName);
        Task<ICollection<LineOfBusiness>> GetLineOfBusinessesByIds(ICollection<int> lineOfBusinessesIds);
        // Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinesses(LineOfBusinessSearchViewModel searchViewModel);
        Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinesses(LineOfBusinessSearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ApiResponse<bool>> AddLineOfBusiness(LineOfBusinessViewModel model, ApplicationUser? user);
        Task<ApiResponse<bool>> DeleteLineOfBusiness(LineOfBusinessViewModel deleteLineOfBusiness, ApplicationUser? user);
        Task<ApiResponse<bool>> UpdateLineOfBusiness(LineOfBusinessViewModel updateLineOfBusiness, ApplicationUser? user);
    }
}