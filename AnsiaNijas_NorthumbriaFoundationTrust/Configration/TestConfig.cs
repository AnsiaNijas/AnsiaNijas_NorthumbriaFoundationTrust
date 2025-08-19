using System.Text.Json;
using Allure.NUnit.Attributes;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Support
{
    public sealed class TestConfig
    {
        public string BaseUrl { get; set; } = "https://www.northumbria.nhs.uk";
        public string Browser { get; set; } = "chrome";
        public bool Headless { get; set; } = true;
        public int DefaultTimeoutMs { get; set; } = 10000;


        public static TestConfig Load()
        {
            var cfg = new TestConfig();

            // Read appsettings.json from output folder (bin/.../net8.0)
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (File.Exists(path))
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                if (doc.RootElement.TryGetProperty("TestSettings", out var s))
                {
                    if (s.TryGetProperty("BaseUrl", out var v)) cfg.BaseUrl = v.GetString() ?? cfg.BaseUrl;
                    if (s.TryGetProperty("Browser", out v)) cfg.Browser = v.GetString() ?? cfg.Browser;
                    if (s.TryGetProperty("Headless", out v)) cfg.Headless = v.GetBoolean();
                    if (s.TryGetProperty("DefaultTimeoutMs", out v)) cfg.DefaultTimeoutMs = v.GetInt32();
                }
            }

            // Env vars override JSON (handy for CI)
            cfg.BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? cfg.BaseUrl;
            cfg.Browser = Environment.GetEnvironmentVariable("BROWSER") ?? cfg.Browser;
            if (bool.TryParse(Environment.GetEnvironmentVariable("HEADLESS"), out var h)) cfg.Headless = h;
            if (int.TryParse(Environment.GetEnvironmentVariable("PW_TIMEOUT_MS"), out var t)) cfg.DefaultTimeoutMs = t;

            return cfg;
        }
    }
}
