using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Infrastructure.Persistence.Implementation;

namespace Ticketing.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<TicketingDataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}