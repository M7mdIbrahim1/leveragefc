using System;
using Backend.Auth;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.FileViewModels;

namespace Backend.Models.CompanyViewModels
{
    public class CompanyViewModel
    {

        public CompanyViewModel()
        {
            this.LineOfBusinesses = new HashSet<LineOfBusinessViewModel>();
            this.Files = new HashSet<FileViewModel>();
            this.CompanyGroups = new HashSet<CompanyGroupViewModel>();
        }
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? OwnerId { get; set; }
        //public string Contact { get; set; }

        public bool? IsActive { get; set; }

        public int? TotalCount { get; set; }

        public virtual ApplicationUser? Owner { get; set; }
        public virtual ICollection<LineOfBusinessViewModel>? LineOfBusinesses { get; set; }

        public virtual ICollection<FileViewModel>? Files { get; set; }

        public virtual ICollection<CompanyGroupViewModel>? CompanyGroups { get; set; }

        public string? CommercialRegistrationNumber { get; set; }

        // public string? CommercialRegistrationNumberPath { get; set; }

        public string? Email { get; set; }

        public string? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }

        public string? TaxCardNumber { get; set; }

        // public string? TaxCardNumberPath { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? State { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostCode { get; set; }
    }
}