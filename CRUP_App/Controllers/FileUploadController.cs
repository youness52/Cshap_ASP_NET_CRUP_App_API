using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUP_App.Controllers
{
    [Route("api/savefile")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public FileUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> SaveFile()
        {
            try
            {
                if (!Request.Form.Files.Any())
                {
                    return BadRequest("No file uploaded.");
                }

                var postedFile = Request.Form.Files[0];

                if (postedFile.Length <= 0)
                {
                    return BadRequest("Empty file uploaded.");
                }

                var fileExtension = Path.GetExtension(postedFile.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" }; // Add more if needed
                if (!allowedExtensions.Contains(fileExtension.ToLowerInvariant()))
                {
                    return BadRequest("Invalid file type.");
                }

                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "Photos");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Generate a random filename
                var fileName = Guid.NewGuid().ToString("N") + fileExtension;
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(stream);
                }

                return Ok(new { fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
    }
}
