using Backend.Models.ClientViewModels;
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
    [Route("/api/")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService clientService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ClientController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IClientService clientService)
        {
            this.clientService = clientService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetAllClients))]
        public async Task<ICollection<ClientViewModel>> GetAllClients([FromBody] ClientSearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                return await clientService.GetAllClients(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin))
            {
                return await clientService.GetAllClients(searchViewModel, user.Id, true);
            }
            else
            {
                return await clientService.GetAllClients(searchViewModel, user.Id, false);
            }
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpGet]
        [Route(nameof(GetClientById))]
        public async Task<ClientViewModel> GetClientById(int clientId)
        => await clientService.GetClientById(clientId);


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(AddClient))]
        // [Route("AddClient")]
        public async Task<ApiResponse<bool>> AddClient([FromBody] ClientViewModel model)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await clientService.AddClient(model, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(DeleteClient))]
        public async Task<ApiResponse<bool>> DeleteClient(ClientViewModel deleteClient)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await clientService.DeleteClient(deleteClient, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(UpdateClient))]
        public async Task<ApiResponse<bool>> UpdateClient(ClientViewModel updateClient)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await clientService.UpdateClient(updateClient, user);
        }




    }
}