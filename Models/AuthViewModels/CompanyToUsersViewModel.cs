using System.ComponentModel.DataAnnotations;

namespace Backend.Models.AuthViewModels
{
    public class CompanyToUsersViewModel
    {

        public ICollection<string>? UserIds { get; set; }
        public int CompanyId { get; set; }

        public ICollection<int>? LineOfBusinessIds { get; set; }
    }
}