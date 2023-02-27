using System.ComponentModel.DataAnnotations;
using Backend.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class Company : BaseEntity
    {
        public Company()
        {
            this.LineOfBusinesses = new HashSet<LineOfBusiness>();
            this.Files = new HashSet<File>();
            this.CompanyGroups = new HashSet<CompanyGroup>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Owner is required")]

        [ForeignKey(nameof(ApplicationUser))]
        public string OwnerId { get; set; }

        public string Description { get; set; }
        // public string Contact { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public string? CommercialRegistrationNumber { get; set; }

        //  public string? CommercialRegistrationNumberPath { get; set; }

        public string? Email { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? State { get; set; }

        public string? City { get; set; }

        public string? PostCode { get; set; }

        public string? Country { get; set; }

        public string? ContactNumber { get; set; }

        public string? ContactPerson { get; set; }

        public string? TaxCardNumber { get; set; }

        public virtual ICollection<LineOfBusiness> LineOfBusinesses { get; set; }

        public virtual ICollection<CompanyGroup> CompanyGroups { get; set; }

        public virtual ICollection<File>? Files { get; set; }


    }
}