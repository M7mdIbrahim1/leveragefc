using System.ComponentModel.DataAnnotations;
using Backend.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class File : BaseEntity
    {
        public File()
        {
            this.Clients = new HashSet<Client>();
            this.Companies = new HashSet<Company>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "URL is required")]
        public string Url { get; set; }
        // public string Contact { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Company> Companies { get; set; }

    }
}