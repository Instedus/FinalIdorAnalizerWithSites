using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Site_1.Models;
using System.Security.Claims;

[Authorize]
public class DocumentsController : Controller
{
    private readonly AppDbContext _context;

    public DocumentsController(AppDbContext context)
    {
        _context = context;
    }

    // Базовая IDOR уязвимость с числовым ID
    [HttpGet("documents/{id}")]
    public async Task<IActionResult> GetDocument(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }
        return View("View", document);
    }

    // IDOR с загрязнением параметров
    [HttpGet("documents/search")]
    public async Task<IActionResult> SearchDocuments(string id)
    {
        // Уязвимость: загрязнение параметров - принимает несколько значений
        var ids = id.Split(',').Select(int.Parse).ToList();
        var documents = await _context.Documents.Where(d => ids.Contains(d.Id)).ToListAsync();

        return Json(documents);
    }

    // IDOR с JSON globbing
    [HttpPost("documents/batch")]
    public async Task<IActionResult> GetBatchDocuments([FromBody] dynamic request)
    {
        // Уязвимость: принимает массив, булевы значения, wildcards
        var documents = new List<Document>();

        if (request.ids is System.Text.Json.JsonElement jsonElement && jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var item in jsonElement.EnumerateArray())
            {
                if (item.TryGetInt32(out int id))
                {
                    var doc = await _context.Documents.FindAsync(id);
                    if (doc != null) documents.Add(doc);
                }
            }
        }
        else if (request.ids is bool)
        {
            // Уязвимость: обработка булевых значений
            documents = await _context.Documents.ToListAsync();
        }
        else if (request.ids is string str && (str == "*" || str == "%"))
        {
            // Уязвимость: обработка wildcards
            documents = await _context.Documents.ToListAsync();
        }
        else if (request.ids is string idsStr)
        {
            // Уязвимость: обработка строковых значений с разделителями
            var idList = idsStr.Split(',').Select(int.Parse).ToList();
            documents = await _context.Documents.Where(d => idList.Contains(d.Id)).ToListAsync();
        }

        return Json(documents);
    }

    // IDOR через метод запроса
    [HttpGet("documents/user/{userId}")]
    public async Task<IActionResult> GetUserDocumentsByGet(int userId)
    {
        // Защищенная версия
        var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (currentUser != userId)
        {
            return Forbid();
        }

        var documents = await _context.Documents.Where(d => d.UserId == userId).ToListAsync();
        return Json(documents);
    }

    [HttpPost("documents/user/{userId}")]
    public async Task<IActionResult> GetUserDocumentsByPost(int userId)
    {
        // Уязвимость: нет проверки прав доступа при POST запросе
        var documents = await _context.Documents.Where(d => d.UserId == userId).ToListAsync();
        return Json(documents);
    }

    // IDOR с использованием статических ключевых слов
    [HttpGet("documents/current")]
    public async Task<IActionResult> GetCurrentDocument()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.UserId == userId);
        return Json(document);
    }

    [HttpGet("documents/me")]
    public async Task<IActionResult> GetMyDocument()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.UserId == userId);
        return Json(document);
    }

    [HttpGet("documents/static/{keyword}")]
    public async Task<IActionResult> GetDocumentByKeyword(string keyword)
    {
        // Уязвимость: замена ключевых слов на числовые ID
        int? userId = null;

        if (keyword == "current" || keyword == "me")
        {
            userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        else if (int.TryParse(keyword, out int parsedId))
        {
            userId = parsedId;
        }

        if (userId == null)
        {
            return BadRequest();
        }

        var document = await _context.Documents.FirstOrDefaultAsync(d => d.UserId == userId.Value);
        return Json(document);
    }

    // IDOR с непредсказуемыми идентификаторами (UUID)
    [HttpGet("documents/secure/{uuid}")]
    public async Task<IActionResult> GetSecureDocument(string uuid)
    {
        // Уязвимость: UUID можно найти в других эндпоинтах
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id.ToString() == uuid);
        if (document == null)
        {
            return NotFound();
        }

        return Json(document);
    }

    // Для получения UUID (источник утечки)
    [HttpGet("documents/list")]
    public async Task<IActionResult> ListDocuments()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var documents = await _context.Documents
            .Where(d => d.UserId == userId)
            .Select(d => new DocumentViewModel
            {
                Id = d.Id,
                Title = d.Title,
                Uuid = d.Id.ToString("D")
            })
            .ToListAsync();

        return View("List", documents); 
    }
}