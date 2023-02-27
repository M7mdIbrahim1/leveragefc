using System;
using Backend.Models.CompanyViewModels;
using Backend.Models.LineOfBusinessViewModels;

namespace Backend.Models.AuthViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            this.UserRoles = new HashSet<string>();
        }
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        //  public string? Contact { get; set; }
        public int? CompanyId { get; set; }

        public bool? IsActive { get; set; }

        // public int? LineOfBusinessId { get; set; }
        public int? TotalCount { get; set; }

        public virtual CompanyViewModel? Company { get; set; }

        public virtual ICollection<LineOfBusinessViewModel>? LineOfBusinesses { get; set; }
        public virtual ICollection<string>? UserRoles { get; set; }
    }
}