using Site_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_3;
using System.Security.Claims;

namespace Site_3.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly AdvancedDbContext _context;

        public MessagesController(AdvancedDbContext context)
        {
            _context = context;
        }

        [HttpGet("messages/drafts")]
        public async Task<IActionResult> Drafts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var drafts = await _context.Messages.Where(m => m.UserId == userId && m.Status == "draft").ToListAsync();
            return View(drafts);
        }

        // ⚠️ Уязвимость: можно указать TargetUserId
        public class DraftModel
        {
            public string Content { get; set; } = string.Empty;
            public int TargetUserId { get; set; } // Уязвимость!
        }

        [HttpPost("messages/draft")]
        public async Task<IActionResult> SaveDraft([FromBody] DraftModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var message = new Message
            {
                UserId = model.TargetUserId, // ⚠️ Вторичный IDOR
                Content = model.Content,
                Status = "draft"
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return Json(new { id = message.Id, status = "saved" });
        }

        // ⚠️ Уязвимость: parameter pollution
        [HttpGet("messages/batch-drafts")]
        public async Task<IActionResult> BatchDrafts([FromQuery] string userId)
        {
            var userIds = userId.Split(',').Select(int.Parse).ToList();
            var drafts = await _context.Messages.Where(m => userIds.Contains(m.UserId) && m.Status == "draft").ToListAsync();
            return Json(drafts);
        }
    }
}