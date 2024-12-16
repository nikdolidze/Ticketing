using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Implementation.Repositories;

public class PaymentRepository : Repository<Payment, int>, IPaymentRepository
{
    public PaymentRepository(TicketingDataContext context) : base(context)
    {
    }
}