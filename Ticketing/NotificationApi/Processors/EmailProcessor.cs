using Polly;
using Polly.Retry;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NotificationApi.Processors;

public class EmailProcessor : INotificationProcessor
{
    private readonly string _apiKey;
    private readonly string _senderEmail;
    private readonly AsyncRetryPolicy _retryPolicy;

    public EmailProcessor(IConfiguration configuration)
    {
        _apiKey = configuration["SendGrid:ApiKey"]!;
        _senderEmail = configuration["SendGrid:SenderEmail"]!;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2),
                (exception, timeSpan, retryCount, _) => { Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds} seconds due to: {exception.Message}"); });
    }

    public async Task SendNotificationAsync(NotificationRequest request)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_senderEmail, "Epam");
        var to = new EmailAddress("nika_dolidze@epam.com");
        var subject = request.Subject;
        var plainTextContent = request.Body;
        var htmlContent = $"<p>{request.Body}</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        await _retryPolicy.ExecuteAsync(async () =>
        {
            var sendEmailAsync = await client.SendEmailAsync(msg);

            if ((int)sendEmailAsync.StatusCode >= 400) throw new Exception($"Failed to send email. Status code: {sendEmailAsync.StatusCode}");

            Console.WriteLine("Email sent successfully!");
        });
    }
}