using Microsoft.Playwright;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

try
{
    Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to install Chromium: {ex.Message}");
    Environment.Exit(1);
}

IPlaywright playwright = null!;
IBrowser browser = null!;
IBrowserContext context = null!;
IPage page = null!;

try
{
    playwright = await Playwright.CreateAsync();
    browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = true,
        Args = new[] { "--no-sandbox", "--disable-blink-features=AutomationControlled" }
    });

    context = await browser.NewContextAsync(new BrowserNewContextOptions
    {
        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
    });
    page = await context.NewPageAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to launch browser: {ex.Message}");
    Environment.Exit(1);
}

var isReady = true;

Console.WriteLine("Server started and ready to accept requests");

app.MapGet("/", () =>
{
    Console.WriteLine("Health check received");
    return Results.Ok("ready");
});

app.MapPost("/hero/{heroId}", async (int heroId, HttpContext httpContext) =>
{
    if (!isReady)
        return Results.StatusCode(503);

    var apiToken = httpContext.Request.Headers["X-Api-Token"].ToString();
    if (string.IsNullOrEmpty(apiToken))
        return Results.Unauthorized();

    try
    {
        // Set up route interception to add auth header
        await context.RouteAsync("**/api.stratz.com/graphql", async route =>
        {
            var headers = new Dictionary<string, string>(route.Request.Headers)
            {
                { "Authorization", $"Bearer {apiToken}" }
            };
            await route.ContinueAsync(new RouteContinueOptions { Headers = headers });
        });

        var query = new
        {
            query = $@"{{ heroStats {{ heroVsHeroMatchup(heroId: {heroId}) {{ advantage {{ with {{ heroId2 synergy }} vs {{ heroId2 synergy }} }} }} }} }}"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(query);

        var result = await page.EvaluateAsync<string>(@"
            async (jsonStr) => {
                const response = await fetch('https://api.stratz.com/graphql', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: jsonStr
                });
                if (!response.ok) {
                    return JSON.stringify({ error: 'HTTP ' + response.status });
                }
                return await response.text();
            }
        ", json);

        Console.WriteLine($"Result: {result?[..Math.Min(200, result?.Length ?? 0)]}");
        return Results.Content(result, "application/json");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

app.Run("http://localhost:5000");