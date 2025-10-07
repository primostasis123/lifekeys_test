using Microsoft.EntityFrameworkCore;
using MentalHealthCheckinApi.Models;

namespace MentalHealthCheckinApi.Data;

public class DbSeeder
{
    private readonly AppDbContext _db;

    public DbSeeder(AppDbContext db) => _db = db;

    public async Task SeedAsync()
    {
        // Always ensure users exist
        if (!await _db.Users.AnyAsync())
        {
            var aliceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var bobId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var charlieId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var dianaId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var eveId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var frankId = Guid.Parse("66666666-6666-6666-6666-666666666666");

            _db.Users.AddRange(new[]
            {
            new AppUser { Id = aliceId,   Username = "alice",   Role = "employee" },
            new AppUser { Id = bobId,     Username = "bob",     Role = "manager"  },
            new AppUser { Id = charlieId, Username = "charlie", Role = "employee" },
            new AppUser { Id = dianaId,   Username = "diana",   Role = "manager"  },
            new AppUser { Id = eveId,     Username = "eve",     Role = "employee" },
            new AppUser { Id = frankId,   Username = "frank",   Role = "manager"  }
        });

            await _db.SaveChangesAsync();
        }

        // Always reseed check-ins
        if (!await _db.CheckIns.AnyAsync())
        {
            var now = DateTimeOffset.UtcNow;

            var users = await _db.Users.ToListAsync();

            var aliceId = users.First(u => u.Username == "alice").Id;
            var bobId = users.First(u => u.Username == "bob").Id;
            var charlieId = users.First(u => u.Username == "charlie").Id;
            var dianaId = users.First(u => u.Username == "diana").Id;
            var eveId = users.First(u => u.Username == "eve").Id;
            var frankId = users.First(u => u.Username == "frank").Id;

            _db.CheckIns.AddRange(new[]
            {
            new CheckIn { Id = Guid.NewGuid(), UserId = aliceId,   Mood = 4, Notes = "Feeling productive.", CreatedAt = now.AddDays(-2) },
            new CheckIn { Id = Guid.NewGuid(), UserId = aliceId,   Mood = 2, Notes = "A bit stressed.",     CreatedAt = now.AddDays(-1) },
            new CheckIn { Id = Guid.NewGuid(), UserId = bobId,     Mood = 5, Notes = "Great day!",          CreatedAt = now.AddDays(-3) },
            new CheckIn { Id = Guid.NewGuid(), UserId = charlieId, Mood = 3, Notes = "Normal workload.",    CreatedAt = now.AddDays(-2) },
            new CheckIn { Id = Guid.NewGuid(), UserId = dianaId,   Mood = 5, Notes = "Team exceeded targets.", CreatedAt = now.AddDays(-4) },
            new CheckIn { Id = Guid.NewGuid(), UserId = eveId,     Mood = 4, Notes = "Good progress.",      CreatedAt = now.AddDays(-2) },
            new CheckIn { Id = Guid.NewGuid(), UserId = frankId,   Mood = 3, Notes = "Busy with reports.",  CreatedAt = now.AddDays(-5) }
        });

            await _db.SaveChangesAsync();
        }
    }
}
