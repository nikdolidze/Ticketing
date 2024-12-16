using Events;
using MassTransit;
using NotificationApi.Processors;

namespace NotificationApi.Consumers;

public class PaymentCreatedConsumer : IConsumer<PaymentCreated>
{
    private readonly INotificationProcessor _notificationProcessor;

    public PaymentCreatedConsumer(INotificationProcessor notificationProcessor)
    {
        _notificationProcessor = notificationProcessor;
    }

    public async Task Consume(ConsumeContext<PaymentCreated> context)
    {
        var payment = context.Message;

        var notificationRequest = new NotificationRequest
        {
            To = payment.CustomerEmail,
            Subject = "Payment Created",
            Body = $"Your payment of {payment.OrderAmount} has been successfully created. Payment ID: {payment.NotificationTrackingId}"
        };

        await _notificationProcessor.SendNotificationAsync(notificationRequest);
    }
}