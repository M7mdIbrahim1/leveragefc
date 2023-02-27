using System.ComponentModel.DataAnnotations;

namespace Backend.Models.AuthViewModels
{
    public class UserRolesViewModel
    {

        public List<string>? UserIds { get; set; }

        public List<string>? Roles { get; set; }
    }
}