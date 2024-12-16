namespace NotificationApi.Processors;

public class SmsProcessor : INotificationProcessor
{
    public async Task SendNotificationAsync(NotificationRequest request)
    {
        Console.WriteLine($"Sending SMS to {request.To}: {request.Body}");
        await Task.CompletedTask;
    }
}