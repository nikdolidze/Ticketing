using Microsoft.Extensions.DependencyInjection;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Application.Services.Wrappers;

namespace Ticketing.Core.Application;

public static class ServiceExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IVenueService, VenueService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddScoped<ICacheWrapper, MemoryCacheWrapper>();
    }
}