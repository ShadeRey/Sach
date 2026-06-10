using Microsoft.Playwright;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
    Args = new[] { "--no-sandbox", "--disable-blink-features=AutomationControlled" }
});

var context = await browser.NewContextAsync();
var page = await context.NewPageAsync();

var isReady = false;

// Запускаем загрузку браузера в фоне
_ = Task.Run(async () =>
{
    await page.GotoAsync("https://api.stratz.com/graphiql");
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    await Task.Delay(3000);
    isReady = true;
    Console.WriteLine("Браузер готов!");
});

app.MapGet("/", () => isReady ? Results.Ok("ready") : Results.StatusCode(503));

app.MapPost("/hero/{heroId}", async (int heroId, HttpContext httpContext) =>
{
    if (!isReady)
        return Results.StatusCode(503);

    var apiToken = httpContext.Request.Headers["X-Api-Token"].ToString();
    if (string.IsNullOrEmpty(apiToken))
        return Results.Unauthorized();

    try
    {
        var result = await page.EvaluateAsync<string>($@"
            async () => {{
                const response = await fetch('https://api.stratz.com/graphql', {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                        'Accept': 'application/json',
                        'Authorization': 'Bearer {apiToken}'
                    }},
                    body: JSON.stringify({{
                        query: '{{ heroStats {{ heroVsHeroMatchup(heroId: {heroId}) {{ advantage {{ with {{ heroId2 synergy }} vs {{ heroId2 synergy }} }} }} }} }}'
                    }})
                }});
                return await response.text();
            }}
        ");

        Console.WriteLine($"Результат: {result?[..Math.Min(200, result?.Length ?? 0)]}");
        return Results.Content(result, "application/json");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

app.Run("http://localhost:5000");