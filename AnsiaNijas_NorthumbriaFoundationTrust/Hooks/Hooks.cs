using System;
using System.IO;
using Reqnroll;
using Microsoft.Playwright;
using NUnit.Framework;
using AnsiaNijas_NorthumbriaFoundationTrust.Support;
using Allure.Net.Commons; // AllureApi.*

namespace Northumbria.Tests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _ctx;
        private IPlaywright? _pw;
        private IBrowser? _browser;
        private TestConfig? _cfg;

        
        private static readonly string ArtifactRoot =
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestResults");

        private static string ScreenshotDir => Path.Combine(ArtifactRoot, "screenshots");
        private static string TraceDir => Path.Combine(ArtifactRoot, "trace");
        private static string AllureResultsDir => Path.Combine(ArtifactRoot, "allure-results");

        public Hooks(ScenarioContext ctx) => _ctx = ctx;

        
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Directory.CreateDirectory(ScreenshotDir);
            Directory.CreateDirectory(TraceDir);
            Directory.CreateDirectory(AllureResultsDir);

            
            Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY", AllureResultsDir);

        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _cfg = TestConfig.Load();

            _pw ??= await Playwright.CreateAsync();
            var browserName = (_cfg.Browser ?? "chrome").Trim().ToLowerInvariant();

            _browser = browserName == "firefox"
                ? await _pw.Firefox.LaunchAsync(new() { Headless = _cfg.Headless })
                : await _pw.Chromium.LaunchAsync(new() { Headless = _cfg.Headless, Channel = "chrome" });

            var context = await _browser.NewContextAsync(new() { BaseURL = _cfg.BaseUrl });

            // Start Playwright tracing
            await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });

            var page = await context.NewPageAsync();
            page.SetDefaultTimeout(_cfg.DefaultTimeoutMs);
            page.SetDefaultNavigationTimeout(_cfg.DefaultTimeoutMs);

            _ctx["Page"] = page;
            _ctx["BaseUrl"] = _cfg.BaseUrl;
            _ctx["BrowserName"] = browserName;

            // Best-effort cookie banner
            var accept = page.GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = false });
            if (await accept.IsVisibleAsync()) await accept.ClickAsync();
        }

        // Screenshot immediately when a step fails
        [AfterStep]
        public async Task AfterStep()
        {
            if (_ctx.TestError == null) return;

            if (_ctx.TryGetValue("Page", out IPage? page) && page is not null)
            {
                var shotPath = Path.Combine(
                    ScreenshotDir,
                    Sanitize($"{_ctx.ScenarioInfo.Title}_stepFail.png"));

                await page.ScreenshotAsync(new() { Path = shotPath, FullPage = true });

                if (File.Exists(shotPath))
                {
                    AllureApi.AddAttachment(
                        $"Failed step - {_ctx.StepContext.StepInfo.Text}",
                        "image/png",
                        shotPath);
                }
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_ctx.TryGetValue("Page", out IPage? page) && page is not null)
            {
                // Stop trace and attach
                var tracePath = Path.Combine(TraceDir, Sanitize(_ctx.ScenarioInfo.Title) + ".zip");
                await page.Context.Tracing.StopAsync(new() { Path = tracePath });
                if (File.Exists(tracePath))
                    AllureApi.AddAttachment("Playwright trace", "application/zip", tracePath);

                // Final screenshot if scenario failed
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    var finalShot = Path.Combine(ScreenshotDir, Sanitize(_ctx.ScenarioInfo.Title) + ".png");
                    await page.ScreenshotAsync(new() { Path = finalShot, FullPage = true });
                    if (File.Exists(finalShot))
                        AllureApi.AddAttachment("Failure screenshot", "image/png", finalShot);
                }

                await page.Context.CloseAsync();
            }

            try
            {
                var browserName = _ctx.TryGetValue("BrowserName", out string? b) ? b : "unknown";
                AllureApi.AddLabel("browser", browserName);
            }
            catch { /* ignore if lifecycle not available */ }

            if (_browser is not null) await _browser.CloseAsync();
            _pw?.Dispose();
        }

        private static string Sanitize(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }
    }
}
