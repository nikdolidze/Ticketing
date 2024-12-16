using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ticketing.Controllers;
using Ticketing.Core.Application.DTOs.Payment;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Enums;
using Xunit;

namespace Ticketing.UnitTests.Presentation.ControllerTests;

public class PaymentControllerTests
{
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _controller = new PaymentController(_mockPaymentService.Object);
    }

    [Fact]
    public async Task GetPaymentStatus_ShouldReturnOk_WhenPaymentExists()
    {
        // Arrange
        var paymentId = 1;
        var paymentStatus = PaymentStatus.Completed; // Or whatever status you want to return
        var payment = new PaymentDto() { Status = paymentStatus };

        _mockPaymentService.Setup(ps => ps.GetPayment(paymentId)).ReturnsAsync(payment);

        // Act
        var result = await _controller.GetPaymentStatus(paymentId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().Be(paymentStatus);
    }

    [Fact]
    public async Task GetPaymentStatus_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        // Arrange
        var paymentId = 1;
        _mockPaymentService.Setup(ps => ps.GetPayment(paymentId)).ReturnsAsync(default(PaymentDto?));

        // Act
        var result = await _controller.GetPaymentStatus(paymentId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Value.Should().Be("Payment not found");
    }

    [Fact]
    public async Task FailPayment_ShouldReturnOk_WhenPaymentFails()
    {
        // Arrange
        var paymentId = 1;

        _mockPaymentService.Setup(ps => ps.FailPaymentAsync(paymentId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.FailPayment(paymentId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(new { Message = "Payment marked as failed and seats are now available." });
    }

    [Fact]
    public async Task FailPayment_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var paymentId = 1;
        var exceptionMessage = "Payment not found";
        _mockPaymentService.Setup(ps => ps.FailPaymentAsync(paymentId)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.FailPayment(paymentId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task CompletePayment_ShouldReturnOk_WhenPaymentCompletesSuccessfully()
    {
        // Arrange
        var paymentId = 1;

        _mockPaymentService.Setup(ps => ps.CompletePaymentAsync(paymentId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CompletePayment(paymentId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(new { Message = "Payment completed and seats moved to sold state." });
    }

    [Fact]
    public async Task CompletePayment_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var paymentId = 1;
        var exceptionMessage = "Payment could not be completed";
        _mockPaymentService.Setup(ps => ps.CompletePaymentAsync(paymentId)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.CompletePayment(paymentId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().Be(exceptionMessage);
    }
}