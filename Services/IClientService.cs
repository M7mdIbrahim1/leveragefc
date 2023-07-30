using Backend.Models;
using Backend.Models.ClientViewModels;
using Backend.Helper;
using Backend.Auth;

namespace Backend.Services
{
    public interface IClientService
    {
        Task<ClientViewModel> GetClientById(int clientId);
        Task<ClientViewModel> GetClientByName(string clientName, int lobId);
        Task<ICollection<ClientViewModel>> GetClientsByIds(ICollection<int> clientsIds);
        Task<ICollection<ClientViewModel>> GetAllClients(ClientSearchViewModel searchViewModel, string? userId, bool isOwner);
        Task<ApiResponse<bool>> AddClient(ClientViewModel model, ApplicationUser? user);
        Task<ApiResponse<bool>> DeleteClient(ClientViewModel deleteClient, ApplicationUser? user);
        Task<ApiResponse<bool>> UpdateClient(ClientViewModel updateClient, ApplicationUser? user);
        Task<ICollection<Client>> GetClientByNames(ICollection<string> clientNames);
    }
}