using Ticketing.Core.Domain.Entities;

namespace Ticketing.Core.Application.Interfaces.Repositories;

public interface IPaymentRepository : IRepository<Payment, int>
{
}