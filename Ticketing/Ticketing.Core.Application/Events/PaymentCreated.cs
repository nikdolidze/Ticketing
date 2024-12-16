namespace Events;

public class PaymentCreated
{
    public Guid NotificationTrackingId { get; set; }

    public DateTime Timestamp { get; set; }

    public string CustomerEmail { get; set; }

    public string CustomerName { get; set; }

    public double OrderAmount { get; set; }
}