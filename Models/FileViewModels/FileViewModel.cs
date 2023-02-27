using Backend.Models.ClientViewModels;
using Backend.Models.CompanyViewModels;

namespace Backend.Models.FileViewModels
{
    public class FileViewModel
    {
        public FileViewModel()
        {
            this.Clients = new HashSet<ClientViewModel>();
            this.Companies = new HashSet<CompanyViewModel>();
        }

        public int? Id { get; set; }
        public string? Name { get; set; }

        public string? Url { get; set; }

        public string? Status { get; set; }
        //public IFormFile? FormFile { get; set; }

        public virtual ICollection<CompanyViewModel>? Companies { get; set; }
        public virtual ICollection<ClientViewModel>? Clients { get; set; }


    }
}