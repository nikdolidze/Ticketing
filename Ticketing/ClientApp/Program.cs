using NBomber.CSharp;

namespace ClientApp;

public static class Program
{
    private static readonly HttpClient HttpClient = new();
    private const string BaseUrl = "https://localhost:44382/";
    private const string CartId = "F1BFB41F-6A21-4351-6010-08DCE915D9B8";
    private static int _successfulCount = 0;

    private static Task Main(string[] args)
    {
        var scenario = Scenario.Create("book_seat_scenario", async context =>
            {
                var requestUrl = $"{BaseUrl}api/Orders/carts/{CartId}/book";
                var response = await HttpClient.PutAsync(requestUrl, null);
                if (response.IsSuccessStatusCode)
                {
                    Interlocked.Increment(ref _successfulCount);
                    return Response.Ok();
                }

                return Response.Fail();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.KeepConstant(1000, TimeSpan.FromSeconds(3))
            );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        Console.WriteLine($"Number of successful bookings: {_successfulCount}");
        return Task.CompletedTask;
    }
}