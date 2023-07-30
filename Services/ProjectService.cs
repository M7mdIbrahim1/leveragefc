using Backend.Data;
using Backend.Models;
using Backend.Models.ProjectViewModels;
using Backend.Helper;
using Backend.Repositories;
using System.Linq.Expressions;
using AutoMapper;
using Backend.Auth;
using Backend.Models.CompanyViewModels;
using Backend.Models.ClientViewModels;
using Backend.Models.MilestoneViewModels;
using Backend.Models.OpportunityViewModels;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.FileViewModels;


namespace Backend.Services
{
    public class ProjectService : IProjectService
    {

        private readonly IMapper mapper;


        private readonly ApplicationDbContext dbContext;
        private readonly ICompanyService companyService;

        private readonly ILineOfBusinessService lobService;

        private readonly IOpportunityService opportunityService;

        private readonly IClientService clientService;

        public ProjectService(ApplicationDbContext dbContext, IMapper mapper, ICompanyService companyService, ILineOfBusinessService lobService, IOpportunityService opportunityService, IClientService clientService) //IRepository<Project> projectRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.companyService = companyService;
            this.lobService = lobService;
            this.opportunityService = opportunityService;
            this.clientService = clientService;
        }
        public async Task<ApiResponse<bool>> AddProject(ProjectViewModel model, ApplicationUser? user)
        {
            var project = new Project
            {
                ClientId = model.ClientId.Value,
                ProjectName = model.ProjectName,
                LineOfBusinessId = model.LineOfBusinessId.Value,

                Status = 0,
                Scope = model.Scope.Value,

                CompletionDateScheduled = model.CompletionDateScheduled,
                //  ClientApprovalDate = model.ClientApprovalDate,
                KickOffDateScheduled = model.KickOffDateScheduled,
                ProjectMilestones = model.ProjectMilestones.OrderBy(x => x.MilestoneIndex).Select((x, i) => new ProjectMilestone()
                {
                    Name = x.Name,
                    NeedPayment = x.NeedPayment.Value,
                    Description = x.Description,
                    MilestoneId = x.MilestoneId.Value,
                    Note = x.Note,
                    PaymentValue = x.PaymentValue,
                    PaymentValueCurrency = x.PaymentValueCurrency,
                    DateScheduled = x.DateScheduled,
                    Status = 0,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsDeleted = false,
                    IsActive = true,
                    ChangeSequenceNumber = 0,
                    CreatorUserId = user != null ? user.Id : null,
                    LastUpdateUserId = user != null ? user.Id : null,
                    MilestoneIndex = i + 1,
                    Start = i == 0,
                    End = i == model.ProjectMilestones.Count - 1
                }).ToList(),
                ContractSignatureDate = model.ContractSignatureDate,
                ContractValue = model.ContractValue,
                ContractValueCurrency = model.ContractValueCurrency,
                RetainerValidatity = model.RetainerValidatity,
                OpportunityId = model.OpportunityId.Value,
                Note = model.Note,
                CurrentProjectMilestoneIndex = 1,
                MilestoneCount = model.ProjectMilestones.Count


                //   Contact = model.Contact
            };
            // project.ProjectMilestone = project.ProjectMilestones.First();
            var addProject = await new ProjectRepository(dbContext).AddAsync(project, user);
            if (addProject == null)
            {
                return new ApiResponse<bool>(false, "Error in creating project", false);
            }

            var updateClient = await clientService.UpdateClient(new ClientViewModel()
            {
                Id = model.ClientId,
                AddressLine1 = model.Client.AddressLine1,
                AddressLine2 = model.Client.AddressLine2,
                City = model.Client.City,
                State = model.Client.State,
                Country = model.Client.Country,
                PostCode = model.Client.PostCode,
                Email = model.Client.Email,
                ContactNumber = model.Client.ContactNumber,
                ContactPerson = model.Client.ContactPerson,
                CommercialRegistrationNumber = model.Client.CommercialRegistrationNumber,
                TaxCardNumber = model.Client.TaxCardNumber,
                //CommercialRegistrationNumberPath = model.Client.CommercialRegistrationNumberPath,
                //TaxCardNumberPath = model.Client.TaxCardNumberPath,
                Files = model.Client.Files
            }, user);
            if (updateClient != new ApiResponse<bool>(true, "Client updated successfully", true))
            {
                return new ApiResponse<bool>(false, "Error in updating client, but project created successfully", false);
            }


            var updateOpportunity = await opportunityService.CompleteOpportunity(new OpportunityViewModel() { Id = model.OpportunityId }, user);
            if (updateOpportunity != new ApiResponse<bool>(true, "Opportunity completed successfully", true))
            {
                return new ApiResponse<bool>(false, "Error in completing opportunity, but project created successfully", false);
            }

            return new ApiResponse<bool>(true, "Project was created successfully", true);
        }

        public async Task<ApiResponse<bool>> UpdateProject(ProjectViewModel updatedProject, ApplicationUser? user)
        {
            var project = await new ProjectRepository(dbContext).GetAsync(a => a.Id == updatedProject.Id, null, false, p => p.ProjectMilestones);
            if (project == null)
            {
                return new ApiResponse<bool>(false, "Project not found", false);
            }

            project.ClientId = updatedProject.ClientId.Value;
            project.ProjectName = updatedProject.ProjectName;
            // project.LineOfBusinessId = updatedProject.LineOfBusinessId.Value;

            project.Status = updatedProject.Status.Value;
            project.Scope = updatedProject.Scope.Value;

            project.CompletionDateScheduled = updatedProject.CompletionDateScheduled;
            project.CompletionDateActual = updatedProject.CompletionDateActual;
            // project.ClientApprovalDate = updatedProject.ClientApprovalDate;
            project.KickOffDateScheduled = updatedProject.KickOffDateScheduled;
            project.KickOffDateActual = updatedProject.KickOffDateActual;
            project.ContractSignatureDate = updatedProject.ContractSignatureDate;
            project.ContractValue = updatedProject.ContractValue;
            project.ContractValueCurrency = updatedProject.ContractValueCurrency;
            project.RetainerValidatity = updatedProject.RetainerValidatity;
            // project.OpportunityId = updatedProject.OpportunityId.Value;
            project.Note = updatedProject.Note;
            // project.ProjectMilestoneId = updatedProject.ProjectMilestoneId.Value;

            foreach (var projectMilestone in project.ProjectMilestones)
            {
                var updatedProjectMilestone = updatedProject.ProjectMilestones.First(x => x.Id == projectMilestone.Id);
                if (updatedProjectMilestone != null)
                {
                    projectMilestone.Name = updatedProjectMilestone.Name;
                    projectMilestone.NeedPayment = updatedProjectMilestone.NeedPayment.Value;
                    projectMilestone.Description = updatedProjectMilestone.Description;
                    // projectMilestone.MilestoneId = updatedProjectMilestone.MilestoneId.Value;
                    projectMilestone.Note = updatedProjectMilestone.Note;
                    projectMilestone.PaymentValue = updatedProjectMilestone.PaymentValue;
                    projectMilestone.PaymentValueCurrency = updatedProjectMilestone.PaymentValueCurrency;
                    projectMilestone.DateScheduled = updatedProjectMilestone.DateScheduled;
                    projectMilestone.DateActual = updatedProjectMilestone.DateActual;
                    projectMilestone.Status = updatedProjectMilestone.Status.Value;
                    projectMilestone.UpdatedDate = DateTime.Now;
                    projectMilestone.ChangeSequenceNumber += 1;
                    projectMilestone.LastUpdateUserId = user != null ? user.Id : null;
                }
            }
            //starting first milestone when project is just started and set project to inprogress
            if (updatedProject.Status == 1 && updatedProject.ProjectMilestones.All(x => x.Status == 0))
            {
                project.ProjectMilestones.First().Status = 1;
                project.Status = 2;
            }
            //starting next milestone after last completed one
            else if (updatedProject.Status == 2)
            {
                var lastCompleted = updatedProject.ProjectMilestones.Last(x => x.Status == 2);
                if (lastCompleted != null)
                {
                    var index = updatedProject.ProjectMilestones.ToList().IndexOf(lastCompleted);
                    if (index < project.ProjectMilestones.Count - 1)
                    {
                        var coming = project.ProjectMilestones.ToList()[index + 1];
                        if (coming.Status == 0)
                        {
                            coming.Status = 1;
                            project.CurrentProjectMilestoneId = coming.Id;
                            project.CurrentProjectMilestoneIndex = coming.MilestoneIndex;
                        }
                    }
                    //if you are completing last milestone this shall complete the project
                    else if (index == project.ProjectMilestones.Count - 1)
                    {
                        if (project.Status == 2)
                        {
                            project.Status = 3;
                        }
                    }
                }
            }
            // else if (updatedProject.Status == 3)
            // {

            // }




            var isUpdated = await new ProjectRepository(dbContext).UpdateAsync(project, project.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the project was not updated", false);
            }

            return new ApiResponse<bool>(true, "Project updated successfully", true);
        }

        public async Task<ApiResponse<bool>> DeleteProject(ProjectViewModel deleteProject, ApplicationUser? user)
        {
            //var isDeleted = await new ProjectRepository(dbContext).DeleteAsync(s => s.Id == projectId);
            // return new ApiResponse<bool>(isDeleted, isDeleted ? "project was deleted successfully" : "Error in deleting project", isDeleted);

            var project = await new ProjectRepository(dbContext).GetAsync(a => a.Id == deleteProject.Id);
            if (project == null)
            {
                return new ApiResponse<bool>(false, "Project not found", false);
            }

            project.IsDeleted = true;

            var isUpdated = await new ProjectRepository(dbContext).UpdateAsync(project, deleteProject.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the project was not deleted", false);
            }

            return new ApiResponse<bool>(true, "Project deleted successfully", true);
        }

        // public async Task<ApiResponse<bool>> ExportProjects(ProjectSearchViewModel searchViewModel, string? userId, bool isOwner)
        // {
        //     var ProjectViewModelsList = await GetAllProjects(searchViewModel, userId, isOwner);

        //     return new ApiResponse<bool>(true, "Report downloaded successfully", true);

        // }

        public async Task<ProjectViewModelsList> GetAllProjects(ProjectSearchViewModel searchViewModel, string? userId, bool isOwner)
        {


            Paging<Project> paging = null;

            if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
            {
                paging = new Paging<Project>
                {
                    Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
                    OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
                    PageSize = searchViewModel.pageSize.Value
                };
            }


            Expression<Func<Project, bool>> predicate1 = searchViewModel.status != null && searchViewModel.status != -1 ? x => x.Status == searchViewModel.status && x.IsDeleted == false : x => x.IsDeleted == false;

            Expression<Func<Project, bool>> predicate2;
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
            Expression<Func<Project, bool>> predicate3 = Expression.Lambda<Func<Project, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);


            Expression<Func<Project, bool>> predicate4 = searchViewModel.fromDate != null && searchViewModel.toDate != null ? x => x.ContractSignatureDate >= DateTime.Parse(searchViewModel.fromDate) && x.ContractSignatureDate <= DateTime.Parse(searchViewModel.toDate) : x => 1 == 1;

            Expression<Func<Project, bool>> predicate5 = Expression.Lambda<Func<Project, bool>>(Expression.AndAlso(predicate3.Body, Expression.Invoke(predicate4, param)), param);

            var projectCount = (await new ProjectRepository(dbContext).GetListAsync(predicate5, null)).Count();


            var projects = await new ProjectRepository(dbContext).GetListAsync(predicate5, paging, p => p.LineOfBusiness, p => p.LineOfBusiness.Company, p => p.LineOfBusiness.Milestones, p => p.Client, p => p.Client.Files, p => p.Opportunity, p => p.ProjectMilestones, p => p.ProjectMilestones.Select(y => y.Invoice));//, p => p.ProjectMilestone);
            ProjectViewModelsList model = new ProjectViewModelsList()
            {
                TotalCount = projectCount,
                Projects = getProjectsViewModel(projects.ToList())
            };

            return model;
        }

        private ICollection<ProjectViewModel> getProjectsViewModel(ICollection<Project> projects)
        {
            var projectsViewModel = new List<ProjectViewModel>();
            foreach (var project in projects)
            {
                var projectViewModel = new ProjectViewModel()
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    OpportunityId = project.OpportunityId,
                    ClientId = project.ClientId,
                    KickOffDateScheduled = project.KickOffDateScheduled,
                    KickOffDateActual = project.KickOffDateActual,
                    // ClientApprovalDate = project.ClientApprovalDate,
                    CompletionDateActual = project.CompletionDateActual,
                    CompletionDateScheduled = project.CompletionDateScheduled,
                    ContractValue = project.ContractValue,
                    ContractValueCurrency = project.ContractValueCurrency,
                    ContractSignatureDate = project.ContractSignatureDate,
                    Note = project.Note,
                    RetainerValidatity = project.RetainerValidatity,
                    Status = project.Status,
                    Scope = project.Scope,
                    Client = new ClientViewModel()
                    {
                        Id = project.Client.Id,
                        Name = project.Client.Name,
                        Description = project.Client.Description,
                        AddressLine1 = project.Client.AddressLine1,
                        AddressLine2 = project.Client.AddressLine2,
                        City = project.Client.City,
                        State = project.Client.State,
                        Country = project.Client.Country,
                        PostCode = project.Client.PostCode,
                        Email = project.Client.Email,
                        ContactNumber = project.Client.ContactNumber,
                        ContactPerson = project.Client.ContactPerson,
                        CommercialRegistrationNumber = project.Client.CommercialRegistrationNumber,
                        TaxCardNumber = project.Client.TaxCardNumber,
                        //CommercialRegistrationNumberPath = project.Client.CommercialRegistrationNumberPath,
                        //TaxCardNumberPath = project.Client.TaxCardNumberPath,
                        Files = project.Client.Files.Select(x => new FileViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Url = x.Url,
                            Status = x.Status
                        }).ToList()
                    },
                    LineOfBusinessId = project.LineOfBusinessId,
                    LineOfBusiness = new LineOfBusinessViewModel()
                    {
                        Name = project.LineOfBusiness.Name,
                        Id = project.LineOfBusiness.Id,
                        Company = new CompanyViewModel()
                        {
                            Id = project.LineOfBusiness.Company.Id,
                            Name = project.LineOfBusiness.Company.Name,
                        },
                        Milestones = project.LineOfBusiness.Milestones.OrderBy(x => x.Index).Select(x => new MilestoneViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            NeedPayment = x.NeedPayment,
                        }).ToList()
                    },
                    CurrentProjectMilestoneId = project.CurrentProjectMilestoneId,
                    ProjectMilestones = project.ProjectMilestones.OrderBy(x => x.MilestoneIndex).Select(x => new ProjectMilestoneViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NeedPayment = x.NeedPayment,
                        PaymentValue = x.PaymentValue,
                        PaymentValueCurrency = x.PaymentValueCurrency,
                        DateActual = x.DateActual,
                        DateScheduled = x.DateScheduled,
                        Note = x.Note,
                        Status = x.Status
                    }).ToList(),
                    Opportunity = new OpportunityViewModel()
                    {
                        Id = project.OpportunityId,
                        ClientStatus = project.Opportunity.ClientStatus
                    }

                };
                projectsViewModel.Add(projectViewModel);
            }
            return projectsViewModel;
        }




        public async Task<ProjectViewModel> GetProjectById(ProjectViewModel model)
        {
            var project = await new ProjectRepository(dbContext).GetAsync(c => c.Id == model.Id, null, false, p => p.LineOfBusiness, p => p.LineOfBusiness.Company, p => p.LineOfBusiness.Milestones, p => p.Client, p => p.Client.Files, p => p.ProjectMilestones, p => p.ProjectMilestones.Select(y => y.Invoice), p => p.Opportunity);
            if (project != null)
            {

                var projectViewModel = new ProjectViewModel()
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    OpportunityId = project.OpportunityId,
                    ClientId = project.ClientId,
                    KickOffDateScheduled = project.KickOffDateScheduled,
                    KickOffDateActual = project.KickOffDateActual,
                    CompletionDateActual = project.CompletionDateActual,
                    CompletionDateScheduled = project.CompletionDateScheduled,
                    ContractValue = project.ContractValue,
                    ContractValueCurrency = project.ContractValueCurrency,
                    ContractSignatureDate = project.ContractSignatureDate,
                    Note = project.Note,
                    RetainerValidatity = project.RetainerValidatity,
                    Status = project.Status,
                    Scope = project.Scope,
                    Client = new ClientViewModel()
                    {
                        Id = project.Client.Id,
                        Name = project.Client.Name,
                        Description = project.Client.Description,
                        AddressLine1 = project.Client.AddressLine1,
                        AddressLine2 = project.Client.AddressLine2,
                        City = project.Client.City,
                        State = project.Client.State,
                        Country = project.Client.Country,
                        PostCode = project.Client.PostCode,
                        Email = project.Client.Email,
                        ContactNumber = project.Client.ContactNumber,
                        ContactPerson = project.Client.ContactPerson,
                        CommercialRegistrationNumber = project.Client.CommercialRegistrationNumber,
                        TaxCardNumber = project.Client.TaxCardNumber,
                        // CommercialRegistrationNumberPath = project.Client.CommercialRegistrationNumberPath,
                        // TaxCardNumberPath = project.Client.TaxCardNumberPath,
                        Files = project.Client.Files.Select(x => new FileViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Url = x.Url,
                            Status = x.Status
                        }).ToList()
                    },
                    LineOfBusinessId = project.LineOfBusinessId,
                    LineOfBusiness = new LineOfBusinessViewModel()
                    {
                        Name = project.LineOfBusiness.Name,
                        Id = project.LineOfBusiness.Id,
                        Company = new CompanyViewModel()
                        {
                            Id = project.LineOfBusiness.Company.Id,
                            Name = project.LineOfBusiness.Company.Name,
                        }
                        ,
                        Milestones = project.LineOfBusiness.Milestones.OrderBy(x => x.Index).Select(x => new MilestoneViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            NeedPayment = x.NeedPayment,
                        }).ToList()
                    },
                    CurrentProjectMilestoneId = project.CurrentProjectMilestoneId,
                    ProjectMilestones = project.ProjectMilestones.OrderBy(x => x.MilestoneIndex).Select(x => new ProjectMilestoneViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NeedPayment = x.NeedPayment,
                        PaymentValue = x.PaymentValue,
                        PaymentValueCurrency = x.PaymentValueCurrency,
                        DateActual = x.DateActual,
                        DateScheduled = x.DateScheduled,
                        Note = x.Note,
                        Status = x.Status,
                        MilestoneIndex = x.MilestoneIndex,
                        Start = x.Start,
                        End = x.End
                    }).ToList(),
                    Opportunity = new OpportunityViewModel()
                    {
                        Id = project.OpportunityId,
                        ClientStatus = project.Opportunity.ClientStatus
                    }

                };
                return projectViewModel;
            }
            else
            {
                return null;
            }

        }

        public async Task<ProjectViewModel> GetProjectByOpportunityId(ProjectViewModel model)
        {
            var project = await new ProjectRepository(dbContext).GetAsync(c => c.OpportunityId == model.OpportunityId, null, false, p => p.LineOfBusiness, p => p.LineOfBusiness.Company, p => p.Client, p => p.Client.Files, p => p.ProjectMilestones, p => p.ProjectMilestones.Select(y => y.Invoice), p => p.Opportunity);
            if (project != null)
            {

                var projectViewModel = new ProjectViewModel()
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    OpportunityId = project.OpportunityId,
                    ClientId = project.ClientId,
                    KickOffDateScheduled = project.KickOffDateScheduled,
                    KickOffDateActual = project.KickOffDateActual,
                    CompletionDateActual = project.CompletionDateActual,
                    CompletionDateScheduled = project.CompletionDateScheduled,
                    ContractValue = project.ContractValue,
                    ContractValueCurrency = project.ContractValueCurrency,
                    ContractSignatureDate = project.ContractSignatureDate,
                    Note = project.Note,
                    RetainerValidatity = project.RetainerValidatity,
                    Status = project.Status,
                    Scope = project.Scope,
                    Client = new ClientViewModel()
                    {
                        Id = project.Client.Id,
                        Name = project.Client.Name,
                        Description = project.Client.Description,
                        AddressLine1 = project.Client.AddressLine1,
                        AddressLine2 = project.Client.AddressLine2,
                        City = project.Client.City,
                        State = project.Client.State,
                        Country = project.Client.Country,
                        PostCode = project.Client.PostCode,
                        Email = project.Client.Email,
                        ContactNumber = project.Client.ContactNumber,
                        ContactPerson = project.Client.ContactPerson,
                        CommercialRegistrationNumber = project.Client.CommercialRegistrationNumber,
                        TaxCardNumber = project.Client.TaxCardNumber,
                        // CommercialRegistrationNumberPath = project.Client.CommercialRegistrationNumberPath,
                        //  TaxCardNumberPath = project.Client.TaxCardNumberPath,
                        Files = project.Client.Files.Select(x => new FileViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Url = x.Url,
                            Status = x.Status
                        }).ToList()
                    },
                    LineOfBusinessId = project.LineOfBusinessId,
                    LineOfBusiness = new LineOfBusinessViewModel()
                    {
                        Name = project.LineOfBusiness.Name,
                        Id = project.LineOfBusiness.Id,
                        Company = new CompanyViewModel()
                        {
                            Id = project.LineOfBusiness.Company.Id,
                            Name = project.LineOfBusiness.Company.Name,
                        },
                        Milestones = project.LineOfBusiness.Milestones.OrderBy(x => x.Index).Select(x => new MilestoneViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            NeedPayment = x.NeedPayment,
                        }).ToList()

                    },
                    CurrentProjectMilestoneId = project.CurrentProjectMilestoneId,
                    ProjectMilestones = project.ProjectMilestones.OrderBy(x => x.MilestoneIndex).Select(x => new ProjectMilestoneViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NeedPayment = x.NeedPayment,
                        PaymentValue = x.PaymentValue,
                        PaymentValueCurrency = x.PaymentValueCurrency,
                        DateActual = x.DateActual,
                        DateScheduled = x.DateScheduled,
                        Note = x.Note,
                        Status = x.Status,
                        MilestoneIndex = x.MilestoneIndex,
                        Start = x.Start,
                        End = x.End
                    }).ToList(),
                    Opportunity = new OpportunityViewModel()
                    {
                        Id = project.OpportunityId,
                        ClientStatus = project.Opportunity.ClientStatus
                    }

                };
                return projectViewModel;
            }
            else
            {
                return null;
            }

        }



        public async Task<ICollection<ProjectViewModel>> GetProjectsByIds(ICollection<int> projectIds)
        {
            var projects = await new ProjectRepository(dbContext).GetListAsync(c => projectIds.Contains(c.Id), null);
            var projectsViewModel = new List<ProjectViewModel>();
            if (projects != null)
            {
                foreach (var project in projects)
                {
                    projectsViewModel.Add(mapper.Map<ProjectViewModel>(project));
                }
                return projectsViewModel;
            }
            else
            {
                return null;
            }

        }



        public async Task<ICollection<bool>> ImportProjects(ICollection<ProjectViewModel> models, ApplicationUser? user)
        {
            var result = new List<bool>();
            foreach (var model in models)
            {
                // try
                // {
                var lob = await lobService.GetLOBByName(model.LineOfBusiness.Name, model.LineOfBusiness.Company.Name);
                var client = await clientService.GetClientByName(model.Client.Name, lob.Id.Value);
                if (lob != null && client != null)
                {
                    var newProject = new Project
                    {
                        ProjectName = model.ProjectName,
                        //LineOfBusinessId = model.LineOfBusinessId.Value,

                        Status = model.Status.Value,
                        Scope = model.Scope.Value,




                        CompletionDateScheduled = model.CompletionDateScheduled,
                        CompletionDateActual = model.CompletionDateActual,
                        // project.ClientApprovalDate = updatedProject.ClientApprovalDate;
                        KickOffDateScheduled = model.KickOffDateScheduled,
                        KickOffDateActual = model.KickOffDateActual,
                        ContractSignatureDate = model.ContractSignatureDate,
                        ContractValue = model.ContractValue,
                        ContractValueCurrency = model.ContractValueCurrency,
                        RetainerValidatity = model.RetainerValidatity,
                        //OpportunityId = model.OpportunityId.Value;
                        //Opportunity = GetOpportunity(model),
                        Note = model.Note,
                        LineOfBusinessId = lob.Id.Value,
                        ClientId = client.Id.Value,
                        CurrentProjectMilestoneIndex = model.CurrentProjectMilestoneIndex,
                        MilestoneCount = model.ProjectMilestones != null ? model.ProjectMilestones.Count : 0,
                        // ProjectMilestoneId = model.ProjectMilestoneId.Value;


                    };



                    // var company = await companyService.GetCompanyByName(model.LineOfBusiness.Company.Name);

                    // if (company == null)
                    // {
                    //     newProject.LineOfBusiness = new LineOfBusiness()
                    //     {
                    //         Name = model.LineOfBusiness.Name,
                    //         Description = "Imported through excel",
                    //         ChangeSequenceNumber = 0,
                    //         CreatedDate = DateTime.Now,
                    //         UpdatedDate = DateTime.Now,
                    //         CreatorUserId = user.Id,
                    //         LastUpdateUserId = user.Id,
                    //         IsDeleted = false,
                    //         IsActive = true,
                    //         Milestones = GetMilestones(model.ProjectMilestones, user),
                    //         Company = new Company()
                    //         {
                    //             Name = model.LineOfBusiness.Company.Name,
                    //             Description = "Imported through excel",
                    //             OwnerId = "cc2b0840-0804-4ea1-ac39-b41575bf14d4",
                    //             ChangeSequenceNumber = 0,
                    //             CreatedDate = DateTime.Now,
                    //             UpdatedDate = DateTime.Now,
                    //             CreatorUserId = user.Id,
                    //             LastUpdateUserId = user.Id,
                    //             IsDeleted = false,
                    //             IsActive = true,
                    //         }
                    //     };
                    // }
                    // else if (lob == null)
                    // {
                    //     newProject.LineOfBusiness = new LineOfBusiness()
                    //     {
                    //         Name = model.LineOfBusiness.Name,
                    //         Description = "Imported through excel",
                    //         ChangeSequenceNumber = 0,
                    //         CreatedDate = DateTime.Now,
                    //         UpdatedDate = DateTime.Now,
                    //         CreatorUserId = user.Id,
                    //         LastUpdateUserId = user.Id,
                    //         IsDeleted = false,
                    //         IsActive = true,
                    //         Milestones = GetMilestones(model.ProjectMilestones, user),
                    //         CompanyId = company.Id.Value,
                    //         // Company = new Company()
                    //         // {
                    //         //     //Id= company.Id.Value,
                    //         //     Name = company.Name,
                    //         //     Description = company.Description,
                    //         //     IsActive = true,
                    //         // }

                    //     };
                    // }
                    // else
                    // {
                    //     // newProject.LineOfBusiness = new LineOfBusiness()
                    //     // {
                    //     //     Id = lob.Id.Value,
                    //     //     Name = lob.Name,
                    //     //     Description = lob.Description
                    //     // };
                    //     newProject.LineOfBusinessId = lob.Id.Value;
                    // }
                    // if (client == null)
                    // {
                    //     // var lobs = new List<LineOfBusiness>();
                    //     //var lobs = await GetLineOfBusinessByIds(model.Client.LineOfBusinesses.Select(x => x.Id.Value).ToList());
                    //     //lobs.Add(lob);
                    //     // newProject.ClientStatus = 0;
                    //     newProject.Client = new Client()
                    //     {
                    //         Name = model.Client.Name,
                    //         Description = "Imported through excel",
                    //         // LineOfBusinesses = new List<LineOfBusiness>(){
                    //         // new LineOfBusiness(){
                    //         //     Id = lob.Id.HasValue? lob.Id.Value:-1,
                    //         //     Name = lob.Name,
                    //         //     Description = lob.Description
                    //         // }
                    //         // },
                    //         ChangeSequenceNumber = 0,
                    //         CreatedDate = DateTime.Now,
                    //         UpdatedDate = DateTime.Now,
                    //         CreatorUserId = user.Id,
                    //         LastUpdateUserId = user.Id,
                    //         IsDeleted = false,
                    //         IsActive = true,
                    //     };
                    //     if (lob != null)
                    //     {
                    //         newProject.Client.LineOfBusinesses = new List<LineOfBusiness>(){ new LineOfBusiness(){
                    //         Id = lob.Id.HasValue? lob.Id.Value:-1,
                    //         Name = lob.Name,
                    //       Description = lob.Description
                    //      }
                    // };
                    //     }
                    //     else
                    //     {
                    //         newProject.Client.LineOfBusinesses = new List<LineOfBusiness>(){
                    //         newProject.LineOfBusiness
                    //     };
                    //     }
                    //     // lobs.Add(new LineOfBusiness()
                    //     // {
                    //     //     Id = model.LineOfBusinessId.Value,
                    //     //     // Name = model.LineOfBusiness.Name,
                    //     //     // Description = model.LineOfBusiness.Description,
                    //     //     // CompanyId = model.LineOfBusiness.CompanyId.Value,
                    //     //     // IsRetainer = model.LineOfBusiness.IsRetainer.Value,
                    //     //     // IsActive = model.LineOfBusiness.IsActive.Value,
                    //     //     // Clients = new List<Client>(){
                    //     //     //     newOpportunity.Client
                    //     //     // }
                    //     // });


                    //     // newOpportunity.Client.LineOfBusinesses = lobs;

                    // }
                    // else
                    // {
                    //     newProject.ClientId = client.Id.Value;
                    //     //newProject.ClientStatus = 1;
                    // }

                    if (model.ProjectMilestones != null && model.ProjectMilestones.Count > 0)
                    {
                        int i = 0;
                        foreach (var x in model.ProjectMilestones)
                        {
                            var projectMilestone = new ProjectMilestone()
                            {
                                Name = x.Name,
                                //NeedPayment = x.NeedPayment.Value,
                                Description = "Imported through excel",
                                //Milestone = newProject.LineOfBusiness != null && newProject.LineOfBusiness.Milestones != null && newProject.LineOfBusiness.Milestones.Count > 0 ? newProject.LineOfBusiness.Milestones.First(y => y.Name == x.Name) : null,
                                // MilestoneId = lob != null ? lob.Milestones.First(y => y.Name == x.Name).Id.Value : -1,
                                Note = x.Note,
                                //PaymentValue = x.PaymentValue,
                                //PaymentValueCurrency = x.PaymentValueCurrency,
                                DateScheduled = x.DateScheduled,
                                DateActual = x.DateActual,
                                Status = x.DateActual.HasValue ? 2 : (model.CurrentProjectMilestoneIndex == i + 1 ? (model.Status == 4 ? 3 : 1) : 0),
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                IsDeleted = false,
                                IsActive = true,
                                ChangeSequenceNumber = 0,
                                CreatorUserId = user != null ? user.Id : null,
                                LastUpdateUserId = user != null ? user.Id : null,
                                MilestoneIndex = i + 1,
                                Start = i == 0,
                                End = i == model.ProjectMilestones.Count - 1
                            };
                            i++;
                            if (lob != null && lob.Milestones != null && lob.Milestones.Count > 0)
                            {
                                var milestone = lob.Milestones.FirstOrDefault(y => y.Name == x.Name);
                                if (milestone != null)
                                {
                                    projectMilestone.MilestoneId = milestone.Id.Value;
                                }
                            }
                            else
                            {
                                projectMilestone.Milestone = newProject.LineOfBusiness.Milestones.First(y => y.Name == x.Name);
                            }
                            newProject.ProjectMilestones.Add(projectMilestone);
                            // if(model.CurrentProjectMilestoneIndex==i+1){
                            //     model.ProjectMilestone = projectMilestone;
                            // }
                        }
                        // newProject.ProjectMilestones = model.ProjectMilestones.Select((x, i) => new ProjectMilestone()
                        // {

                        //     Name = x.Name,
                        //     //NeedPayment = x.NeedPayment.Value,
                        //     Description = "Imported through excel",
                        //     Milestone = newProject.LineOfBusiness != null && newProject.LineOfBusiness.Milestones != null && newProject.LineOfBusiness.Milestones.Count > 0 ? newProject.LineOfBusiness.Milestones.First(y => y.Name == x.Name) : null,
                        //     MilestoneId = lob != null ? lob.Milestones.First(y => y.Name == x.Name).Id.Value : -1,
                        //     Note = x.Note,
                        //     //PaymentValue = x.PaymentValue,
                        //     //PaymentValueCurrency = x.PaymentValueCurrency,
                        //     DateScheduled = x.DateScheduled,
                        //     DateActual = x.DateActual,
                        //     Status = x.DateActual.HasValue ? 3 : 0,
                        //     CreatedDate = DateTime.Now,
                        //     UpdatedDate = DateTime.Now,
                        //     IsDeleted = false,
                        //     IsActive = true,
                        //     ChangeSequenceNumber = 0,
                        //     CreatorUserId = user != null ? user.Id : null,
                        //     LastUpdateUserId = user != null ? user.Id : null,
                        //     MilestoneIndex = i + 1,
                        //     Start = i == 0,
                        //     End = i == model.ProjectMilestones.Count - 1
                        // }).ToList();
                    }

                    var opportunity = await opportunityService.GetOpportunityByProjecName(newProject.ProjectName);
                    if (opportunity != null)
                    {
                        newProject.OpportunityId = opportunity.Id;
                    }
                    else
                    {
                        newProject.Opportunity = GetOpportunity(newProject, user, lob, client);
                    }


                    var project = await new ProjectRepository(dbContext).AddAsync(newProject, user);
                    if (project == null)
                    {
                        result.Add(false);
                    }
                    else
                    {
                        result.Add(true);
                    }
                }
                else
                {
                    result.Add(false);
                }
                // }
                // catch (Exception ex)
                // {
                //     result.Add(false);
                // }
            }


            return result;
        }

        public Opportunity GetOpportunity(Project newProject, ApplicationUser? user, LineOfBusinessViewModel lob, ClientViewModel client)
        {

            var newOpportunity = new Opportunity
            {
                //Id = -1,
                ProjectName = newProject.ProjectName,
                //LineOfBusinessId = model.LineOfBusinessId.Value,

                Status = 4,
                Scope = newProject.Scope,
                //Client = newProject.Client,
                //ClientId = newProject.ClientId,
                ClientStatus = 1,
                // LineOfBusiness = newProject.LineOfBusiness,
                // LineOfBusinessId = newProject.LineOfBusinessId,
                //Project = newProject,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                IsDeleted = false,
                IsActive = true,
                ChangeSequenceNumber = 0,
                CreatorUserId = user != null ? user.Id : null,
                LastUpdateUserId = user != null ? user.Id : null,
                //Source = model.Source.Value,

                // ClientStatus = model.ClientStatus.Value,
                FirstContactDate = newProject.ContractSignatureDate,
                Note = "Created from imported project",


                FirstProposalDate = newProject.ContractSignatureDate,
                FirstProposalValue = newProject.ContractValue,
                FirstProposalValueCurrency = newProject.ContractValueCurrency,

                CurrentProposalValue = newProject.ContractValue,
                CurrentProposalValueCurrency = newProject.ContractValueCurrency,

                ContractSignatureDate = newProject.ContractSignatureDate,
                FinalContractValue = newProject.ContractValue,
                FinalContractValueCurrency = newProject.ContractValueCurrency,
                RetainerValidatity = newProject.RetainerValidatity,
                //   Contact = model.Contact
            };
            if (client == null)
            {
                newOpportunity.Client = newProject.Client;
            }
            else
            {
                newOpportunity.ClientId = client.Id.Value;
            }
            if (lob == null)
            {
                newOpportunity.LineOfBusiness = newProject.LineOfBusiness;
            }
            else
            {
                newOpportunity.LineOfBusinessId = lob.Id.Value;
            }
            return newOpportunity;

        }

        public ICollection<Milestone> GetMilestones(ICollection<ProjectMilestoneViewModel> projectMilestones, ApplicationUser? user)
        {
            var milestones = projectMilestones.Select((x, i) => new Milestone()
            {

                Name = x.Name,
                //NeedPayment = x.NeedPayment.Value,
                Description = "Imported through excel",
                // Milestone = newProject.LineOfBusiness.Milestones.First(y => y.Name == x.Name),
                // //MilestoneId = x.MilestoneId.Value,
                // Note = x.Note,
                // //PaymentValue = x.PaymentValue,
                // //PaymentValueCurrency = x.PaymentValueCurrency,
                // DateScheduled = x.DateScheduled,
                // DateActual = x.DateActual,
                // Status = x.DateActual.HasValue ? 3 : 0,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                IsDeleted = false,
                IsActive = true,
                ChangeSequenceNumber = 0,
                CreatorUserId = user != null ? user.Id : null,
                LastUpdateUserId = user != null ? user.Id : null,
                // MilestoneIndex = i + 1,
                // Start = i == 0,
                // End = i == model.ProjectMilestones.Count - 1
            }).ToList();
            return milestones;
        }





    }
}