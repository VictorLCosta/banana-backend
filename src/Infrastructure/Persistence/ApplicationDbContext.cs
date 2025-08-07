using Application.Core.Identity.Users.Abstractions;
using Application.Core.Interfaces;

using Domain.Entities;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUser currentUser) : 
    BaseDbContext(options, currentUser), IApplicationDbContext
{
    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}