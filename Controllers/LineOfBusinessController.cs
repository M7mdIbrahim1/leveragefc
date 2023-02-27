using Backend.Models.LineOfBusinessViewModels;
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
    [Route("/LineOfBusiness/")]
    public class LineOfBusinessController : ControllerBase
    {
        private readonly ILineOfBusinessService lineOfBusinessService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public LineOfBusinessController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ILineOfBusinessService lineOfBusinessService)
        {
            this.lineOfBusinessService = lineOfBusinessService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetAllLineOfBusinesses))]
        public async Task<ICollection<LineOfBusinessViewModel>> GetAllLineOfBusinesses([FromBody] LineOfBusinessSearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                return await lineOfBusinessService.GetLineOfBusinesses(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin))
            {
                return await lineOfBusinessService.GetLineOfBusinesses(searchViewModel, user.Id, true);
            }
            else
            {
                return await lineOfBusinessService.GetLineOfBusinesses(searchViewModel, user.Id, false);
            }


        }


        //[Authorize(Roles = UserRoles.SuperAdmin+", "+UserRoles.FDBAdmin)]
        // [HttpGet]
        // [Route(nameof(GetLineOfBusinesses))]
        // public async Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinesses(SearchViewModel<LineOfBusiness> searchViewModel)
        // => await lineOfBusinessService.GetLineOfBusinesses(searchViewModel);

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpGet]
        [Route(nameof(GetLineOfBusinessById))]
        public async Task<LineOfBusinessViewModel> GetLineOfBusinessById(int lineOfBusinessId)
        => await lineOfBusinessService.GetLineOfBusinessById(lineOfBusinessId);


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(AddLineOfBusiness))]
        // [Route("AddLineOfBusiness")]
        public async Task<ApiResponse<bool>> AddLineOfBusiness([FromBody] LineOfBusinessViewModel model)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await lineOfBusinessService.AddLineOfBusiness(model, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(DeleteLineOfBusiness))]
        public async Task<ApiResponse<bool>> DeleteLineOfBusiness(LineOfBusinessViewModel deleteLineOfBusiness)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await lineOfBusinessService.DeleteLineOfBusiness(deleteLineOfBusiness, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(UpdateLineOfBusiness))]
        public async Task<ApiResponse<bool>> UpdateLineOfBusiness(LineOfBusinessViewModel updateLineOfBusiness)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await lineOfBusinessService.UpdateLineOfBusiness(updateLineOfBusiness, user);
        }




    }
}