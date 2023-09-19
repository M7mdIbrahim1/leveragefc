using Backend.Models.CompanyViewModels;
using Backend.Models.CommonViewModels;
using Backend.Helper;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Backend.Auth;
using Microsoft.AspNetCore.Identity;


namespace Backend.Controllers
{


    // [Authorize]
    [ApiController]
    [Route("/api/Company/")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CompanyController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ICompanyService companyService)
        {
            this.companyService = companyService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetCompanies))]
        public async Task<ICollection<CompanyViewModel>> GetCompanies([FromBody] CompanySearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                return await companyService.GetCompanies(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin))
            {
                return await companyService.GetCompanies(searchViewModel, user.Id, true);
            }
            else
            {
                return await companyService.GetCompanies(searchViewModel, user.Id, false);
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpGet]
        [Route(nameof(GetCompanyById))]
        public async Task<CompanyViewModel> GetCompanyById(int companyId)
        => await companyService.GetCompanyById(companyId);


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(AddCompany))]
        // [Route("AddCompany")]
        public async Task<ApiResponse<bool>> AddCompany([FromBody] CompanyViewModel model)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await companyService.AddCompany(model, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(DeleteCompany))]
        public async Task<ApiResponse<bool>> DeleteCompany(CompanyViewModel deleteCompany)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await companyService.DeleteCompany(deleteCompany, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(UpdateCompany))]
        public async Task<ApiResponse<bool>> UpdateCompany(CompanyViewModel updateCompany)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await companyService.UpdateCompany(updateCompany, user);
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin)]
        [HttpPost]
        [Route(nameof(ImportCompanies))]
        public async Task<ICollection<ApiResponse<bool>>> ImportCompanies(ICollection<CompanyViewModel> models)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await companyService.ImportCompanies(models, user);
        }

    }
}