using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalHealthCheckinApi.Data;
using MentalHealthCheckinApi.Dtos;
using MentalHealthCheckinApi.Models;

namespace MentalHealthCheckinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CheckInsController(AppDbContext db) => _db = db;

    // POST: api/checkins
    [HttpPost]
    public async Task<ActionResult<CheckInDto>> Create([FromBody] CheckInCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var username = User.Identity?.Name ?? "unknown";

        var checkIn = new CheckIn
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Mood = dto.Mood,
            Notes = dto.Notes,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.CheckIns.Add(checkIn);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = checkIn.Id },
            new CheckInDto(checkIn.Id, userId, username, checkIn.Mood, checkIn.Notes, checkIn.CreatedAt));
    }

    // GET: api/checkins?user=alice&from=2025-10-01&to=2025-10-08
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckInDto>>> List([FromQuery] string? user, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _db.CheckIns.Include(c => c.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(user))
            query = query.Where(c => c.User!.Username == user);

        if (from.HasValue)
        {
            var fUtc = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
            var fOff = new DateTimeOffset(fUtc);
            query = query.Where(c => c.CreatedAt >= fOff);
        }

        if (to.HasValue)
        {
            var tUtcExclusive = DateTime.SpecifyKind(to.Value.Date.AddDays(1), DateTimeKind.Utc);
            var tOffExclusive = new DateTimeOffset(tUtcExclusive);
            query = query.Where(c => c.CreatedAt < tOffExclusive);
        }

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CheckInDto(
                c.Id, c.UserId, c.User!.Username, c.Mood, c.Notes, c.CreatedAt))
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/checkins/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CheckInDto>> GetById(Guid id)
    {
        var checkIn = await _db.CheckIns.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        if (checkIn is null) return NotFound();

        var isManager = User.IsInRole("manager");
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!isManager && checkIn.UserId != currentUserId) return Forbid();

        return new CheckInDto(checkIn.Id, checkIn.UserId, checkIn.User!.Username, checkIn.Mood, checkIn.Notes, checkIn.CreatedAt);
    }

    // PUT: api/checkins/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CheckInDto>> Update(Guid id, [FromBody] CheckInUpdateDto dto)
    {
        var checkIn = await _db.CheckIns.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        if (checkIn is null) return NotFound();

        var isManager = User.IsInRole("manager");
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!isManager && checkIn.UserId != currentUserId) return Forbid();

        checkIn.Mood = dto.Mood;
        checkIn.Notes = dto.Notes;

        await _db.SaveChangesAsync();

        return new CheckInDto(checkIn.Id, checkIn.UserId, checkIn.User!.Username, checkIn.Mood, checkIn.Notes, checkIn.CreatedAt);
    }
}
