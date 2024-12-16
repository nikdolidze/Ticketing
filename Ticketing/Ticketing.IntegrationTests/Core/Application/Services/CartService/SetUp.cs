using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Ticketing.Core.Domain.Entities;
using Ticketing.Core.Domain.Enums;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.IntegrationTests.Core.Application.Services.CartService;

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

        var seat = new Seat
        {
            SeatNumber = 1,
            SeatStatus = SeatStatus.Available,
            Row = new Row { Id = 1 }
        };

        var price = new Price
        {
            Id = 1,
            Amount = 100
        };

        await context.Seats.AddAsync(seat);
        await context.Prices.AddAsync(price);
        await context.SaveChangesAsync();

        var newCart = new Cart { Id = Constants.CartId, State = CartState.Active };
        await context.Carts.AddAsync(newCart);
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