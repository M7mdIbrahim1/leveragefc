using Backend.Data;
using Backend.Models;
using Backend.Models.MilestoneViewModels;
using Backend.Helper;
using Backend.Repositories;
using System.Linq.Expressions;
using AutoMapper;
using Backend.Auth;
using Backend.Models.CompanyViewModels;
using Backend.Models.LineOfBusinessViewModels;


namespace Backend.Services
{
    public class MilestoneService : IMilestoneService
    {

        private readonly IMapper mapper;


        private readonly ApplicationDbContext dbContext;
        private readonly ICompanyService companyService;

        private readonly ILineOfBusinessService lobService;

        public MilestoneService(ApplicationDbContext dbContext, IMapper mapper, ICompanyService companyService, ILineOfBusinessService lobService) //IRepository<Milestone> milestoneRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.companyService = companyService;
            this.lobService = lobService;
        }
        public async Task<ApiResponse<bool>> AddMilestone(MilestoneViewModel model, ApplicationUser? user)
        {
            var milestone = await new MilestoneRepository(dbContext).AddAsync(new Milestone
            {
                Name = model.Name,
                Description = model.Description,
                LineOfBusinessId = model.LineOfBusinessId.Value,
                NeedPayment = model.NeedPayment.HasValue ? model.NeedPayment.Value : false,
                DefaultAmountValue = model.NeedPayment.HasValue ? model.DefaultAmountValue : 0,
                Index = model.Index,
                //   Contact = model.Contact
            }, user);
            if (milestone == null)
            {
                return new ApiResponse<bool>(false, "Error in creating milestone", false);
            }
            return new ApiResponse<bool>(true, "Milestone was created successfully", true);
        }

        public async Task<ApiResponse<bool>> UpdateMilestone(MilestoneViewModel updatedMilestone, ApplicationUser? user)
        {
            var milestone = await new MilestoneRepository(dbContext).GetAsync(a => a.Id == updatedMilestone.Id);
            if (milestone == null)
            {
                return new ApiResponse<bool>(false, "Milestone not found", false);
            }

            milestone.Name = updatedMilestone.Name;
            milestone.Description = updatedMilestone.Description;
            milestone.LineOfBusinessId = updatedMilestone.LineOfBusinessId.Value;
            milestone.NeedPayment = updatedMilestone.NeedPayment.Value;
            milestone.DefaultAmountValue = updatedMilestone.DefaultAmountValue;
            milestone.Index = updatedMilestone.Index;
            // milestone.Contact = updatedMilestone.Contact;

            var isUpdated = await new MilestoneRepository(dbContext).UpdateAsync(milestone, milestone.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the milestone was not updated", false);
            }

            return new ApiResponse<bool>(true, "Milestone updated successfully", true);
        }

        public async Task<ApiResponse<bool>> DeleteMilestone(MilestoneViewModel deleteMilestone, ApplicationUser? user)
        {
            //var isDeleted = await new MilestoneRepository(dbContext).DeleteAsync(s => s.Id == milestoneId);
            // return new ApiResponse<bool>(isDeleted, isDeleted ? "milestone was deleted successfully" : "Error in deleting milestone", isDeleted);

            var milestone = await new MilestoneRepository(dbContext).GetAsync(a => a.Id == deleteMilestone.Id);
            if (milestone == null)
            {
                return new ApiResponse<bool>(false, "Milestone not found", false);
            }

            milestone.IsDeleted = true;

            var isUpdated = await new MilestoneRepository(dbContext).UpdateAsync(milestone, deleteMilestone.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the milestone was not deleted", false);
            }

            return new ApiResponse<bool>(true, "Milestone deleted successfully", true);
        }


        public async Task<ICollection<Milestone>> GetLOBMilestones(int lobId)
        {
            var milestones = (await new MilestoneRepository(dbContext).GetListAsync(x => x.LineOfBusinessId == lobId)).OrderBy(x => x.Index).ToList();
            return milestones;
        }

        public async Task<ICollection<MilestoneViewModel>> GetAllMilestones(MilestoneSearchViewModel searchViewModel, string? userId, bool isOwner)
        {


            Paging<Milestone> paging = null;

            if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
            {
                paging = new Paging<Milestone>
                {
                    Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
                    OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
                    PageSize = searchViewModel.pageSize.Value
                };
            }


            Expression<Func<Milestone, bool>> predicate1 = searchViewModel.name != null && searchViewModel.name != "" ? x => x.Name.ToLower().Contains(searchViewModel.name.ToLower()) && x.IsDeleted == false : x => x.IsDeleted == false;

            Expression<Func<Milestone, bool>> predicate2;
            if (userId != null && userId != "")
            {
                ICollection<LineOfBusinessViewModel> lobs = new List<LineOfBusinessViewModel>();
                if (isOwner)
                {
                    var companies = (await companyService.GetCompanies(new CompanySearchViewModel(), userId, true));

                    foreach (var company in companies)
                    {
                        foreach (var item in company.LineOfBusinesses)
                        {
                            lobs.Add(item);
                        }
                    }
                }
                else
                {
                    lobs = await lobService.GetLineOfBusinessesByUserId(userId);
                }
                if (searchViewModel.lineOfBusinessId != null && searchViewModel.lineOfBusinessId != -1)
                {
                    if (lobs.Any(x => x.Id == searchViewModel.lineOfBusinessId))
                    {
                        predicate2 = x => x.LineOfBusinessId == searchViewModel.lineOfBusinessId;
                    }
                    else
                    {
                        predicate2 = x => false;
                    }
                }
                else
                {
                    predicate2 = x => lobs.Any(y => y.Id == x.LineOfBusinessId);
                }

            }
            else
            {
                predicate2 = searchViewModel.lineOfBusinessId != null && searchViewModel.lineOfBusinessId != -1 ? x => x.LineOfBusinessId == searchViewModel.lineOfBusinessId : x => 1 == 1;
            }
            ParameterExpression param = predicate1.Parameters[0];
            Expression<Func<Milestone, bool>> predicate3 = Expression.Lambda<Func<Milestone, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);
            // Expression<Func<Company, bool>> predicate3 = (x => predicate1(x) && predicate2(x));

            var milestoneCount = (await new MilestoneRepository(dbContext).GetListAsync(predicate3, null)).Count();


            var milestones = (await new MilestoneRepository(dbContext).GetListAsync(predicate3, paging, p => p.LineOfBusiness, p => p.LineOfBusiness.Company)).OrderBy(x => x.Index);
            var model = new List<MilestoneViewModel>();
            foreach (var milestone in milestones)
            {
                model.Add(new MilestoneViewModel
                {
                    Id = milestone.Id,
                    Name = milestone.Name,
                    Description = milestone.Description,
                    LineOfBusinessId = milestone.LineOfBusinessId,
                    LineOfBusiness = new LineOfBusinessViewModel()
                    {
                        Name = milestone.LineOfBusiness.Name,
                        Id = milestone.LineOfBusiness.Id,
                        Company = new CompanyViewModel()
                        {
                            Id = milestone.LineOfBusiness.Company.Id,
                            Name = milestone.LineOfBusiness.Company.Name,
                        }
                    },
                    NeedPayment = milestone.NeedPayment,
                    DefaultAmountValue = milestone.DefaultAmountValue,
                    Index = milestone.Index,
                    TotalCount = milestoneCount
                });
            }
            return model;
        }

        public async Task<MilestoneViewModel> GetMilestoneById(int milestoneId)
        {
            var milestone = await new MilestoneRepository(dbContext).GetAsync(c => c.Id == milestoneId, null, false, p => p.LineOfBusiness);
            if (milestone != null)
            {

                var milestoneViewModel = mapper.Map<MilestoneViewModel>(milestone);
                return milestoneViewModel;
            }
            else
            {
                return null;
            }

        }

        public async Task<ICollection<MilestoneViewModel>> GetMilestonesByIds(ICollection<int> milestoneIds)
        {
            var milestones = await new MilestoneRepository(dbContext).GetListAsync(c => milestoneIds.Contains(c.Id), null, p => p.LineOfBusiness);
            var milestonesViewModel = new List<MilestoneViewModel>();
            if (milestones != null)
            {
                foreach (var milestone in milestones)
                {
                    milestonesViewModel.Add(mapper.Map<MilestoneViewModel>(milestone));
                }
                return milestonesViewModel;
            }
            else
            {
                return null;
            }

        }



    }
}