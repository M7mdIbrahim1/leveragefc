using Backend.Data;
using Backend.Models;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Helper;
using Backend.Repositories;
using System.Linq.Expressions;
using AutoMapper;
using Backend.Auth;
using Backend.Models.CompanyViewModels;
using Microsoft.EntityFrameworkCore;


namespace Backend.Services
{
    public class LineOfBusinessService : ILineOfBusinessService
    {

        private readonly IMapper mapper;


        private readonly ApplicationDbContext dbContext;
        private readonly ICompanyService companyService;

        public LineOfBusinessService(ApplicationDbContext dbContext, IMapper mapper, ICompanyService companyService) //IRepository<LineOfBusiness> lineOfBusinessRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.companyService = companyService;
        }
        public async Task<ApiResponse<bool>> AddLineOfBusiness(LineOfBusinessViewModel model, ApplicationUser? user)
        {
            var lob = new LineOfBusiness
            {
                Name = model.Name,
                Description = model.Description,
                CompanyId = model.CompanyId.Value,

                IsRetainer = model.IsRetainer.Value,
                IsActive = model.IsActive
                //   Contact = model.Contact
            };
            if (model.CompanyGroup != null)
            {
                lob.CompanyGroup = new CompanyGroup()
                {
                    Name = model.CompanyGroup.Name,
                    CompanyId = model.CompanyGroup.CompanyId.Value,
                    ChangeSequenceNumber = 0,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatorUserId = user.Id,
                    LastUpdateUserId = user.Id,
                    IsDeleted = false,
                    IsActive = true,
                };
            }
            else if (model.CompanyGroupId.HasValue)
            {
                lob.CompanyGroupId = model.CompanyGroupId;
            }
            var lineOfBusiness = await new LineOfBusinessRepository(dbContext).AddAsync(lob, user);
            if (lineOfBusiness == null)
            {
                return new ApiResponse<bool>(false, "Error in creating lineOfBusiness", false);
            }
            return new ApiResponse<bool>(true, "LineOfBusiness was created successfully", true);
        }

        public async Task<ApiResponse<bool>> UpdateLineOfBusiness(LineOfBusinessViewModel updatedLineOfBusiness, ApplicationUser? user)
        {
            var lineOfBusiness = await new LineOfBusinessRepository(dbContext).GetAsync(a => a.Id == updatedLineOfBusiness.Id);
            if (lineOfBusiness == null)
            {
                return new ApiResponse<bool>(false, "LineOfBusiness not found", false);
            }

            bool changeActive = lineOfBusiness.IsActive != updatedLineOfBusiness.IsActive;

            lineOfBusiness.Name = updatedLineOfBusiness.Name;
            lineOfBusiness.Description = updatedLineOfBusiness.Description;
            lineOfBusiness.CompanyId = updatedLineOfBusiness.CompanyId.Value;
            lineOfBusiness.IsRetainer = updatedLineOfBusiness.IsRetainer.Value;
            lineOfBusiness.IsActive = updatedLineOfBusiness.IsActive;
            // lineOfBusiness.Contact = updatedLineOfBusiness.Contact;

            if (updatedLineOfBusiness.CompanyGroup != null)
            {
                lineOfBusiness.CompanyGroup = new CompanyGroup()
                {
                    Name = updatedLineOfBusiness.CompanyGroup.Name,
                    CompanyId = updatedLineOfBusiness.CompanyGroup.CompanyId.Value
                };
            }
            else if (updatedLineOfBusiness.CompanyGroupId.HasValue)
            {
                lineOfBusiness.CompanyGroupId = updatedLineOfBusiness.CompanyGroupId;
            }

            var isUpdated = await new LineOfBusinessRepository(dbContext).UpdateAsync(lineOfBusiness, lineOfBusiness.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the lineOfBusiness was not updated", false);
            }

            bool userUpdate = true;

            if (changeActive)
            {
                //TODO: update all LOB users to inactive
                userUpdate = await updateLOBUsers(updatedLineOfBusiness.Id.Value, lineOfBusiness.IsActive.Value);
            }

            if (!userUpdate)
            {
                return new ApiResponse<bool>(false, "Line of business updated! yet error with setting its users to inActive", false);
            }

            return new ApiResponse<bool>(true, "LineOfBusiness updated successfully", true);
        }

        public async Task<bool> updateLOBUsers(int lobId, bool activeValue)
        {
            var lineOfBusiness = await new LineOfBusinessRepository(dbContext).GetAsync(a => a.Id == lobId, null, true, l => l.Users);
            var isUpdated = true;
            try
            {
                foreach (var user in lineOfBusiness.Users)
                {

                    user.IsActive = activeValue;
                    dbContext.Update(user);
                }
                dbContext.SaveChanges();
            }
            catch
            {
                isUpdated = false;
            }

            return isUpdated;
        }

        public async Task<ApiResponse<bool>> DeleteLineOfBusiness(LineOfBusinessViewModel deleteLineOfBusiness, ApplicationUser? user)
        {
            //var isDeleted = await new LineOfBusinessRepository(dbContext).DeleteAsync(s => s.Id == lineOfBusinessId);
            // return new ApiResponse<bool>(isDeleted, isDeleted ? "lineOfBusiness was deleted successfully" : "Error in deleting lineOfBusiness", isDeleted);

            var lineOfBusiness = await new LineOfBusinessRepository(dbContext).GetAsync(a => a.Id == deleteLineOfBusiness.Id);
            if (lineOfBusiness == null)
            {
                return new ApiResponse<bool>(false, "LineOfBusiness not found", false);
            }

            lineOfBusiness.IsDeleted = true;

            var isUpdated = await new LineOfBusinessRepository(dbContext).UpdateAsync(lineOfBusiness, deleteLineOfBusiness.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the lineOfBusiness was not deleted", false);
            }


            //TODO: update all LOB users to inactive
            bool userUpdate = await updateLOBUsers(lineOfBusiness.Id, false);


            if (!userUpdate)
            {
                return new ApiResponse<bool>(false, "Line of business deleted! yet error with setting its users to inActive", false);
            }

            return new ApiResponse<bool>(true, "LineOfBusiness deleted successfully", true);
        }

        public async Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinesses(LineOfBusinessSearchViewModel searchViewModel, string? userId, bool isOwner)
        {


            Paging<LineOfBusiness> paging = null;

            if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
            {
                paging = new Paging<LineOfBusiness>
                {
                    Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
                    OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
                    PageSize = searchViewModel.pageSize.Value
                };
            }


            Expression<Func<LineOfBusiness, bool>> predicate1 = searchViewModel.name != null && searchViewModel.name != "" ? x => x.Name.ToLower().Contains(searchViewModel.name.ToLower()) && x.IsDeleted == false : x => x.IsDeleted == false;

            Expression<Func<LineOfBusiness, bool>> predicate2;
            if (userId != null && userId != "")
            {
                if (isOwner)
                {
                    var companies = (await companyService.GetCompanies(new CompanySearchViewModel(), userId, true)).Select(x => x.Id);
                    if (searchViewModel.companyId != null && searchViewModel.companyId != -1)
                    {
                        if (companies.Any(x => x == searchViewModel.companyId))
                        {
                            predicate2 = x => x.CompanyId == searchViewModel.companyId;
                        }
                        else
                        {
                            predicate2 = x => false;
                        }
                    }
                    else
                    {
                        predicate2 = x => companies.Contains(x.CompanyId);
                    }
                }
                else
                {
                    var lobs = await GetLineOfBusinessesByUserId(userId);
                    predicate2 = x => lobs.Any(y => y.Id == x.Id);
                }
            }
            else
            {
                predicate2 = searchViewModel.companyId != null && searchViewModel.companyId != -1 ? x => x.CompanyId == searchViewModel.companyId : x => 1 == 1;
            }
            ParameterExpression param = predicate1.Parameters[0];
            Expression<Func<LineOfBusiness, bool>> predicate3 = Expression.Lambda<Func<LineOfBusiness, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);
            // Expression<Func<Company, bool>> predicate3 = (x => predicate1(x) && predicate2(x));

            var lineOFBusinesesCount = (await new LineOfBusinessRepository(dbContext).GetListAsync(predicate3, null)).Count();


            var lineOfBusinesses = await new LineOfBusinessRepository(dbContext).GetListAsync(predicate3, paging, p => p.Company, p => p.Clients, p => p.CompanyGroup);
            var model = new List<LineOfBusinessViewModel>();
            foreach (var lineOfBusiness in lineOfBusinesses)
            {
                model.Add(new LineOfBusinessViewModel
                {
                    Id = lineOfBusiness.Id,
                    Name = lineOfBusiness.Name,
                    Description = lineOfBusiness.Description,
                    CompanyId = lineOfBusiness.CompanyId,
                    IsRetainer = lineOfBusiness.IsRetainer,
                    IsActive = lineOfBusiness.IsActive,
                    Company = new CompanyViewModel()
                    {
                        Name = lineOfBusiness.Company.Name,
                        Id = lineOfBusiness.Company.Id
                    },
                    CompanyGroupId = lineOfBusiness.CompanyGroupId,
                    Clients = lineOfBusiness.Clients.Select(x => new Models.ClientViewModels.ClientViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                    }).ToList(),
                    TotalCount = lineOFBusinesesCount
                });
            }
            return model;
        }

        // public async Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinesses(SearchViewModel<LineOfBusiness> searchViewModel)
        // {
        //     Paging<LineOfBusiness> paging = null;

        //     if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
        //     {
        //         paging = new Paging<LineOfBusiness>
        //         {
        //             Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
        //             OrderBy = searchViewModel.orderBy,
        //             PageSize = searchViewModel.pageSize.Value
        //         };
        //     }

        //     var lineOfBusinesss = await new LineOfBusinessRepository(dbContext).GetListAsync(searchViewModel.predicate, paging, p => p.Company);
        //     var model = new List<LineOfBusinessViewModel>();
        //     foreach (var lineOfBusiness in lineOfBusinesss)
        //     {
        //         model.Add(new LineOfBusinessViewModel
        //         {
        //             Id = lineOfBusiness.Id,
        //             Name = lineOfBusiness.Name,
        //             Description = lineOfBusiness.Description,
        //             CompanyId = lineOfBusiness.CompanyId,
        //             Company = lineOfBusiness.Company,
        //             //  Contact = lineOfBusiness.Contact,
        //         });
        //     }
        //     return model;

        // }

        public async Task<LineOfBusinessViewModel> GetLineOfBusinessById(int lineOfBusinessId)
        {
            var lineOfBusiness = await new LineOfBusinessRepository(dbContext).GetAsync(c => c.Id == lineOfBusinessId, null, false, p => p.Company);
            if (lineOfBusiness != null)
            {

                var lineOfBusinessViewModel = mapper.Map<LineOfBusinessViewModel>(lineOfBusiness);
                return lineOfBusinessViewModel;
            }
            else
            {
                return null;
            }

        }

        public async Task<LineOfBusinessViewModel> GetLOBByName(string lobName, string compName)
        {
            List<LineOfBusiness> lineOfBusinesses = (await new LineOfBusinessRepository(dbContext).GetListAsync(c => c.Name == lobName, null, p => p.Company, p => p.Milestones)).ToList();

            if (lineOfBusinesses != null && lineOfBusinesses.Count > 0)
            {
                var lineOfBusiness = lineOfBusinesses.Where(x => x.Company.Name == compName).First();
                if (lineOfBusiness != null)
                {
                    var lineOfBusinessViewModel = new LineOfBusinessViewModel
                    {
                        Id = lineOfBusiness.Id,
                        Name = lineOfBusiness.Name,
                        Description = lineOfBusiness.Description,
                        CompanyId = lineOfBusiness.CompanyId,
                        IsRetainer = lineOfBusiness.IsRetainer,
                        IsActive = lineOfBusiness.IsActive,
                        Company = new CompanyViewModel()
                        {
                            Name = lineOfBusiness.Company.Name,
                            Id = lineOfBusiness.Company.Id
                        },
                        Clients = lineOfBusiness.Clients.Select(x => new Models.ClientViewModels.ClientViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                        }).ToList(),
                        Milestones = lineOfBusiness.Milestones.Select(x => new Models.MilestoneViewModels.MilestoneViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                        }).ToList(),
                        // TotalCount = lineOFBusinesesCount
                    };
                    return lineOfBusinessViewModel;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public async Task<ICollection<LineOfBusiness>> GetLineOfBusinessesByIds(ICollection<int> lineOfBusinessIds)
        {
            return (await new LineOfBusinessRepository(dbContext).GetListAsync(c => lineOfBusinessIds.Contains(c.Id), null, p => p.Company)).ToList();
            // var lineOfBusinessesViewModel = new List<LineOfBusinessViewModel>();
            // if (lineOfBusinesses != null)
            // {
            //     foreach (var lineOfBusiness in lineOfBusinesses)
            //     {
            //         lineOfBusinessesViewModel.Add(new LineOfBusinessViewModel(){
            //             Id= lineOfBusiness.Id,
            //             Name= lineOfBusiness.Name,
            //             Description = lineOfBusiness.Description,
            //             CompanyId = lineOfBusiness.CompanyId
            //         });
            //     }
            //     return lineOfBusinessesViewModel;
            // }
            // else
            // {
            //     return null;
            // }

        }

        public async Task<ICollection<LineOfBusinessViewModel>> GetLineOfBusinessesByUserId(string userId)
        {
            var lineOfBusinesses = await new LineOfBusinessRepository(dbContext).GetListAsync(c => c.Users.Any(x => x.Id == userId), null, p => p.Company, p => p.Users);
            var lineOfBusinessesViewModel = new List<LineOfBusinessViewModel>();
            foreach (var lineOfBusiness in lineOfBusinesses)
            {
                lineOfBusinessesViewModel.Add(new LineOfBusinessViewModel
                {
                    Id = lineOfBusiness.Id,
                    Name = lineOfBusiness.Name,
                    Description = lineOfBusiness.Description,
                    CompanyId = lineOfBusiness.CompanyId,
                    IsRetainer = lineOfBusiness.IsRetainer,
                    IsActive = lineOfBusiness.IsActive,
                    Company = new CompanyViewModel()
                    {
                        Name = lineOfBusiness.Company.Name,
                        Id = lineOfBusiness.Company.Id
                    },
                });
            }
            return lineOfBusinessesViewModel;

        }



    }
}