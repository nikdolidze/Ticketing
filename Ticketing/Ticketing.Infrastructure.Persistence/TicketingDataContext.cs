using Microsoft.EntityFrameworkCore;
using Ticketing.Core.Domain.Entities;
using Ticketing.Infrastructure.Persistence.Configurations;

namespace Ticketing.Infrastructure.Persistence;

public class TicketingDataContext : DbContext
{
    public TicketingDataContext(DbContextOptions<TicketingDataContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }

    public DbSet<Price> Prices { get; set; }

    public DbSet<Row> Rows { get; set; }

    public DbSet<Seat> Seats { get; set; }

    public DbSet<Section> Sections { get; set; }

    public DbSet<Venue> Venues { get; set; }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new VenueConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemConfiguration());
    }
}