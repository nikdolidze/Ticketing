using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ticketing.Infrastructure.Persistence;

public class TicketingFactory : IDesignTimeDbContextFactory<TicketingDataContext>
{
    public TicketingDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TicketingDataContext>();
        optionsBuilder.UseSqlServer("data source=localhost;initial catalog=Ticketing;trusted_connection=true;TrustServerCertificate=True");

        return new TicketingDataContext(optionsBuilder.Options);
    }
}