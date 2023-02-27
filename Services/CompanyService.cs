using Backend.Data;
using Backend.Models;
using Backend.Models.CompanyViewModels;
using Backend.Models.CommonViewModels;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.FileViewModels;
using Backend.Helper;
using Backend.Repositories;
using System.Linq.Expressions;
using AutoMapper;



using Microsoft.AspNetCore.Identity;
using Backend.Auth;


namespace Backend.Services
{
    public class CompanyService : ICompanyService
    {

        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        //   private readonly IRepository<Company> companyRepository;

        //  private readonly ILineOfBusinessService lobService;

        private readonly ApplicationDbContext dbContext;

        public CompanyService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IMapper mapper)//, ILineOfBusinessService lobService) //IRepository<Company> companyRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this._userManager = userManager;
            //   this.lobService = lobService;
            //   this.companyRepository= companyRepository;
        }
        public async Task<ApiResponse<bool>> AddCompany(CompanyViewModel model, ApplicationUser? user)
        {
            var company = new Company
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = model.OwnerId,
                IsActive = model.IsActive
                //   Contact = model.Contact
            };
            company.CommercialRegistrationNumber = model.CommercialRegistrationNumber;
            company.AddressLine1 = model.AddressLine1;
            company.AddressLine2 = model.AddressLine2;
            company.State = model.State;
            company.City = model.City;
            company.Country = model.Country;
            company.ContactNumber = model.ContactNumber;
            company.ContactPerson = model.ContactPerson;
            company.Email = model.Email;
            company.TaxCardNumber = model.TaxCardNumber;
            company.PostCode = model.PostCode;

            company.Files = model.Files.Select(x => new Backend.Models.File
            {
                //Id=x.Id.Value,
                Name = x.Name,
                Url = x.Url,
                Status = x.Status,

            }).ToList();

            var addedCompany = await new CompanyRepository(dbContext).AddAsync(company, user);

            if (addedCompany == null)
            {
                return new ApiResponse<bool>(false, "Error in creating company", false);
            }
            return new ApiResponse<bool>(true, "Company was created successfully", true);
        }

        public async Task<ApiResponse<bool>> UpdateCompany(CompanyViewModel updatedCompany, ApplicationUser? user)
        {
            var company = await new CompanyRepository(dbContext).GetAsync(a => a.Id == updatedCompany.Id, null, true, c => c.LineOfBusinesses, c => c.Files);
            if (company == null)
            {
                return new ApiResponse<bool>(false, "Company not found", false);
            }

            bool changeActive = company.IsActive != updatedCompany.IsActive;

            company.Name = updatedCompany.Name;
            company.Description = updatedCompany.Description;
            company.OwnerId = updatedCompany.OwnerId;
            company.IsActive = updatedCompany.IsActive;
            // company.Contact = updatedCompany.Contact;

            company.CommercialRegistrationNumber = updatedCompany.CommercialRegistrationNumber;
            company.AddressLine1 = updatedCompany.AddressLine1;
            company.AddressLine2 = updatedCompany.AddressLine2;
            company.State = updatedCompany.State;
            company.City = updatedCompany.City;
            company.Country = updatedCompany.Country;
            company.ContactNumber = updatedCompany.ContactNumber;
            company.ContactPerson = updatedCompany.ContactPerson;
            company.Email = updatedCompany.Email;
            company.TaxCardNumber = updatedCompany.TaxCardNumber;
            company.PostCode = updatedCompany.PostCode;

            removeFiles(company.Files, company.Files.Where(x => !updatedCompany.Files.Any(y => y.Id == x.Id)).Select(x => x.Id).ToList());
            addFiles(company.Files, updatedCompany.Files.Where(x => !company.Files.Any(y => y.Id == x.Id)).ToList(), user);


            var isUpdated = await new CompanyRepository(dbContext).UpdateAsync(company, company.Id, user);

            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the company was not updated", false);
            }

            bool userUpdate = true;

            if (changeActive)
            {
                //TODO: update all LOB users to inactive
                userUpdate = await updateCompanyUsers(company, updatedCompany.IsActive.Value);
            }

            if (!userUpdate)
            {
                return new ApiResponse<bool>(false, "Compnay updated! yet error with setting its users to inActive", false);
            }

            return new ApiResponse<bool>(true, "Company updated successfully", true);
        }

        public void removeFiles(ICollection<Backend.Models.File> files, List<int> ids)
        {
            foreach (var id in ids)
            {
                files.Remove(files.FirstOrDefault(x => x.Id == id));
            }
        }


        public void addFiles(ICollection<Backend.Models.File> files, ICollection<FileViewModel> addedFiles, ApplicationUser user)
        {
            foreach (var item in addedFiles)
            {
                files.Add(new Backend.Models.File()
                {
                    Name = item.Name,
                    // Id = item.Id.Value,
                    Url = item.Url,
                    Status = item.Status,
                    ChangeSequenceNumber = 0,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatorUserId = user.Id,
                    LastUpdateUserId = user.Id,
                    IsDeleted = false,
                    IsActive = true,
                    // Company = new Company()
                    // {
                    //     Id = item.Company.Id.Value,
                    //     Name = item.Company.Name,
                    //     // Description = item.Company.Description,
                    // }
                });
            }
        }

        public async Task<bool> updateCompanyUsers(Company company, bool activeValue)
        {

            var isUpdated = true;

            try
            {
                foreach (var lineOfBusiness in company.LineOfBusinesses)
                {


                    foreach (var user in lineOfBusiness.Users)
                    {

                        user.IsActive = activeValue;
                        dbContext.Update(user);
                    }
                }
                dbContext.SaveChanges();
            }
            catch
            {
                isUpdated = false;
            }

            return isUpdated;
        }

        public async Task<ApiResponse<bool>> DeleteCompany(CompanyViewModel deleteCompany, ApplicationUser? user)
        {
            //var isDeleted = await new CompanyRepository(dbContext).DeleteAsync(s => s.Id == companyId);
            // return new ApiResponse<bool>(isDeleted, isDeleted ? "company was deleted successfully" : "Error in deleting company", isDeleted);

            var company = await new CompanyRepository(dbContext).GetAsync(a => a.Id == deleteCompany.Id, null, true, c => c.LineOfBusinesses);
            if (company == null)
            {
                return new ApiResponse<bool>(false, "Company not found", false);
            }

            company.IsDeleted = true;

            var isUpdated = await new CompanyRepository(dbContext).UpdateAsync(company, company.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the company was not deleted", false);
            }


            //TODO: update all LOB users to inactive
            bool userUpdate = await updateCompanyUsers(company, false);


            if (!userUpdate)
            {
                return new ApiResponse<bool>(false, "Compnay updated! yet error with setting its users to inActive", false);
            }

            return new ApiResponse<bool>(true, "Company deleted successfully", true);
        }

        public async Task<ICollection<CompanyViewModel>> GetCompanies(CompanySearchViewModel searchViewModel, string? userId, bool isOwner)
        {


            Paging<Company> paging = null;

            if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
            {
                paging = new Paging<Company>
                {
                    Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
                    OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
                    PageSize = searchViewModel.pageSize.Value
                };
            }


            Expression<Func<Company, bool>> predicate1 = searchViewModel.name != null && searchViewModel.name != "" ? x => x.Name.ToLower().Contains(searchViewModel.name.ToLower()) && x.IsDeleted == false : x => x.IsDeleted == false;
            ICollection<LineOfBusinessViewModel> lobs = new List<LineOfBusinessViewModel>();
            Expression<Func<Company, bool>> predicate2;
            if (userId != null && userId != "")
            {
                if (isOwner)
                {
                    predicate2 = x => x.OwnerId == userId;
                }
                else
                {
                    lobs = await GetLineOfBusinessesByUserId(userId);
                    predicate2 = x => x.LineOfBusinesses.Any(y => lobs.Any(z => z.Id == y.Id));
                }
            }
            else
            {
                predicate2 = searchViewModel.ownerId != null && searchViewModel.ownerId != "" ? x => x.OwnerId == searchViewModel.ownerId : x => 1 == 1;
            }
            ParameterExpression param = predicate1.Parameters[0];
            Expression<Func<Company, bool>> predicate3 = Expression.Lambda<Func<Company, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);
            // Expression<Func<Company, bool>> predicate3 = (x => predicate1(x) && predicate2(x));

            var companysCount = (await new CompanyRepository(dbContext).GetListAsync(predicate3, null, p => p.LineOfBusinesses)).Count();

            var companys = await new CompanyRepository(dbContext).GetListAsync(predicate3, paging, p => p.LineOfBusinesses, p => p.LineOfBusinesses.Select(x => x.Clients), p => p.Files, p => p.CompanyGroups);
            var model = new List<CompanyViewModel>();

            foreach (var company in companys)
            {


                model.Add(new CompanyViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    OwnerId = company.OwnerId,
                    ContactPerson = company.ContactPerson,
                    Email = company.Email,
                    ContactNumber = company.ContactNumber,
                    AddressLine1 = company.AddressLine1,
                    AddressLine2 = company.AddressLine2,
                    PostCode = company.PostCode,
                    Country = company.Country,
                    State = company.State,
                    City = company.City,
                    CommercialRegistrationNumber = company.CommercialRegistrationNumber,
                    TaxCardNumber = company.TaxCardNumber,
                    Files = company.Files.Select(x => new FileViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Url = x.Url,
                        Status = x.Status
                    }).ToList(),
                    CompanyGroups = company.CompanyGroups.Select(x => new CompanyGroupViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CompanyId = x.CompanyId
                    }).ToList(),
                    IsActive = company.IsActive,
                    TotalCount = companysCount,
                    //Owner = company.Owner,
                    // Owner = _userManager.FindByIdAsync(company.OwnerId).Result,
                    LineOfBusinesses = company.LineOfBusinesses.Where(x => (userId == null || isOwner) || (lobs.Count > 0 && lobs.Any(y => y.Id == x.Id))).Select(x => new LineOfBusinessViewModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        IsRetainer = x.IsRetainer,
                        Clients = x.Clients.Select(x => new Models.ClientViewModels.ClientViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                        }).ToList(),
                    }).ToList(),

                    //  Contact = company.Contact,
                });

            }




            return model;
        }

        // public async Task<ICollection<CompanyViewModel>> GetCompanies(CompanySearchViewModel searchViewModel)
        // {
        //     Paging<Company> paging = null;

        //     if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
        //     {
        //         paging = new Paging<Company>
        //         {
        //             Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
        //             OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
        //             PageSize = searchViewModel.pageSize.Value
        //         };
        //     }

        //     Expression<Func<Company, bool>> predicate1 = searchViewModel.name != null && searchViewModel.name != "" ? x => x.Name.ToLower().Contains(searchViewModel.name.ToLower()) && x.isDeleted == false : x => x.isDeleted == false;
        //     Expression<Func<Company, bool>> predicate2 = searchViewModel.ownerId != null && searchViewModel.ownerId != "" ? x => x.OwnerId == searchViewModel.ownerId : x => 1 == 1;
        //     ParameterExpression param = predicate1.Parameters[0];
        //     Expression<Func<Company, bool>> predicate3 = Expression.Lambda<Func<Company, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);
        //     // Expression<Func<Company, bool>> predicate3 = (x => predicate1(x) && predicate2(x));

        //     var companysCount = (await new CompanyRepository(dbContext).GetListAsync(predicate3, null, p => p.LineOfBusinesses)).Count();


        //     var companys = await new CompanyRepository(dbContext).GetListAsync(predicate3, paging, p => p.LineOfBusinesses);
        //     var model = new List<CompanyViewModel>();
        //     foreach (var company in companys)
        //     {
        //         model.Add(new CompanyViewModel
        //         {
        //             Id = company.Id,
        //             Name = company.Name,
        //             Description = company.Description,
        //             OwnerId = company.OwnerId,
        //             TotalCount = companysCount,

        //             //  Contact = company.Contact,
        //         });
        //     }
        //     return model;

        // }

        public async Task<CompanyViewModel> GetCompanyById(int companyId)
        {
            var company = await new CompanyRepository(dbContext).GetAsync(c => c.Id == companyId, null, false, p => p.LineOfBusinesses);
            if (company != null)
            {

                var companyViewModel = new CompanyViewModel()
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    OwnerId = company.OwnerId,
                };
                return companyViewModel;
            }
            else
            {
                return null;
            }

        }

        public async Task<CompanyViewModel> GetCompanyByName(string companyName)
        {
            var company = await new CompanyRepository(dbContext).GetAsync(c => c.Name == companyName, null, false, p => p.LineOfBusinesses, p => p.LineOfBusinesses.Select(x => x.Milestones));
            if (company != null)
            {
                var companyViewModel = new CompanyViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    OwnerId = company.OwnerId,
                    IsActive = company.IsActive,
                    //TotalCount = companysCount,
                    //Owner = company.Owner,
                    // Owner = _userManager.FindByIdAsync(company.OwnerId).Result,
                    LineOfBusinesses = company.LineOfBusinesses.Select(x => new LineOfBusinessViewModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        Clients = x.Clients.Select(x => new Models.ClientViewModels.ClientViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                        }).ToList(),
                    }).ToList(),

                    //  Contact = company.Contact,
                };
                return companyViewModel;
            }
            else
            {
                return null;
            }

        }

        public async Task<ICollection<CompanyViewModel>> GetCompaniesByIds(ICollection<int> companyIds)
        {
            var companies = await new CompanyRepository(dbContext).GetListAsync(c => companyIds.Contains(c.Id), null, p => p.LineOfBusinesses);
            var companiesViewModel = new List<CompanyViewModel>();
            if (companies != null)
            {
                foreach (var company in companies)
                {
                    companiesViewModel.Add(mapper.Map<CompanyViewModel>(company));
                }
                return companiesViewModel;
            }
            else
            {
                return null;
            }

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
                    IsActive = lineOfBusiness.IsActive,
                    IsRetainer = lineOfBusiness.IsRetainer,
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
                });
            }
            return lineOfBusinessesViewModel;

        }



    }
}