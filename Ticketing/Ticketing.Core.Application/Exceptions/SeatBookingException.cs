namespace Ticketing.Core.Application.Exceptions;

public class SeatBookingException : Exception
{
    public SeatBookingException(string message) : base(message)
    {
    }
}