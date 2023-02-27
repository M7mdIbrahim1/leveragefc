using Backend.Models.OpportunityViewModels;
using Backend.Helper;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Auth;
using Microsoft.AspNetCore.Identity;
using ClosedXML.Excel;
using System.Text;



namespace Backend.Controllers
{


    // [Authorize]
    [ApiController]
    [Route("/")]
    public class OpportunityController : ControllerBase
    {
        private readonly IOpportunityService opportunityService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public OpportunityController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IOpportunityService opportunityService)
        {
            this.opportunityService = opportunityService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(ExportOpportunities))]
        public async Task<IActionResult> ExportOpportunities([FromBody] OpportunitySearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var opportunityViewModelsList = new OpportunityViewModelsList();

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                opportunityViewModelsList = await opportunityService.GetAllOpportunities(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin) || roles.Contains(UserRoles.CommercialManager))
            {
                opportunityViewModelsList = await opportunityService.GetAllOpportunities(searchViewModel, user.Id, true);
            }
            else
            {
                opportunityViewModelsList = await opportunityService.GetAllOpportunities(searchViewModel, user.Id, false);
            }

            StringBuilder str = new StringBuilder();
            str.Append("<table border=`" + "1px" + "`b>");
            str.Append("<tr>");
            str.Append("<td><b><font face=Arial Narrow size=3>Company</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Client</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Project</font></b></td>");

            foreach (var opportunity in opportunityViewModelsList.Opportunities)
            {
                str.Append("<tr>");
                str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + opportunity.LineOfBusiness.Company.Name + " - " + opportunity.LineOfBusiness.Name + "</font></td>");
                str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + opportunity.Client.Name + "</font></td>");
                str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + opportunity.ProjectName + "</font></td>");
                str.Append("</tr>");
            }

            str.Append("</table>");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=Information" + DateTime.Now.Year.ToString() + ".xls");
            this.Response.ContentType = "application/vnd.ms-excel";
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(str.ToString());
            return File(temp, "application/vnd.ms-excel");

            // DataTable dt = new DataTable("Grid");
            // dt.Columns.AddRange(new DataColumn[15] { new DataColumn("Company"),
            //                          new DataColumn("Client"),
            //                          new DataColumn("Project"),
            //                          new DataColumn("ClientStatus"),
            //                          new DataColumn("Source"),
            //                          new DataColumn("Scope"),
            //                          new DataColumn("Status"),
            //                          new DataColumn("FirstContactDate"),
            //                          new DataColumn("FirstProposalDate"),
            //                          new DataColumn("FirstProposalValue"),
            //                          new DataColumn("FirstProposalValueCurrency"),
            //                         new DataColumn("ContractSignatureDate"),
            //                          new DataColumn("FinalContractValue"),
            //                          new DataColumn("FinalContractValueCurrency"),
            //                          new DataColumn("RetainerValidatity"),
            //                           });

            // foreach (var opportunity in opportunityViewModelsList.Opportunities)
            // {
            //     dt.Rows.Add(opportunity.LineOfBusiness.Company.Name + " - " + opportunity.LineOfBusiness.Name,
            //      opportunity.Client.Name,
            //      opportunity.ProjectName,
            //      opportunity.ClientStatus,
            //      opportunity.Source,
            //      opportunity.Scope,
            //      opportunity.Status,
            //      opportunity.FirstContactDate,
            //      opportunity.FirstProposalDate,
            //      opportunity.FirstProposalValue,
            //      opportunity.FirstProposalValueCurrency,
            //      opportunity.ContractSignatureDate,
            //      opportunity.FinalContractValue,
            //      opportunity.FinalContractValueCurrency,
            //      opportunity.RetainerValidatity
            //      );
            // }


            // //using ClosedXML.Excel;
            // using (XLWorkbook wb = new XLWorkbook())
            // {
            //     wb.Worksheets.Add(dt);
            //     using (MemoryStream stream = new MemoryStream())
            //     {
            //         wb.SaveAs(stream);
            //         return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
            //     }
            // }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetAllOpportunities))]
        public async Task<OpportunityViewModelsList> GetAllOpportunities([FromBody] OpportunitySearchViewModel searchViewModel)
        {

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(UserRoles.SuperAdmin) || roles.Contains(UserRoles.FDBAdmin))
            {
                return await opportunityService.GetAllOpportunities(searchViewModel, null, false);
            }
            else if (roles.Contains(UserRoles.CompanyAdmin) || roles.Contains(UserRoles.CommercialManager))
            {
                return await opportunityService.GetAllOpportunities(searchViewModel, user.Id, true);
            }
            else
            {
                return await opportunityService.GetAllOpportunities(searchViewModel, user.Id, false);
            }
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(GetOpportunityById))]
        public async Task<OpportunityViewModel> GetOpportunityById([FromBody] OpportunityViewModel opportunityViewModel)
        => await opportunityService.GetOpportunityById(opportunityViewModel);


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(AddOpportunity))]
        // [Route("AddOpportunity")]
        public async Task<ApiResponse<bool>> AddOpportunity([FromBody] OpportunityViewModel model)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await opportunityService.AddOpportunity(model, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(DeleteOpportunity))]
        public async Task<ApiResponse<bool>> DeleteOpportunity(OpportunityViewModel deleteOpportunity)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await opportunityService.DeleteOpportunity(deleteOpportunity, user);
        }


        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(UpdateOpportunity))]
        public async Task<ApiResponse<bool>> UpdateOpportunity(OpportunityViewModel updateOpportunity)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await opportunityService.UpdateOpportunity(updateOpportunity, user);
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route(nameof(CompleteOpportunity))]
        public async Task<ApiResponse<bool>> CompleteOpportunity(OpportunityViewModel updateOpportunity)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await opportunityService.CompleteOpportunity(updateOpportunity, user);
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin)]
        [HttpPost]
        [Route(nameof(ImportOpportunities))]
        public async Task<ICollection<ApiResponse<bool>>> ImportOpportunities(ICollection<OpportunityViewModel> models)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return await opportunityService.ImportOpportunities(models, user);
        }

    }
}