namespace NotificationApi.Processors;

public interface INotificationProcessor
{
    Task SendNotificationAsync(NotificationRequest request);
}

public class NotificationRequest
{
    public string To { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }
}