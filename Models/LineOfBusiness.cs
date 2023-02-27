using System.ComponentModel.DataAnnotations;
using Backend.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class LineOfBusiness : BaseEntity
    {
        public LineOfBusiness()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Clients = new HashSet<Client>();
            this.Milestones = new HashSet<Milestone>();

        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
        // public string Contact { get; set; }

        //[Required(ErrorMessage = "Owner is required")]
        // public string OwnerId { get; set; }

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual CompanyGroup? CompanyGroup { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Milestone> Milestones { get; set; }

        public bool IsRetainer { get; set; }


        [ForeignKey(nameof(CompanyGroup))]
        public int? CompanyGroupId { get; set; }

    }
}