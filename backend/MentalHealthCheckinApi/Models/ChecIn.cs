using System.ComponentModel.DataAnnotations;

namespace MentalHealthCheckinApi.Models;

public class CheckIn
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; }

    [Range(1, 5)]
    public int Mood { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
