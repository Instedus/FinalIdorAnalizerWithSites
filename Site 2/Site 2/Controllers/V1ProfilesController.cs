using Site_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_2;

namespace Site_2.Controllers
{
    [ApiController]
    [Route("api/v1/profiles")]
    [Authorize]
    public class V1ProfilesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public V1ProfilesController(ApiDbContext context)
        {
            _context = context;
        }

        // Уязвимо: нет проверки доступа
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        // Parameter pollution
        [HttpGet("search")]
        public async Task<IActionResult> SearchProfiles([FromQuery] string id)
        {
            var ids = id.Split(',').Select(int.Parse).ToList();
            var profiles = await _context.Profiles.Where(p => ids.Contains(p.Id)).ToListAsync();
            return Ok(profiles);
        }

        // HTTP Method bypass
        [HttpPost("current")]
        public async Task<IActionResult> GetCurrentProfilePost()
        {
            var profiles = await _context.Profiles.ToListAsync();
            return Ok(profiles); // Возвращает ВСЕ профили!
        }
    }
}