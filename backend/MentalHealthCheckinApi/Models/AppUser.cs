using System.ComponentModel.DataAnnotations;

namespace MentalHealthCheckinApi.Models;

public class AppUser
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string Username { get; set; } = default!;

    [MaxLength(32)]
    public string Role { get; set; } = "employee";

    public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
}
