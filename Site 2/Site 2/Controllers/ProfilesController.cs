using Site_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_2;
using System.Security.Claims;

namespace Site_2.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly ApiDbContext _context;

        public ProfilesController(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profiles = await _context.Profiles.Where(p => p.UserId == userId).ToListAsync();
            return View(profiles);
        }

        public async Task<IActionResult> View(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null) return NotFound();
            return View(profile);
        }
    }
}