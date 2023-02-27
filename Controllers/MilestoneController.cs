using Backend.Models.MilestoneViewModels;
using Backend.Helper;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Auth;
using Microsoft.AspNetCore.Identity;


namespace Backend.Controllers
{


    // [Authorize]
    [ApiController]
    [Route("/")]
    public class MilestoneController : ControllerBase
    {
        private readonly IMilestoneService milestoneService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public MilestoneController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IMilestoneService milestoneService)
        {
            this.milestoneService = milestoneService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetAllMilestones))]
        public async Task<ICollection<MilestoneViewModel>> GetAllMilestones([FromBody] MilestoneSearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);


            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                return await milestoneService.GetAllMilestones(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin))
            {
                return await milestoneService.GetAllMilestones(searchViewModel, user.Id, true);
            }
            else
            {
                return await milestoneService.GetAllMilestones(searchViewModel, user.Id, false);
            }
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpGet]
        [Route(nameof(GetMilestoneById))]
        public async Task<MilestoneViewModel> GetMilestoneById(int milestoneId)
        => await milestoneService.GetMilestoneById(milestoneId);


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(AddMilestone))]
        // [Route("AddMilestone")]
        public async Task<ApiResponse<bool>> AddMilestone([FromBody] MilestoneViewModel model)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await milestoneService.AddMilestone(model, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(DeleteMilestone))]
        public async Task<ApiResponse<bool>> DeleteMilestone(MilestoneViewModel deleteMilestone)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await milestoneService.DeleteMilestone(deleteMilestone, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(UpdateMilestone))]
        public async Task<ApiResponse<bool>> UpdateMilestone(MilestoneViewModel updateMilestone)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await milestoneService.UpdateMilestone(updateMilestone, user);
        }




    }
}