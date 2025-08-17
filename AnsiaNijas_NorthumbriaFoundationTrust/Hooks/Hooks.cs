using Reqnroll;
using Microsoft.Playwright;

namespace Northumbria.Tests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _ctx;
        private IPlaywright? _pw;
        private IBrowser? _browser;

        public Hooks(ScenarioContext ctx) => _ctx = ctx;

        private static string BaseUrl =>
            Environment.GetEnvironmentVariable("BASE_URL")?.TrimEnd('/')
            ?? "https://www.northumbria.nhs.uk";
        private static string BrowserName =>
            (Environment.GetEnvironmentVariable("BROWSER") ?? "chromium").ToLowerInvariant();
        private static bool Headless =>
            !string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "false", StringComparison.OrdinalIgnoreCase);

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _pw ??= await Playwright.CreateAsync();

            _browser = BrowserName == "firefox"
                ? await _pw.Firefox.LaunchAsync(new() { Headless = Headless })
                : await _pw.Chromium.LaunchAsync(new() { Headless = Headless, Channel = "chrome" });

            var context = await _browser.NewContextAsync(new() { BaseURL = BaseUrl });
            Directory.CreateDirectory("TestResults");
            await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });

            var page = await context.NewPageAsync();
            page.SetDefaultTimeout(10000);

            // Share the page with steps
            _ctx["Page"] = page;

            // Optional: handle cookie banner quickly if it shows up
            var accept = page.GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = false });
            if (await accept.IsVisibleAsync()) await accept.ClickAsync();
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_ctx.TryGetValue("Page", out IPage? page) && page is not null)
            {
                var trace = Path.Combine("TestResults", Sanitize(_ctx.ScenarioInfo.Title) + ".zip");
                await page.Context.Tracing.StopAsync(new() { Path = trace });
                await page.Context.CloseAsync();
            }

            if (_browser is not null)
                await _browser.CloseAsync();
        }

        private static string Sanitize(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }
    }
}
