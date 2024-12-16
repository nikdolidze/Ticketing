using Ticketing.Core.Application.Interfaces.Repositories;

namespace Ticketing.Core.Application.Interfaces;

public interface IUnitOfWork
{
    public IEventRepository EventRepository { get; }

    public IVenueRepository VenueRepository { get; }

    public ICartRepository CartRepository { get; }

    public ISeatRepository SeatRepository { get; }

    public IPaymentRepository PaymentRepository { get; }

    public IPriceRepository PriceRepository { get; }

    public Task<int> SaveChangesAsync();
}