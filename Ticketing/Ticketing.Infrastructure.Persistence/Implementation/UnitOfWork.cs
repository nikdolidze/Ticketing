using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Infrastructure.Persistence.Implementation.Repositories;

namespace Ticketing.Infrastructure.Persistence.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicketingDataContext _context;
    private ICartRepository _cartRepository;
    private IEventRepository _eventRepository;
    private ISeatRepository _seatRepository;
    private IVenueRepository _venueRepository;
    private IPaymentRepository _paymentRepository;
    private IPriceRepository _priceRepository;

    public UnitOfWork(TicketingDataContext context)
    {
        _context = context;
    }

    public IEventRepository EventRepository => _eventRepository ??= new EventRepository(_context);

    public IVenueRepository VenueRepository => _venueRepository ??= new VenueRepository(_context);

    public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_context);

    public ISeatRepository SeatRepository => _seatRepository ??= new SeatRepository(_context);

    public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);

    public IPriceRepository PriceRepository => _priceRepository ??= new PriceRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}