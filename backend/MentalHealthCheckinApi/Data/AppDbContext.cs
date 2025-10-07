using MentalHealthCheckinApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthCheckinApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired().HasMaxLength(64);
            e.Property(u => u.Role).IsRequired().HasMaxLength(32);
            e.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<CheckIn>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Mood).IsRequired();       // 1–5
            e.Property(c => c.Notes).HasMaxLength(1000);
            e.Property(c => c.CreatedAt).IsRequired();
            e.HasOne(c => c.User)
             .WithMany(u => u.CheckIns)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
