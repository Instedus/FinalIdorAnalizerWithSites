using Site_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_2;
using System.Security.Claims;

namespace Site_2.Controllers
{
    [ApiController]
    [Route("api/v2/profiles")]
    [Authorize]
    public class V2ProfilesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public V2ProfilesController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
    }
}