using Microsoft.AspNetCore.Identity;
using Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.LineOfBusinesses = new HashSet<LineOfBusiness>();
            // this.UserRoles = new HashSet<IdentityRole>();
        }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyId { get; set; }

        // [ForeignKey(nameof(LineOfBusiness))]
        // public int? LineOfBusinessId { get; set; }


        public virtual Company? Company { get; set; }
        public virtual ICollection<LineOfBusiness> LineOfBusinesses { get; set; }
        //public virtual ICollection<IdentityRole> UserRoles { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}

