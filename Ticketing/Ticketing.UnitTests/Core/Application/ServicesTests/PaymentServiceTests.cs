using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Entities;
using Ticketing.Core.Domain.Enums;
using Xunit;

namespace Ticketing.UnitTests.Core.Application.ServicesTests;

public class PaymentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _paymentService = new PaymentService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CompletePaymentAsync_WhenPaymentExists_ShouldCompletePayment()
    {
        // Arrange
        var paymentId = 1;
        var payment = new Payment
        {
            Status = PaymentStatus.Pending,
            PurchasedSeats = new List<Seat> { new() { SeatStatus = SeatStatus.Available } }
        };

        _mockUnitOfWork.Setup(u => u.PaymentRepository.ReadAsync(paymentId,
            It.IsAny<Expression<Func<Payment, object>>>())).ReturnsAsync(payment);

        // Act
        await _paymentService.CompletePaymentAsync(paymentId);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.PurchasedSeats.ForEach(seat => seat.SeatStatus.Should().Be(SeatStatus.Sold));

        _mockUnitOfWork.Verify(u => u.PaymentRepository.UpdateAsync(payment), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CompletePaymentAsync_WhenPaymentDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var paymentId = 1;

        _mockUnitOfWork.Setup(u => u.PaymentRepository.ReadAsync(paymentId))
            .ReturnsAsync((Payment)null); // No payment found

        // Act
        var act = async () => await _paymentService.CompletePaymentAsync(paymentId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Payment is Is not correct");
    }

    [Fact]
    public async Task FailPaymentAsync_WhenPaymentExists_ShouldFailPayment()
    {
        // Arrange
        var paymentId = 1;
        var payment = new Payment
        {
            Status = PaymentStatus.Pending,
            PurchasedSeats = new List<Seat> { new() { SeatStatus = SeatStatus.Sold } }
        };

        _mockUnitOfWork.Setup(u => u.PaymentRepository.ReadAsync(paymentId, It.IsAny<Expression<Func<Payment, object>>>()))
            .ReturnsAsync(payment);

        // Act
        await _paymentService.FailPaymentAsync(paymentId);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.PurchasedSeats.ForEach(seat => seat.SeatStatus.Should().Be(SeatStatus.Available));

        _mockUnitOfWork.Verify(u => u.PaymentRepository.UpdateAsync(payment), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task FailPaymentAsync_WhenPaymentDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var paymentId = 1;

        _mockUnitOfWork.Setup(u => u.PaymentRepository.ReadAsync(paymentId))
            .ReturnsAsync((Payment)null); // No payment found

        // Act
        var act = async () => await _paymentService.FailPaymentAsync(paymentId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Payment is Is not correct");
    }

    [Fact]
    public async Task GetPayment_WhenPaymentExists_ReturnsPaymentDto()
    {
        // Arrange
        var paymentId = 1;
        var payment = new Payment
        {
            Status = PaymentStatus.Completed,
            PurchasedSeats = new List<Seat>()
        };

        _mockUnitOfWork.Setup(u => u.PaymentRepository.ReadAsync(paymentId, It.IsAny<Expression<Func<Payment, object>>>()))
            .ReturnsAsync(payment);

        // Act
        var result = await _paymentService.GetPayment(paymentId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Completed);
    }

    [Fact]
    public async Task GetPayment_WhenPaymentDoesNotExist_ReturnsEmptyPaymentDto()
    {
        // Arrange
        var paymentId = 1;

        _mockUnitOfWork.Setup(u => u.PaymentRepository.ReadAsync(paymentId, It.IsAny<Expression<Func<Payment, object>>>()))
            .ReturnsAsync((Payment)null);

        // Act
        var result = await _paymentService.GetPayment(paymentId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeNull();
    }
}