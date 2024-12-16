using Microsoft.AspNetCore.Mvc;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Enums;

namespace Ticketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{paymentId}")]
    public async Task<ActionResult<PaymentStatus>> GetPaymentStatus(int paymentId)
    {
        var payment = await _paymentService.GetPayment(paymentId);
        if (payment?.Status == null) return NoContent();

        return Ok(payment.Status);
    }

    [HttpPost("{paymentId}/failed")]
    public async Task<IActionResult> FailPayment(int paymentId)
    {
        try
        {
            await _paymentService.FailPaymentAsync(paymentId);
            return Ok(new { Message = "Payment marked as failed and seats are now available." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // TODO error handling should be done across application
        }
    }

    [HttpPost("{paymentId}/complete")]
    public async Task<IActionResult> CompletePayment(int paymentId)
    {
        try
        {
            await _paymentService.CompletePaymentAsync(paymentId);
            return Ok(new { Message = "Payment completed and seats moved to sold state." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}