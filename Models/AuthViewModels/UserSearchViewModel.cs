using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Backend.Models.AuthViewModels
{
    public class UserSearchViewModel<ApplicationUser>
    {
        public string? emailOrUserName { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public Expression<Func<ApplicationUser, object>>? orderBy { get; set; }

        public ICollection<int>? companiesIds { get; set; }

        public ICollection<string>? roles { get; set; }

        public ICollection<int>? lineOfBusinessesIds { get; set; }

    }
}