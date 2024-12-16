using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Ticketing.Core.Domain.Entities;
using Ticketing.Core.Domain.Enums;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.IntegrationTests.Presentation.Controller.OrdersController;

[SetUpFixture]
public class SetUp
{
    private CustomWebApplicationFactory<Program> _factory;

    [OneTimeSetUp]
    public async Task RunSetup()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        await SeedDatabase();
    }

    private async Task SeedDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<TicketingDataContext>();

        await context.Database.EnsureCreatedAsync();

        var venue = new Venue { Name = "Venue 1", Location = "Location 1" };
        context.Venues.Add(venue);

        var eventEntity = new Event { Name = "Event 1", Date = DateTime.UtcNow.AddMonths(1), Venue = venue };
        context.Events.Add(eventEntity);

        var section = new Section { Name = "Section A", Venue = venue };
        context.Sections.Add(section);

        var row = new Row { RowNumber = 1, Section = section };
        context.Rows.Add(row);

        var seat = new Seat { SeatNumber = 1, SeatStatus = SeatStatus.Available, Row = row };
        context.Seats.Add(seat);

        var price = new Price { Amount = 100m };
        context.Prices.Add(price);

        var cartItem = new CartItem
        {
            SeatId = seat.Id,
            EventId = eventEntity.Id,
            SectionId = section.Id,
            PriceId = price.Id,
            Seat = seat,
            Event = eventEntity,
            Section = section,
            Price = price
        };

        var cart = new Cart
        {
            Id = Constants.CartId,
            Items = [cartItem],
            State = CartState.Active
        };

        context.Carts.Add(cart);
        await context.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task AfterTestRun()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<TicketingDataContext>();

        await context.Database.EnsureDeletedAsync();
    }
}