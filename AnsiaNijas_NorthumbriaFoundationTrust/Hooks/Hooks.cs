using Reqnroll;
using Microsoft.Playwright;
using NUnit.Framework; // TestContext
using AnsiaNijas_NorthumbriaFoundationTrust.Support;

namespace Northumbria.Tests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _ctx;
        private IPlaywright? _pw;
        private IBrowser? _browser;

        public Hooks(ScenarioContext ctx) => _ctx = ctx;

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            // 1) Read settings from appsettings.json (+ env overrides)
            var cfg = TestConfig.Load();

            // 2) Playwright + Browser
            _pw ??= await Playwright.CreateAsync();
            _browser = cfg.Browser.ToLowerInvariant() == "firefox"
                ? await _pw.Firefox.LaunchAsync(new() { Headless = cfg.Headless })
                : await _pw.Chromium.LaunchAsync(new() { Headless = cfg.Headless, Channel = "chrome" });

            // 3) Context with BaseURL from config
            var artifactsDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults");
            Directory.CreateDirectory(artifactsDir);

            var context = await _browser.NewContextAsync(new() { BaseURL = cfg.BaseUrl });

            // Tracing (no video)
            await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });

            // 4) Page + timeouts
            var page = await context.NewPageAsync();
            page.SetDefaultTimeout(cfg.DefaultTimeoutMs);
            page.SetDefaultNavigationTimeout(cfg.DefaultTimeoutMs);

            // 5) Share for steps/pages
            _ctx["Page"] = page;
            _ctx["BaseUrl"] = cfg.BaseUrl; 

            // 6) Cookie banner (best effort)
            var accept = page.GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = false });
            if (await accept.IsVisibleAsync()) await accept.ClickAsync();

            // Allure Report

        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_ctx.TryGetValue("Page", out IPage? page) && page is not null)
            {
                var tracePath = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "TestResults",
                    Sanitize(_ctx.ScenarioInfo.Title) + ".zip");

                await page.Context.Tracing.StopAsync(new() { Path = tracePath });
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
