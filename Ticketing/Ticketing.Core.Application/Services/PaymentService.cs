using Ticketing.Core.Application.DTOs.Payment;
using Ticketing.Core.Application.Exceptions;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CompletePaymentAsync(int paymentId)
    {
        var payment = await _unitOfWork.PaymentRepository.ReadAsync(paymentId, x => x.PurchasedSeats);
        if (payment == null) throw new PaymentException("Payment is Is not correct");

        payment.Status = PaymentStatus.Completed;

        payment.PurchasedSeats.ForEach(x => x.SeatStatus = SeatStatus.Sold);

        await _unitOfWork.PaymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task FailPaymentAsync(int paymentId)
    {
        var payment = await _unitOfWork.PaymentRepository.ReadAsync(paymentId, x => x.PurchasedSeats);
        if (payment == null) throw new PaymentException("Payment is Is not correct");

        payment.Status = PaymentStatus.Failed;

        payment.PurchasedSeats.ForEach(x => x.SeatStatus = SeatStatus.Available);

        await _unitOfWork.PaymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PaymentDto?> GetPayment(int paymentId)
    {
        var result = await _unitOfWork.PaymentRepository.ReadAsync(paymentId, x => x.PurchasedSeats);
        if (result == null) return new PaymentDto();

        return new PaymentDto()
        {
            Status = result.Status
        };
    }
}

public interface IPaymentService
{
    Task<PaymentDto?> GetPayment(int paymentId);

    public Task CompletePaymentAsync(int paymentId);

    public Task FailPaymentAsync(int paymentId);
}