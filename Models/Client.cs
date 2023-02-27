using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Client : BaseEntity
    {
        public Client()
        {
            this.LineOfBusinesses = new HashSet<LineOfBusiness>();
            this.Files = new HashSet<File>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        //[ForeignKey(nameof(LineOfBusiness))]
        // public int LineOfBusinessId { get; set; }

        public virtual ICollection<LineOfBusiness> LineOfBusinesses { get; set; }

        public virtual ICollection<File>? Files { get; set; }

        public string Description { get; set; }

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

        //public string? TaxCardNumberPath { get; set; }

    }
}