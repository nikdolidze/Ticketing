using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Ticketing.Core.Domain.Entities;
using Ticketing.Core.Domain.Enums;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.IntegrationTests.Presentation.Controller.OrdersController;

[TestFixture]
public class OrdersControllerTests
{
    private HttpClient _client;
    private CustomWebApplicationFactory<Program> _factory;

    public OrdersControllerTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task BookSeats_ReturnsOk_WhenCartIsValid()
    {
        // Arrange
        var cartId = Constants.CartId;

        // Act
        var response = await _client.PutAsync($"api/orders/carts/{cartId}/book", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        Cart? seat;
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<TicketingDataContext>();

            seat = await context.Carts.Include(x => x.Items).ThenInclude(x => x.Seat).FirstOrDefaultAsync();
        }

        seat.Items.First().Seat.SeatStatus.Should().Be(SeatStatus.Booked);
    }
}