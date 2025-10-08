
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalHealthCheckinApi.Controllers;
using MentalHealthCheckinApi.Data;
using MentalHealthCheckinApi.Dtos;
using MentalHealthCheckinApi.Models;

namespace MentalHealthCheckinApi.Tests.Controllers;

public class CheckinsControllerTests
{
    private static AppDbContext SeedDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"tests-{Guid.NewGuid()}")
            .Options;

        var db = new AppDbContext(options);

        var alice = new AppUser { Id = Guid.NewGuid(), Username = "alice", Role = "employee" };
        var bob = new AppUser { Id = Guid.NewGuid(), Username = "bob", Role = "manager" };

        db.Users.AddRange(alice, bob);

        db.CheckIns.AddRange(
            new CheckIn { Id = Guid.NewGuid(), UserId = alice.Id, Mood = 5, Notes = "A", CreatedAt = DateTimeOffset.Parse("2025-10-05T10:00:00Z") },
            new CheckIn { Id = Guid.NewGuid(), UserId = alice.Id, Mood = 3, Notes = "B", CreatedAt = DateTimeOffset.Parse("2025-10-07T10:00:00Z") },
            new CheckIn { Id = Guid.NewGuid(), UserId = bob.Id, Mood = 2, Notes = "C", CreatedAt = DateTimeOffset.Parse("2025-10-06T10:00:00Z") }
        );

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task List_Filters_By_User_And_DateRange_Returns_Ok_With_Expected_Item()
    {
        using var db = SeedDb();

        // If your class is named CheckInsController (capital I), change the type below accordingly.
        var controller = new CheckInsController(db);

        // inclusive: 2025-10-06 .. 2025-10-07 (your action uses < to+1 day)
        var from = new DateTime(2025, 10, 06, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2025, 10, 07, 0, 0, 0, DateTimeKind.Utc);

        var result = await controller.List("alice", from, to);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<CheckInDto>>(ok.Value);
        var items = list.ToList();

        items.Should().HaveCount(1);
        items[0].Username.Should().Be("alice");
        items[0].Mood.Should().Be(3);
        items[0].Notes.Should().Be("B");
    }
}
