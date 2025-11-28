using Site_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_3;
using Site_3.Models;
using System.Security.Claims;

namespace Site_3.Controllers
{
    [Authorize]
    public class SharedLinksController : Controller
    {
        private readonly AdvancedDbContext _context;

        public SharedLinksController(AdvancedDbContext context)
        {
            _context = context;
        }

        public class CreateSharedLinkModel
        {
            public string TargetId { get; set; } = string.Empty;
            public string TargetType { get; set; } = "document";
            public int? Days { get; set; }
        }

        [HttpPost("shared-links")]
        public async Task<IActionResult> CreateSharedLink([FromBody] CreateSharedLinkModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var link = new SharedLink
            {
                UserId = userId,
                TargetId = model.TargetId,
                TargetType = model.TargetType,
                ExpiresAt = DateTime.Now.AddDays(model.Days ?? 7)
            };

            _context.SharedLinks.Add(link);
            await _context.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}/shared/{link.Id}";
            return Json(new { link = url });
        }

        // ⚠️ Уязвимость: общая ссылка доступна без аутентификации
        [HttpGet("shared/{linkId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSharedContent(string linkId)
        {
            var link = await _context.SharedLinks.FirstOrDefaultAsync(l => l.Id == linkId && l.ExpiresAt > DateTime.Now);
            if (link == null) return NotFound("Link expired or invalid");

            // ⚠️ Нет проверки прав доступа к целевому объекту
            return Content($"Документ ID: {link.TargetId} (владелец: {link.UserId})");
        }

        // ⚠️ Уязвимость: можно найти все ссылки на документ
        [HttpGet("shared-links/search")]
        public async Task<IActionResult> SearchSharedLinks([FromQuery] string targetId)
        {
            var links = await _context.SharedLinks.Where(l => l.TargetId == targetId).ToListAsync();
            return Json(links);
        }
    }
}