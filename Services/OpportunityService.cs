using Backend.Data;
using Backend.Models;
using Backend.Models.OpportunityViewModels;
using Backend.Helper;
using Backend.Repositories;
using System.Linq.Expressions;
using AutoMapper;
using Backend.Auth;
using Backend.Models.CompanyViewModels;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.ClientViewModels;
using Backend.Models.FileViewModels;
using Microsoft.EntityFrameworkCore;



namespace Backend.Services
{
    public class OpportunityService : IOpportunityService
    {

        private readonly IMapper mapper;


        private readonly ApplicationDbContext dbContext;
        private readonly ICompanyService companyService;

        private readonly ILineOfBusinessService lobService;

        private readonly IClientService clientService;

        public OpportunityService(ApplicationDbContext dbContext, IMapper mapper, ICompanyService companyService, ILineOfBusinessService lobService, IClientService clientService) //IRepository<Opportunity> opportunityRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.companyService = companyService;
            this.lobService = lobService;
            this.clientService = clientService;
        }

        public async Task<ICollection<ApiResponse<bool>>> ImportOpportunities(ICollection<OpportunityViewModel> models, ApplicationUser? user)
        {
            var result = new List<ApiResponse<bool>>();
            foreach (var model in models)
            {
                //try
                // {
                var newOpportunity = new Opportunity
                {
                    ProjectName = model.ProjectName,
                    //LineOfBusinessId = model.LineOfBusinessId.Value,

                    Status = model.Status.Value,
                    Scope = model.Scope.Value,
                    Source = model.Source.Value,

                    // ClientStatus = model.ClientStatus.Value,
                    FirstContactDate = model.FirstContactDate,
                    Note = model.Note,


                    FirstProposalDate = model.FirstProposalDate,
                    FirstProposalValue = model.FirstProposalValue,
                    FirstProposalValueCurrency = model.FirstProposalValueCurrency,

                    CurrentProposalValue = model.CurrentProposalValue,
                    CurrentProposalValueCurrency = model.CurrentProposalValueCurrency,

                    ContractSignatureDate = model.ContractSignatureDate,
                    FinalContractValue = model.FinalContractValue,
                    FinalContractValueCurrency = model.FinalContractValueCurrency,
                    RetainerValidatity = model.RetainerValidatity,
                    //   Contact = model.Contact
                };
                var company = await companyService.GetCompanyByName(model.LineOfBusiness.Company.Name);
                var lob = await lobService.GetLOBByName(model.LineOfBusiness.Name, model.LineOfBusiness.Company.Name);
                var client = await clientService.GetClientByName(model.Client.Name);
                if (company == null)
                {
                    newOpportunity.LineOfBusiness = new LineOfBusiness()
                    {
                        Name = model.LineOfBusiness.Name,
                        Description = "Imported through excel",
                        ChangeSequenceNumber = 0,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        CreatorUserId = user.Id,
                        LastUpdateUserId = user.Id,
                        IsDeleted = false,
                        IsActive = true,
                        Company = new Company()
                        {
                            Name = model.LineOfBusiness.Company.Name,
                            Description = "Imported through excel",
                            OwnerId = "cc2b0840-0804-4ea1-ac39-b41575bf14d4",
                            ChangeSequenceNumber = 0,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            CreatorUserId = user.Id,
                            LastUpdateUserId = user.Id,
                            IsDeleted = false,
                            IsActive = true,
                        }
                    };
                }
                else if (lob == null)
                {
                    newOpportunity.LineOfBusiness = new LineOfBusiness()
                    {
                        Name = model.LineOfBusiness.Name,
                        Description = "Imported through excel",
                        ChangeSequenceNumber = 0,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        CreatorUserId = user.Id,
                        LastUpdateUserId = user.Id,
                        IsDeleted = false,
                        IsActive = true,
                        CompanyId = company.Id.Value,
                        // Company = new Company()
                        // {
                        //     //Id= company.Id.Value,
                        //     Name = company.Name,
                        //     Description = company.Description,
                        //     IsActive = true,
                        // }
                    };
                }
                else
                {
                    newOpportunity.LineOfBusiness = new LineOfBusiness()
                    {
                        Id = lob.Id.Value,
                        Name = lob.Name,
                        Description = lob.Description
                    };
                    newOpportunity.LineOfBusinessId = lob.Id.Value;
                    newOpportunity.LineOfBusiness = null;
                }
                if (client == null)
                {
                    // var lobs = new List<LineOfBusiness>();
                    //var lobs = await GetLineOfBusinessByIds(model.Client.LineOfBusinesses.Select(x => x.Id.Value).ToList());
                    //lobs.Add(lob);
                    newOpportunity.ClientStatus = 0;
                    newOpportunity.Client = new Client()
                    {
                        Name = model.Client.Name,
                        Description = "Imported through excel",
                        // LineOfBusinesses = new List<LineOfBusiness>(){
                        // new LineOfBusiness(){
                        //     Id = lob.Id.HasValue? lob.Id.Value:-1,
                        //     Name = lob.Name,
                        //     Description = lob.Description
                        // }
                        //},
                        ChangeSequenceNumber = 0,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        CreatorUserId = user.Id,
                        LastUpdateUserId = user.Id,
                        IsDeleted = false,
                        IsActive = true,
                    };
                    if (lob != null)
                    {
                        newOpportunity.Client.LineOfBusinesses = new List<LineOfBusiness>(){ new LineOfBusiness(){
                            Id = lob.Id.HasValue? lob.Id.Value:-1,
                            Name = lob.Name,
                          Description = lob.Description
                         }
                    };
                    }
                    else
                    {
                        newOpportunity.Client.LineOfBusinesses = new List<LineOfBusiness>(){
                            newOpportunity.LineOfBusiness
                        };
                    }
                    // lobs.Add(new LineOfBusiness()
                    // {
                    //     Id = model.LineOfBusinessId.Value,
                    //     // Name = model.LineOfBusiness.Name,
                    //     // Description = model.LineOfBusiness.Description,
                    //     // CompanyId = model.LineOfBusiness.CompanyId.Value,
                    //     // IsRetainer = model.LineOfBusiness.IsRetainer.Value,
                    //     // IsActive = model.LineOfBusiness.IsActive.Value,
                    //     // Clients = new List<Client>(){
                    //     //     newOpportunity.Client
                    //     // }
                    // });


                    // newOpportunity.Client.LineOfBusinesses = lobs;

                }
                else
                {
                    newOpportunity.ClientId = client.Id.Value;
                    newOpportunity.ClientStatus = 1;
                    newOpportunity.Client = null;
                }
                //dbContext.ChangeTracker.Clear();
                var opportunity = await new OpportunityRepository(dbContext).AddAsync(newOpportunity, user);
                //dbContext.Entry(opportunity).State = EntityState.Detached;

                if (opportunity == null)
                {
                    result.Add(new ApiResponse<bool>(false, "DB-EF Error in creating opportunity", false));
                }
                else
                {
                    result.Add(new ApiResponse<bool>(true, "Opportunity created successfully", true));
                }
                //  }
                // catch (Exception ex)
                // {
                //     result.Add(new ApiResponse<bool>(false, ex.Message, false));
                // }
            }


            return result;
        }
        public async Task<ApiResponse<bool>> AddOpportunity(OpportunityViewModel model, ApplicationUser? user)
        {
            var newOpportunity = new Opportunity
            {
                ProjectName = model.ProjectName,
                LineOfBusinessId = model.LineOfBusinessId.Value,

                Status = 0,
                Scope = model.Scope.Value,
                Source = model.Source.Value,

                ClientStatus = model.ClientStatus.Value,
                FirstContactDate = model.FirstContactDate,
                Note = model.Note
                //   Contact = model.Contact
            };
            if (model.ClientId == -2)
            {
                // var lobs = new List<LineOfBusiness>();
                var lobs = await GetLineOfBusinessByIds(model.Client.LineOfBusinesses.Select(x => x.Id.Value).ToList());
                //lobs.Add(lob);
                newOpportunity.Client = new Client()
                {
                    Name = model.Client.Name,
                    Description = model.Client.Description,
                    LineOfBusinesses = lobs,
                    ChangeSequenceNumber = 0,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatorUserId = user.Id,
                    LastUpdateUserId = user.Id,
                    IsDeleted = false,
                    IsActive = true,
                };
                // lobs.Add(new LineOfBusiness()
                // {
                //     Id = model.LineOfBusinessId.Value,
                //     // Name = model.LineOfBusiness.Name,
                //     // Description = model.LineOfBusiness.Description,
                //     // CompanyId = model.LineOfBusiness.CompanyId.Value,
                //     // IsRetainer = model.LineOfBusiness.IsRetainer.Value,
                //     // IsActive = model.LineOfBusiness.IsActive.Value,
                //     // Clients = new List<Client>(){
                //     //     newOpportunity.Client
                //     // }
                // });


                // newOpportunity.Client.LineOfBusinesses = lobs;

            }
            else
            {
                newOpportunity.ClientId = model.ClientId.Value;
            }
            var opportunity = await new OpportunityRepository(dbContext).AddAsync(newOpportunity, user);
            if (opportunity == null)
            {
                return new ApiResponse<bool>(false, "Error in creating opportunity", false);
            }

            return new ApiResponse<bool>(true, "Opportunity was created successfully", true);
        }

        public async Task<ICollection<LineOfBusiness>> GetLineOfBusinessByIds(List<int> lineOfBusinessIds)
        {
            return (await new LineOfBusinessRepository(dbContext).GetListAsync(c => lineOfBusinessIds.Contains(c.Id), null, p => p.Company, p => p.Clients)).ToList();
        }

        public async Task<ApiResponse<bool>> UpdateOpportunity(OpportunityViewModel updatedOpportunity, ApplicationUser? user)
        {
            var opportunity = await new OpportunityRepository(dbContext).GetAsync(a => a.Id == updatedOpportunity.Id);
            if (opportunity == null)
            {
                return new ApiResponse<bool>(false, "Opportunity not found", false);
            }

            opportunity.ClientId = updatedOpportunity.ClientId.Value;
            opportunity.ProjectName = updatedOpportunity.ProjectName;
            opportunity.ProjectId = updatedOpportunity.ProjectId;
            // opportunity.LineOfBusinessId = updatedOpportunity.LineOfBusinessId.Value;

            opportunity.Scope = updatedOpportunity.Scope.Value;
            opportunity.Source = updatedOpportunity.Source.Value;
            opportunity.Note = updatedOpportunity.Note;

            opportunity.ClientStatus = updatedOpportunity.ClientStatus.Value;
            opportunity.FirstContactDate = updatedOpportunity.FirstContactDate;

            opportunity.Status = updatedOpportunity.Status.Value;
            opportunity.FirstProposalDate = updatedOpportunity.FirstProposalDate;
            opportunity.FirstProposalValue = updatedOpportunity.FirstProposalValue;
            opportunity.FirstProposalValueCurrency = updatedOpportunity.FirstProposalValueCurrency;

            opportunity.CurrentProposalValue = updatedOpportunity.CurrentProposalValue;
            opportunity.CurrentProposalValueCurrency = updatedOpportunity.CurrentProposalValueCurrency;

            opportunity.ContractSignatureDate = updatedOpportunity.ContractSignatureDate;
            opportunity.FinalContractValue = updatedOpportunity.FinalContractValue;
            opportunity.FinalContractValueCurrency = updatedOpportunity.FinalContractValueCurrency;
            opportunity.RetainerValidatity = updatedOpportunity.RetainerValidatity;


            var isUpdated = await new OpportunityRepository(dbContext).UpdateAsync(opportunity, opportunity.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the opportunity was not updated", false);
            }

            return new ApiResponse<bool>(true, "Opportunity updated successfully", true);
        }

        public async Task<ApiResponse<bool>> CompleteOpportunity(OpportunityViewModel updatedOpportunity, ApplicationUser? user)
        {
            var opportunity = await new OpportunityRepository(dbContext).GetAsync(a => a.Id == updatedOpportunity.Id);
            if (opportunity == null)
            {
                return new ApiResponse<bool>(false, "Opportunity not found", false);
            }



            opportunity.Status = 6;



            var isUpdated = await new OpportunityRepository(dbContext).UpdateAsync(opportunity, opportunity.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error occurred! the opportunity was not updated", false);
            }

            return new ApiResponse<bool>(true, "Opportunity completed successfully", true);
        }

        public async Task<ApiResponse<bool>> DeleteOpportunity(OpportunityViewModel deleteOpportunity, ApplicationUser? user)
        {
            //var isDeleted = await new OpportunityRepository(dbContext).DeleteAsync(s => s.Id == opportunityId);
            // return new ApiResponse<bool>(isDeleted, isDeleted ? "opportunity was deleted successfully" : "Error in deleting opportunity", isDeleted);

            var opportunity = await new OpportunityRepository(dbContext).GetAsync(a => a.Id == deleteOpportunity.Id);
            if (opportunity == null)
            {
                return new ApiResponse<bool>(false, "Opportunity not found", false);
            }

            opportunity.IsDeleted = true;

            var isUpdated = await new OpportunityRepository(dbContext).UpdateAsync(opportunity, deleteOpportunity.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the opportunity was not deleted", false);
            }

            return new ApiResponse<bool>(true, "Opportunity deleted successfully", true);
        }

        // public Task<ApiResponse<bool>> ExportOpportunities(OpportunitySearchViewModel searchViewModel, string? userId, bool isOwner)
        // {
        //     var opportunityViewModelsList = await GetAllOpportunities(searchViewModel, userId, isOwner);



        //     return new ApiResponse<bool>(true, "Report downloaded successfully", true);

        // }

        public async Task<OpportunityViewModelsList> GetAllOpportunities(OpportunitySearchViewModel searchViewModel, string? userId, bool isOwner)
        {


            Paging<Opportunity> paging = null;

            if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
            {
                paging = new Paging<Opportunity>
                {
                    Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
                    OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
                    PageSize = searchViewModel.pageSize.Value
                };
            }


            Expression<Func<Opportunity, bool>> predicate1 = searchViewModel.status != null && searchViewModel.status != -1 ? x => x.Status == searchViewModel.status && x.IsDeleted == false : x => x.IsDeleted == false;

            Expression<Func<Opportunity, bool>> predicate2;
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
                    var intersectIds = lobs.Select(x => x.Id.Value);
                    if (searchViewModel.lineOfBusinessesIds != null && searchViewModel.lineOfBusinessesIds.Count > 0)
                    {
                        intersectIds = lobs.Select(x => x.Id.Value).Intersect(searchViewModel.lineOfBusinessesIds).ToList();
                    }
                    predicate2 = x => intersectIds.Contains(x.LineOfBusinessId);
                }
                else
                {
                    lobs = await lobService.GetLineOfBusinessesByUserId(userId);
                    var intersectIds = lobs.Select(x => x.Id.Value);
                    if (searchViewModel.lineOfBusinessesIds != null && searchViewModel.lineOfBusinessesIds.Count > 0)
                    {
                        intersectIds = lobs.Select(x => x.Id.Value).Intersect(searchViewModel.lineOfBusinessesIds).ToList();
                    }
                    predicate2 = x => intersectIds.Contains(x.LineOfBusinessId) && x.CreatorUserId == userId;
                }
            }
            else
            {
                predicate2 = searchViewModel.lineOfBusinessesIds != null && searchViewModel.lineOfBusinessesIds.Count > 0 ? x => searchViewModel.lineOfBusinessesIds.Contains(x.LineOfBusinessId) : x => 1 == 1;
            }
            ParameterExpression param = predicate1.Parameters[0];
            Expression<Func<Opportunity, bool>> predicate3 = Expression.Lambda<Func<Opportunity, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);

            Expression<Func<Opportunity, bool>> predicate4 = searchViewModel.fromDate != null && searchViewModel.toDate != null ? x => x.FirstContactDate >= DateTime.Parse(searchViewModel.fromDate) && x.FirstContactDate <= DateTime.Parse(searchViewModel.toDate) : x => 1 == 1;

            Expression<Func<Opportunity, bool>> predicate5 = Expression.Lambda<Func<Opportunity, bool>>(Expression.AndAlso(predicate3.Body, Expression.Invoke(predicate4, param)), param);

            var opportunityCount = (await new OpportunityRepository(dbContext).GetListAsync(predicate5, null)).Count();


            var opportunities = await new OpportunityRepository(dbContext).GetListAsync(predicate5, paging, p => p.LineOfBusiness, p => p.LineOfBusiness.Company, p => p.Client, p => p.Client.Files, p => p.Project, p => p.Project.ProjectMilestones, p => p.Project.ProjectMilestones.Select(y => y.Invoice));//, p => p.Project.ProjectMilestone);
            OpportunityViewModelsList model = new OpportunityViewModelsList()
            {
                TotalCount = opportunityCount,
                Opportunities = getOpportunitiesViewModel(opportunities.ToList())
            };

            return model;
        }

        private ICollection<OpportunityViewModel> getOpportunitiesViewModel(ICollection<Opportunity> opportunities)
        {
            var opportunitiesViewModel = new List<OpportunityViewModel>();
            foreach (var opportunity in opportunities)
            {
                var opportunityViewModel = new OpportunityViewModel()
                {
                    Id = opportunity.Id,
                    ProjectName = opportunity.ProjectName,
                    ProjectId = opportunity.ProjectId,
                    ClientId = opportunity.ClientId,
                    ClientStatus = opportunity.ClientStatus,
                    FirstContactDate = opportunity.FirstContactDate,
                    FirstProposalDate = opportunity.FirstProposalDate,
                    FirstProposalValue = opportunity.FirstProposalValue,
                    FirstProposalValueCurrency = opportunity.FirstProposalValueCurrency,
                    FinalContractValue = opportunity.FinalContractValue,
                    FinalContractValueCurrency = opportunity.FinalContractValueCurrency,
                    CurrentProposalValue = opportunity.CurrentProposalValue,
                    CurrentProposalValueCurrency = opportunity.CurrentProposalValueCurrency,
                    ContractSignatureDate = opportunity.ContractSignatureDate,
                    Note = opportunity.Note,
                    RetainerValidatity = opportunity.RetainerValidatity,
                    Status = opportunity.Status,
                    Scope = opportunity.Scope,
                    Source = opportunity.Source,
                    Client = new ClientViewModel()
                    {
                        Id = opportunity.Client.Id,
                        Name = opportunity.Client.Name,
                        Description = opportunity.Client.Description,
                        ContactPerson = opportunity.Client.ContactPerson,
                        Email = opportunity.Client.Email,
                        ContactNumber = opportunity.Client.ContactNumber,
                        AddressLine1 = opportunity.Client.AddressLine1,
                        AddressLine2 = opportunity.Client.AddressLine2,
                        PostCode = opportunity.Client.PostCode,
                        Country = opportunity.Client.Country,
                        State = opportunity.Client.State,
                        City = opportunity.Client.City,
                        CommercialRegistrationNumber = opportunity.Client.CommercialRegistrationNumber,
                        TaxCardNumber = opportunity.Client.TaxCardNumber,
                        Files = opportunity.Client.Files.Select(x => new FileViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Url = x.Url,
                            Status = x.Status
                        }).ToList(),
                    },
                    LineOfBusinessId = opportunity.LineOfBusinessId,
                    LineOfBusiness = new LineOfBusinessViewModel()
                    {
                        Name = opportunity.LineOfBusiness.Name,
                        Id = opportunity.LineOfBusiness.Id,
                        Company = new CompanyViewModel()
                        {
                            Id = opportunity.LineOfBusiness.Company.Id,
                            Name = opportunity.LineOfBusiness.Company.Name,
                        }
                    }

                };
                opportunitiesViewModel.Add(opportunityViewModel);
            }
            return opportunitiesViewModel;
        }



        public async Task<OpportunityViewModel> GetOpportunityById(OpportunityViewModel oppViewModel)
        {
            var opportunity = await new OpportunityRepository(dbContext).GetAsync(c => c.Id == oppViewModel.Id, null, false, p => p.LineOfBusiness, p => p.LineOfBusiness.Company, p => p.Client, p => p.Client.Files, p => p.Project, p => p.Project.ProjectMilestones, p => p.Project.ProjectMilestones.Select(y => y.Invoice));
            if (opportunity != null)
            {

                OpportunityViewModel opportunityViewModel = new OpportunityViewModel();
                // {
                opportunityViewModel.Id = opportunity.Id;
                opportunityViewModel.ProjectName = opportunity.ProjectName;
                opportunityViewModel.ProjectId = opportunity.ProjectId;
                opportunityViewModel.ClientId = opportunity.ClientId;
                opportunityViewModel.ClientStatus = opportunity.ClientStatus;
                opportunityViewModel.FirstContactDate = opportunity.FirstContactDate;
                opportunityViewModel.FirstProposalDate = opportunity.FirstProposalDate;
                opportunityViewModel.FirstProposalValue = opportunity.FirstProposalValue;
                opportunityViewModel.FirstProposalValueCurrency = opportunity.FirstProposalValueCurrency;
                opportunityViewModel.FinalContractValue = opportunity.FinalContractValue;
                opportunityViewModel.FinalContractValueCurrency = opportunity.FinalContractValueCurrency;
                opportunityViewModel.CurrentProposalValue = opportunity.CurrentProposalValue;
                opportunityViewModel.CurrentProposalValueCurrency = opportunity.CurrentProposalValueCurrency;
                opportunityViewModel.ContractSignatureDate = opportunity.ContractSignatureDate;
                opportunityViewModel.Note = opportunity.Note;
                opportunityViewModel.RetainerValidatity = opportunity.RetainerValidatity;
                opportunityViewModel.Status = opportunity.Status;
                opportunityViewModel.Scope = opportunity.Scope;
                opportunityViewModel.Source = opportunity.Source;
                opportunityViewModel.Client = new ClientViewModel()
                {
                    Id = opportunity.Client.Id,
                    Name = opportunity.Client.Name,
                    Description = opportunity.Client.Description,
                    ContactPerson = opportunity.Client.ContactPerson,
                    Email = opportunity.Client.Email,
                    ContactNumber = opportunity.Client.ContactNumber,
                    AddressLine1 = opportunity.Client.AddressLine1,
                    AddressLine2 = opportunity.Client.AddressLine2,
                    PostCode = opportunity.Client.PostCode,
                    Country = opportunity.Client.Country,
                    State = opportunity.Client.State,
                    City = opportunity.Client.City,
                    CommercialRegistrationNumber = opportunity.Client.CommercialRegistrationNumber,
                    TaxCardNumber = opportunity.Client.TaxCardNumber,
                    Files = opportunity.Client.Files.Select(x => new FileViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Url = x.Url,
                        Status = x.Status
                    }).ToList(),
                };
                opportunityViewModel.LineOfBusinessId = opportunity.LineOfBusinessId;
                opportunityViewModel.LineOfBusiness = new LineOfBusinessViewModel()
                {
                    Name = opportunity.LineOfBusiness.Name,
                    Id = opportunity.LineOfBusiness.Id,
                    Company = new CompanyViewModel()
                    {
                        Id = opportunity.LineOfBusiness.Company.Id,
                        Name = opportunity.LineOfBusiness.Company.Name,
                    }
                };
                // };
                return opportunityViewModel;
            }
            else
            {
                return null;
            }

        }


        public async Task<Opportunity> GetOpportunityByProjecName(string projectName)
        {
            var opportunity = await new OpportunityRepository(dbContext).GetAsync(c => c.ProjectName == projectName, null, false, p => p.LineOfBusiness, p => p.LineOfBusiness.Company, p => p.Client, p => p.Client.Files, p => p.Project, p => p.Project.ProjectMilestones, p => p.Project.ProjectMilestones.Select(y => y.Invoice));
            return opportunity;
            // if (opportunity != null)
            // {

            //     OpportunityViewModel opportunityViewModel = new OpportunityViewModel();
            //     // {
            //     opportunityViewModel.Id = opportunity.Id;
            //     opportunityViewModel.ProjectName = opportunity.ProjectName;
            //     opportunityViewModel.ProjectId = opportunity.ProjectId;
            //     opportunityViewModel.ClientId = opportunity.ClientId;
            //     opportunityViewModel.ClientStatus = opportunity.ClientStatus;
            //     opportunityViewModel.FirstContactDate = opportunity.FirstContactDate;
            //     opportunityViewModel.FirstProposalDate = opportunity.FirstProposalDate;
            //     opportunityViewModel.FirstProposalValue = opportunity.FirstProposalValue;
            //     opportunityViewModel.FirstProposalValueCurrency = opportunity.FirstProposalValueCurrency;
            //     opportunityViewModel.FinalContractValue = opportunity.FinalContractValue;
            //     opportunityViewModel.FinalContractValueCurrency = opportunity.FinalContractValueCurrency;
            //     opportunityViewModel.CurrentProposalValue = opportunity.CurrentProposalValue;
            //     opportunityViewModel.CurrentProposalValueCurrency = opportunity.CurrentProposalValueCurrency;
            //     opportunityViewModel.ContractSignatureDate = opportunity.ContractSignatureDate;
            //     opportunityViewModel.Note = opportunity.Note;
            //     opportunityViewModel.RetainerValidatity = opportunity.RetainerValidatity;
            //     opportunityViewModel.Status = opportunity.Status;
            //     opportunityViewModel.Scope = opportunity.Scope;
            //     opportunityViewModel.Source = opportunity.Source;
            //     opportunityViewModel.Client = new ClientViewModel()
            //     {
            //         Id = opportunity.Client.Id,
            //         Name = opportunity.Client.Name,
            //         Description = opportunity.Client.Description,
            //         ContactPerson = opportunity.Client.ContactPerson,
            //         Email = opportunity.Client.Email,
            //         ContactNumber = opportunity.Client.ContactNumber,
            //         AddressLine1 = opportunity.Client.AddressLine1,
            //         AddressLine2 = opportunity.Client.AddressLine2,
            //         PostCode = opportunity.Client.PostCode,
            //         Country = opportunity.Client.Country,
            //         State = opportunity.Client.State,
            //         City = opportunity.Client.City,
            //         CommercialRegistrationNumber = opportunity.Client.CommercialRegistrationNumber,
            //         TaxCardNumber = opportunity.Client.TaxCardNumber,
            //         Files = opportunity.Client.Files.Select(x => new FileViewModel()
            //         {
            //             Id = x.Id,
            //             Name = x.Name,
            //             Url = x.Url,
            //             Status = x.Status
            //         }).ToList(),
            //     };
            //     opportunityViewModel.LineOfBusinessId = opportunity.LineOfBusinessId;
            //     opportunityViewModel.LineOfBusiness = new LineOfBusinessViewModel()
            //     {
            //         Name = opportunity.LineOfBusiness.Name,
            //         Id = opportunity.LineOfBusiness.Id,
            //         Company = new CompanyViewModel()
            //         {
            //             Id = opportunity.LineOfBusiness.Company.Id,
            //             Name = opportunity.LineOfBusiness.Company.Name,
            //         }
            //     };
            //     // };
            //     return opportunityViewModel;
            // }
            // else
            // {
            //     return null;
            // }

        }

        public async Task<ICollection<OpportunityViewModel>> GetOpportunitiesByIds(ICollection<int> opportunityIds)
        {
            var opportunities = await new OpportunityRepository(dbContext).GetListAsync(c => opportunityIds.Contains(c.Id), null);
            var opportunitiesViewModel = new List<OpportunityViewModel>();
            if (opportunities != null)
            {
                foreach (var opportunity in opportunities)
                {
                    opportunitiesViewModel.Add(mapper.Map<OpportunityViewModel>(opportunity));
                }
                return opportunitiesViewModel;
            }
            else
            {
                return null;
            }

        }



    }
}