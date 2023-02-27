
using Microsoft.AspNetCore.Mvc;
using Backend.Models.FileViewModels;



namespace Backend.Controllers
{
    [ApiController]
    [Route("/")]
    public class UploadController : ControllerBase
    {

        [HttpPost]
        [Route(nameof(Upload))]
        public async Task<IActionResult> Upload([FromForm] FileUploadViewModel file)
        {
            try
            {
                if (!(Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/", file.FileName.Split('/')[0]))))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/", file.FileName.Split('/')[0]));
                }
                var path = Path.Combine(
                              Directory.GetCurrentDirectory(),
                              "wwwroot/" + file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.FormFile.CopyTo(stream);
                }

                return Ok();
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet]
        [Route(nameof(Download))]
        public async Task<IActionResult> Download(string fileName)
        {
            if (fileName == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", fileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<IActionResult> Delete([FromBody] FileViewModel file)
        {
            if (file.Name == null)
                return Content("filename not present");

            try
            {
                var path = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "wwwroot/", file.Name);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Ok();
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
               {".csv", "text/csv"}
            };
        }
    }
}