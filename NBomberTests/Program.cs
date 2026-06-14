using NBomber.CSharp;
using NBomber.Http.CSharp;

using var httpClient = new HttpClient();

var scenario = Scenario.Create("get_active_visits", async context =>
{
    var request = Http.CreateRequest("GET", "http://localhost:5095/api/visits/active");
    var response = await Http.Send(httpClient, request);

    return response;
})
.WithoutWarmUp()
.WithLoadSimulations(
    // 50 requests/sec for 2 seconds = 100 requests total, up to 50 in flight at once.
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(2))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
