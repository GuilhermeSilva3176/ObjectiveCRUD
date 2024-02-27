using API.Interfaces;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UsersModel> Users { get; set; }

    public DbSet<EventsModel> Events { get; set; }

    public DbSet<RegistrationModel> Registration { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventsModel>()
            .HasOne(e => e.User)
            .WithMany(u => u.Events)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RegistrationModel>()
            .HasOne(e => e.Users)
            .WithMany(u => u.Registration)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RegistrationModel>()
            .HasOne(e => e.Events) 
            .WithMany(u => u.Registration) 
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
