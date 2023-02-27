using Backend.Models;
using Backend.Models.CompanyViewModels;
using Backend.Helper;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models.CommonViewModels;
using Backend.Auth;

namespace Backend.Services
{
    public interface ICompanyService
    {
        Task<CompanyViewModel> GetCompanyById(int companyId);
        Task<CompanyViewModel> GetCompanyByName(string companyName);
        Task<ICollection<CompanyViewModel>> GetCompaniesByIds(ICollection<int> companyIds);
        Task<ICollection<CompanyViewModel>> GetCompanies(CompanySearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ApiResponse<bool>> AddCompany(CompanyViewModel model, ApplicationUser? user);
        Task<ApiResponse<bool>> DeleteCompany(CompanyViewModel deleteCompany, ApplicationUser? user);
        Task<ApiResponse<bool>> UpdateCompany(CompanyViewModel updateCompany, ApplicationUser? user);
    }
}