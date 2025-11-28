using Site_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_2;
using Site_2.Models;
using System.Text;

namespace Site_2.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public FilesController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [Produces("application/json", "application/xml", "text/plain")]
        public async Task<IActionResult> GetFile(string id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null) return NotFound();

            var accept = Request.Headers["Accept"].ToString();

            if (accept.Contains("application/xml"))
            {
                var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<File><Id>{file.Id}</Id><FileName>{file.FileName}</FileName><Content>{file.Content}</Content></File>";
                return Content(xml, "application/xml");
            }
            else if (accept.Contains("text/plain"))
            {
                return Content($"ID: {file.Id}\nFile: {file.FileName}\nContent: {file.Content}", "text/plain");
            }
            else
            {
                return Ok(file);
            }
        }

        public class FileUploadModel
        {
            public int UserId { get; set; } // Уязвимость: можно указать чужой ID
            public string FileName { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromBody] FileUploadModel model)
        {
            var file = new FileRecord
            {
                Id = Guid.NewGuid().ToString(),
                UserId = model.UserId, // ⚠️ Уязвимость!
                FileName = model.FileName,
                Content = model.Content
            };

            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFile), new { id = file.Id }, file);
        }
    }
}