using System.Diagnostics;

var startTime = Stopwatch.GetTimestamp();

var builder = WebApplication.CreateSlimBuilder(args);

GeneratedServiceRegistrations.PaymentServiceServiceRegistration.Register(builder.Services);

var app = builder.Build();

app.MapGet("/payments", (int? userId, IPaymentService paymentService) =>
{
    if (userId == null)
    {
        return Results.BadRequest("User ID is required.");
    }

    var result = paymentService.ProcessPayment(new PaymentRequest(userId.Value));
    return Results.Json(result, MyJsonContext.Default.String);
});

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => { Console.WriteLine($"Startup time: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms"); });

app.Run();