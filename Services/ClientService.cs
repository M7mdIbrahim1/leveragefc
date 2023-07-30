using Backend.Data;
using Backend.Models;
using Backend.Models.ClientViewModels;
using Backend.Helper;
using Backend.Repositories;
using System.Linq.Expressions;
using AutoMapper;
using Backend.Auth;
using Backend.Models.CompanyViewModels;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.FileViewModels;


namespace Backend.Services
{
    public class ClientService : IClientService
    {

        private readonly IMapper mapper;


        private readonly ApplicationDbContext dbContext;
        private readonly ICompanyService companyService;

        private readonly ILineOfBusinessService lobService;

        public ClientService(ApplicationDbContext dbContext, IMapper mapper, ICompanyService companyService, ILineOfBusinessService lobService) //IRepository<Client> clientRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.companyService = companyService;
            this.lobService = lobService;
        }
        public async Task<ApiResponse<bool>> AddClient(ClientViewModel model, ApplicationUser? user)
        {
            var lobs = await lobService.GetLineOfBusinessesByIds(model.LineOfBusinesses.Select(x => x.Id.Value).ToList());
            var client = new Client
            {
                Name = model.Name,
                Description = model.Description,
                LineOfBusinesses = lobs
                //   Contact = model.Contact
            };

            client.CommercialRegistrationNumber = model.CommercialRegistrationNumber;
            // client.CommercialRegistrationNumberPath = updatedClient.CommercialRegistrationNumberPath;
            client.AddressLine1 = model.AddressLine1;
            client.AddressLine2 = model.AddressLine2;
            client.State = model.State;
            client.City = model.City;
            client.Country = model.Country;
            client.ContactNumber = model.ContactNumber;
            client.ContactPerson = model.ContactPerson;
            client.Email = model.Email;
            client.TaxCardNumber = model.TaxCardNumber;
            //client.TaxCardNumberPath = updatedClient.TaxCardNumberPath;
            client.PostCode = model.PostCode;

            client.Files = model.Files.Select(x => new Backend.Models.File
            {
                //Id=x.Id.Value,
                Name = x.Name,
                Url = x.Url,
                Status = x.Status,

            }).ToList();

            var addedClient = await new ClientRepository(dbContext).AddAsync(client, user);
            if (addedClient == null)
            {
                return new ApiResponse<bool>(false, "Error in creating client", false);
            }
            return new ApiResponse<bool>(true, "Client was created successfully", true);
        }

        public async Task<ApiResponse<bool>> UpdateClient(ClientViewModel updatedClient, ApplicationUser? user)
        {
            var client = await new ClientRepository(dbContext).GetAsync(a => a.Id == updatedClient.Id, null, true, c => c.LineOfBusinesses, c => c.Files);
            if (client == null)
            {
                return new ApiResponse<bool>(false, "Client not found", false);
            }
            if (updatedClient.Name != null && updatedClient.Name != "")
            {
                client.Name = updatedClient.Name;
            }
            if (updatedClient.Description != null && updatedClient.Description != "")
            {
                client.Description = updatedClient.Description;
            }
            if (updatedClient.Name != null && updatedClient.Name != "")
            {
                removeLineofBusinesses(client.LineOfBusinesses, client.LineOfBusinesses.Where(x => !updatedClient.LineOfBusinesses.Any(y => y.Id == x.Id)).Select(x => x.Id).ToList());
                addLineofBusinesses(client.LineOfBusinesses, updatedClient.LineOfBusinesses.Where(x => !client.LineOfBusinesses.Any(y => y.Id == x.Id)).ToList());
            }

            removeFiles(client.Files, client.Files.Where(x => !updatedClient.Files.Any(y => y.Id == x.Id)).Select(x => x.Id).ToList());
            addFiles(client.Files, updatedClient.Files.Where(x => !client.Files.Any(y => y.Id == x.Id)).ToList(), user);
            //client.LineOfBusinessId = updatedClient.LineOfBusinessId.Value;

            client.CommercialRegistrationNumber = updatedClient.CommercialRegistrationNumber;
            // client.CommercialRegistrationNumberPath = updatedClient.CommercialRegistrationNumberPath;
            client.AddressLine1 = updatedClient.AddressLine1;
            client.AddressLine2 = updatedClient.AddressLine2;
            client.State = updatedClient.State;
            client.City = updatedClient.City;
            client.Country = updatedClient.Country;
            client.ContactNumber = updatedClient.ContactNumber;
            client.ContactPerson = updatedClient.ContactPerson;
            client.Email = updatedClient.Email;
            client.TaxCardNumber = updatedClient.TaxCardNumber;
            //client.TaxCardNumberPath = updatedClient.TaxCardNumberPath;
            client.PostCode = updatedClient.PostCode;



            var isUpdated = await new ClientRepository(dbContext).UpdateAsync(client, client.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the client was not updated", false);
            }

            return new ApiResponse<bool>(true, "Client updated successfully", true);
        }

        public void removeLineofBusinesses(ICollection<LineOfBusiness> lineOFBusinesses, List<int> ids)
        {
            foreach (var id in ids)
            {
                lineOFBusinesses.Remove(lineOFBusinesses.FirstOrDefault(x => x.Id == id));
            }
        }

        public void removeFiles(ICollection<Backend.Models.File> files, List<int> ids)
        {
            foreach (var id in ids)
            {
                files.Remove(files.FirstOrDefault(x => x.Id == id));
            }
        }

        public async void addLineofBusinesses(ICollection<LineOfBusiness> lineOFBusinesses, ICollection<LineOfBusinessViewModel> addedLobs)
        {
            var lobs = await lobService.GetLineOfBusinessesByIds(addedLobs.Select(x => x.Id.Value).ToList());
            foreach (var item in lobs)
            {
                lineOFBusinesses.Add(item);
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

        public async Task<ApiResponse<bool>> DeleteClient(ClientViewModel deleteClient, ApplicationUser? user)
        {
            //var isDeleted = await new ClientRepository(dbContext).DeleteAsync(s => s.Id == clientId);
            // return new ApiResponse<bool>(isDeleted, isDeleted ? "client was deleted successfully" : "Error in deleting client", isDeleted);

            var client = await new ClientRepository(dbContext).GetAsync(a => a.Id == deleteClient.Id);
            if (client == null)
            {
                return new ApiResponse<bool>(false, "Client not found", false);
            }

            client.IsDeleted = true;

            var isUpdated = await new ClientRepository(dbContext).UpdateAsync(client, deleteClient.Id, user);
            if (!isUpdated)
            {
                return new ApiResponse<bool>(false, "Error was occurred! the client was not deleted", false);
            }

            return new ApiResponse<bool>(true, "Client deleted successfully", true);
        }

        public async Task<ICollection<ClientViewModel>> GetAllClients(ClientSearchViewModel searchViewModel, string? userId, bool isOwner)
        {


            Paging<Client> paging = null;

            if (searchViewModel.page.HasValue && searchViewModel.page.Value != 0)
            {
                paging = new Paging<Client>
                {
                    Skip = (searchViewModel.page.Value - 1) * searchViewModel.pageSize.Value,
                    OrderBy = searchViewModel.orderBy != null ? searchViewModel.orderBy : x => x,
                    PageSize = searchViewModel.pageSize.Value
                };
            }


            Expression<Func<Client, bool>> predicate1 = searchViewModel.name != null && searchViewModel.name != "" ? x => x.Name.ToLower().Contains(searchViewModel.name.ToLower()) && x.IsDeleted == false : x => x.IsDeleted == false;

            Expression<Func<Client, bool>> predicate2;
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
                    if (searchViewModel.lineOfBusinessIds != null && searchViewModel.lineOfBusinessIds.Count > 0)
                    {
                        intersectIds = lobs.Select(x => x.Id.Value).Intersect(searchViewModel.lineOfBusinessIds).ToList();
                    }
                    predicate2 = x => x.LineOfBusinesses.Any(y => intersectIds.Any(z => z == y.Id));
                }
                else
                {
                    lobs = await lobService.GetLineOfBusinessesByUserId(userId);
                    var intersectIds = lobs.Select(x => x.Id.Value);
                    if (searchViewModel.lineOfBusinessIds != null && searchViewModel.lineOfBusinessIds.Count > 0)
                    {
                        intersectIds = lobs.Select(x => x.Id.Value).Intersect(searchViewModel.lineOfBusinessIds).ToList();
                    }
                    predicate2 = x => x.LineOfBusinesses.Any(y => intersectIds.Any(z => z == y.Id));
                }
            }
            else
            {
                predicate2 = searchViewModel.lineOfBusinessIds != null && searchViewModel.lineOfBusinessIds.Count > 0 ? x => x.LineOfBusinesses.Any(y => searchViewModel.lineOfBusinessIds.Any(z => z == y.Id)) : x => 1 == 1;
            }
            ParameterExpression param = predicate1.Parameters[0];
            Expression<Func<Client, bool>> predicate3 = Expression.Lambda<Func<Client, bool>>(Expression.AndAlso(predicate1.Body, Expression.Invoke(predicate2, param)), param);
            // Expression<Func<Company, bool>> predicate3 = (x => predicate1(x) && predicate2(x));

            var clientCount = (await new ClientRepository(dbContext).GetListAsync(predicate3, null)).Count();


            var clients = await new ClientRepository(dbContext).GetListAsync(predicate3, paging, p => p.LineOfBusinesses, p => p.LineOfBusinesses.Select(x => x.Company), p => p.Files);
            var model = new List<ClientViewModel>();
            foreach (var client in clients)
            {
                model.Add(new ClientViewModel
                {
                    Id = client.Id,
                    Name = client.Name,
                    Description = client.Description,
                    // LineOfBusinessId = client.LineOfBusinessId,
                    LineOfBusinesses = returnLineOfBusinessesViewModels(client.LineOfBusinesses),
                    ContactPerson = client.ContactPerson,
                    Email = client.Email,
                    ContactNumber = client.ContactNumber,
                    AddressLine1 = client.AddressLine1,
                    AddressLine2 = client.AddressLine2,
                    PostCode = client.PostCode,
                    Country = client.Country,
                    State = client.State,
                    City = client.City,
                    CommercialRegistrationNumber = client.CommercialRegistrationNumber,
                    TaxCardNumber = client.TaxCardNumber,
                    Files = client.Files.Select(x => new FileViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Url = x.Url,
                        Status = x.Status
                    }).ToList(),
                    TotalCount = clientCount
                });
            }
            return model;
        }

        public ICollection<LineOfBusinessViewModel> returnLineOfBusinessesViewModels(ICollection<LineOfBusiness> lineOfBusinesses)
        {
            var lineOfBusinessesViewModels = new List<LineOfBusinessViewModel>();

            foreach (var item in lineOfBusinesses)
            {
                var lineOfBusinessViewmodel = new LineOfBusinessViewModel()
                {
                    Name = item.Name,
                    Id = item.Id,
                    Company = new CompanyViewModel()
                    {
                        Id = item.Company.Id,
                        Name = item.Company.Name,
                    }
                };
                lineOfBusinessesViewModels.Add(lineOfBusinessViewmodel);
            }
            return lineOfBusinessesViewModels;
        }

        public async Task<ClientViewModel> GetClientById(int clientId)
        {
            var client = await new ClientRepository(dbContext).GetAsync(c => c.Id == clientId, null, false, p => p.LineOfBusinesses);
            if (client != null)
            {

                //var clientViewModel = mapper.Map<ClientViewModel>(client);
                return new ClientViewModel
                {
                    Id = client.Id,
                    Name = client.Name,
                    Description = client.Description,
                    // LineOfBusinessId = client.LineOfBusinessId,
                    LineOfBusinesses = returnLineOfBusinessesViewModels(client.LineOfBusinesses),
                    AddressLine1 = client.AddressLine1,
                    AddressLine2 = client.AddressLine2,
                    ContactNumber = client.ContactNumber,
                    ContactPerson = client.ContactPerson,
                    State = client.State,
                    City = client.City,
                    Country = client.Country,
                    TaxCardNumber = client.TaxCardNumber,
                    //TaxCardNumberPath = client.TaxCardNumberPath,
                    CommercialRegistrationNumber = client.CommercialRegistrationNumber,
                    //CommercialRegistrationNumberPath = client.CommercialRegistrationNumberPath,
                    Email = client.Email,
                    PostCode = client.PostCode,
                    Files = client.Files.Select(x => new FileViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Url = x.Url,
                        Status = x.Status
                    }).ToList(),
                };
            }
            else
            {
                return null;
            }

        }


        public async Task<ICollection<Client>> GetClientByNames(ICollection<string> clientNames)
        {
            var clients = (await new ClientRepository(dbContext).GetListAsync(c => clientNames.Contains(c.Name), null, p => p.LineOfBusinesses, p => p.LineOfBusinesses.Select(x => x.Company))).ToList();
            return clients;
        }

        public async Task<ClientViewModel> GetClientByName(string clientName, int lobId)
        {
            var clients = (await new ClientRepository(dbContext).GetListAsync(c => c.Name == clientName, null, p => p.LineOfBusinesses, p => p.LineOfBusinesses.Select(x => x.Company))).ToList();
            if (clients != null && clients.Count > 0)
            {
                var client = clients.Where(x => x.LineOfBusinesses.Any(y => y.Id == lobId)).FirstOrDefault();
                //var clientViewModel = mapper.Map<ClientViewModel>(client);
                if (client != null)
                {
                    return new ClientViewModel
                    {
                        Id = client.Id,
                        Name = client.Name,
                        Description = client.Description,
                        // LineOfBusinessId = client.LineOfBusinessId,
                        LineOfBusinesses = returnLineOfBusinessesViewModels(client.LineOfBusinesses),
                        AddressLine1 = client.AddressLine1,
                        AddressLine2 = client.AddressLine2,
                        ContactNumber = client.ContactNumber,
                        ContactPerson = client.ContactPerson,
                        State = client.State,
                        City = client.City,
                        Country = client.Country,
                        TaxCardNumber = client.TaxCardNumber,
                        //TaxCardNumberPath = client.TaxCardNumberPath,
                        CommercialRegistrationNumber = client.CommercialRegistrationNumber,
                        //CommercialRegistrationNumberPath = client.CommercialRegistrationNumberPath,
                        Email = client.Email,
                        PostCode = client.PostCode,
                        Files = client.Files.Select(x => new FileViewModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Url = x.Url,
                            Status = x.Status
                        }).ToList(),
                    };
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

        public async Task<ICollection<ClientViewModel>> GetClientsByIds(ICollection<int> clientIds)
        {
            var clients = await new ClientRepository(dbContext).GetListAsync(c => clientIds.Contains(c.Id), null, p => p.LineOfBusinesses);
            var clientsViewModel = new List<ClientViewModel>();
            if (clients != null)
            {
                foreach (var client in clients)
                {
                    clientsViewModel.Add(mapper.Map<ClientViewModel>(client));
                }
                return clientsViewModel;
            }
            else
            {
                return null;
            }

        }



    }
}