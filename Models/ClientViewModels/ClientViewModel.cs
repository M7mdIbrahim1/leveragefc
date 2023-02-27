using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.FileViewModels;

namespace Backend.Models.ClientViewModels
{
    public class ClientViewModel
    {
        public ClientViewModel()
        {
            this.LineOfBusinesses = new HashSet<LineOfBusinessViewModel>();
            this.Files = new HashSet<FileViewModel>();
        }
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        // public int? LineOfBusinessId { get; set; }
        public virtual ICollection<LineOfBusinessViewModel>? LineOfBusinesses { get; set; }

        public virtual ICollection<FileViewModel>? Files { get; set; }

        public string? CommercialRegistrationNumber { get; set; }

        // public string? CommercialRegistrationNumberPath { get; set; }

        public string? Email { get; set; }

        public string? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }

        public string? TaxCardNumber { get; set; }

        // public string? TaxCardNumberPath { get; set; }

        public int? TotalCount { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? State { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostCode { get; set; }

    }
}