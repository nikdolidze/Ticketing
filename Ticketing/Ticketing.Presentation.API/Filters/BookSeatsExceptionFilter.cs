using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Ticketing.Core.Application.Exceptions;

namespace Ticketing.Filters;

public class BookSeatsExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<BookSeatsExceptionFilter> _logger;

    public BookSeatsExceptionFilter(ILogger<BookSeatsExceptionFilter> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An error occurred: {Message}", context.Exception.Message);

        if (context.Exception is SeatBookingException seatBookingException)
            context.Result = new BadRequestObjectResult(new
            {
                message = seatBookingException.Message
            });

        else if (context.Exception is DbUpdateConcurrencyException concurrencyException)
            context.Result = new ConflictObjectResult(new
            {
                message = concurrencyException.Message
            });
        else
            context.Result = new BadRequestObjectResult(new
            {
                message = "An unexpected error occurred while processing your request."
            });

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}