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
    public class SchedulesController : Controller
    {
        private readonly AdvancedDbContext _context;

        public SchedulesController(AdvancedDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpPost("schedules/export")]
        public async Task<IActionResult> CreateExportSchedule([FromForm] string CronExpression = "0 0 * * *")
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var schedule = new Schedule { UserId = userId, CronExpression = CronExpression };
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // ⚠️ Уязвимость: можно получить экспорт любого пользователя
        [HttpGet("schedules/export/result/{userId}")]
        public async Task<IActionResult> GetExportResult(int userId)
        {
            var job = await _context.ExportJobs
                .Where(j => j.UserId == userId && j.Status == "completed")
                .OrderByDescending(j => j.CreatedAt)
                .FirstOrDefaultAsync();

            if (job == null) return NotFound("No export found");
            return Content($"Результат экспорта для пользователя {userId}:<br>{job.ExportData}");
        }

        // ⚠️ Уязвимость: JSON globbing
        [HttpPost("schedules/batch")]
        public async Task<IActionResult> BatchSchedules([FromBody] dynamic request)
        {
            if (request.userIds is System.Text.Json.JsonElement json && json.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var userIds = json.EnumerateArray().Select(e => e.GetInt32()).ToList();
                var schedules = await _context.Schedules.Where(s => userIds.Contains(s.UserId)).ToListAsync();
                return Json(schedules);
            }
            return BadRequest();
        }
    }
}