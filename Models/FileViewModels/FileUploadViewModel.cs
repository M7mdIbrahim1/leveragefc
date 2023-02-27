using Backend.Models.ClientViewModels;
using Backend.Models.CompanyViewModels;

namespace Backend.Models.FileViewModels
{
    public class FileUploadViewModel
    {

        public string FileName { get; set; }
        public IFormFile FormFile { get; set; }

    }
}