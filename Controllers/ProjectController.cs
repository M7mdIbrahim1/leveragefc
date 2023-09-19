using Backend.Models.ProjectViewModels;
using Backend.Helper;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Auth;
using Microsoft.AspNetCore.Identity;
using Backend.Models;
using Backend.Models.LineOfBusinessViewModels;
using ClosedXML.Excel;
using System.Data;


namespace Backend.Controllers
{


    // [Authorize]
    [ApiController]
    [Route("/api/")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService projectService;

        private readonly IMilestoneService milestoneService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IProjectService projectService, IMilestoneService milestoneService)
        {
            this.projectService = projectService;
            this.milestoneService = milestoneService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetAllProjects))]
        public async Task<ProjectViewModelsList> GetAllProjects([FromBody] ProjectSearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                return await projectService.GetAllProjects(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin) || roles.Contains(UserRoles.CommercialManager))
            {
                return await projectService.GetAllProjects(searchViewModel, user.Id, true);
            }
            else
            {
                return await projectService.GetAllProjects(searchViewModel, user.Id, false);
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(ExportProjects))]
        public async Task<IActionResult> ExportProjects([FromBody] ProjectSearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var projectViewModelsList = new ProjectViewModelsList();

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                projectViewModelsList = await projectService.GetAllProjects(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin) || roles.Contains(UserRoles.CommercialManager))
            {
                projectViewModelsList = await projectService.GetAllProjects(searchViewModel, user.Id, true);
            }
            else
            {
                projectViewModelsList = await projectService.GetAllProjects(searchViewModel, user.Id, false);
            }

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("Company"),
                                     new DataColumn("Client"),
                                     new DataColumn("Project"),
                                     new DataColumn("ClientStatus"),
                                     new DataColumn("Scope"),
                                     new DataColumn("Status"),
                                    new DataColumn("ContractSignatureDate"),
                                     new DataColumn("ContractValue"),
                                     new DataColumn("Currency"),
                                     new DataColumn("RetainerValidatity"),
                                      });

            foreach (var project in projectViewModelsList.Projects)
            {
                dt.Rows.Add(project.LineOfBusiness.Company.Name + " - " + project.LineOfBusiness.Name,
                 project.Client.Name,
                 project.ProjectName,
                // opportunity.ClientStatus,
                 project.Scope,
                 project.Status,
                 project.ContractSignatureDate,
                 project.ContractValue,
                 project.ContractValueCurrency,
                 project.RetainerValidatity
                 );
            }


            //using ClosedXML.Excel;
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetProjectById))]
        public async Task<ProjectViewModel> GetProjectById(ProjectViewModel model)
        => await projectService.GetProjectById(model);

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetProjectByOpportunityId))]
        public async Task<ProjectViewModel> GetProjectByOpportunityId(ProjectViewModel model)
        => await projectService.GetProjectByOpportunityId(model);


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(AddProject))]
        // [Route("AddProject")]
        public async Task<ApiResponse<bool>> AddProject([FromBody] ProjectViewModel model)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await projectService.AddProject(model, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(DeleteProject))]
        public async Task<ApiResponse<bool>> DeleteProject(ProjectViewModel deleteProject)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await projectService.DeleteProject(deleteProject, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(UpdateProject))]
        public async Task<ApiResponse<bool>> UpdateProject(ProjectViewModel updateProject)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await projectService.UpdateProject(updateProject, user);
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetLOBMilestones))]
        public async Task<ICollection<Milestone>> GetLOBMilestones([FromBody] LineOfBusinessViewModel model)
        => await milestoneService.GetLOBMilestones(model.Id.Value);

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin)]
        [HttpPost]
        [Route(nameof(ImportProjects))]
        public async Task<ICollection<bool>> ImportProjects(ICollection<ProjectViewModel> models)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await projectService.ImportProjects(models, user);
        }

    }
}