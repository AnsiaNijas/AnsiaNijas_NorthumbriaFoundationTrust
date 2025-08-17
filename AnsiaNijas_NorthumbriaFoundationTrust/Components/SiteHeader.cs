using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Northumbria.Tests.Components
{
    /// <summary>
    /// Reusable site SiteHeader (e.g., search toggle + search box).
    /// </summary>
    public class SiteHeader
    {
        private readonly IPage _page;
        public SiteHeader(IPage page) => _page = page;

        // Likely "Search" toggle (magnifier or button)
        private ILocator SearchToggle =>
            _page.GetByRole(AriaRole.Button, new() { Name = "Search", Exact = false });

        // Candidate search inputs (placeholder used on the site + safe fallbacks)
        private ILocator SearchInputCandidates =>
            _page.GetByPlaceholder("What can we help you to find today?")
                 .Locator(", :scope") // no-op to keep chaining happy
                 .Page.Locator("input[type='search'], input[type='text'][placeholder*='Search' i], input[placeholder*='What can we help you to find today?' i]");

        /// <summary>Ensure the search input is visible (click the SiteHeader toggle if needed).</summary>
        public async Task EnsureSearchVisibleAsync()
        {
            var visible = await SearchInputCandidates.First.IsVisibleAsync(new());
            if (!visible)
            {
                if (await SearchToggle.First.IsVisibleAsync(new()))
                {
                    await SearchToggle.First.ClickAsync();
                    await _page.WaitForTimeoutAsync(150); // allow small animation
                }
            }
        }

        /// <summary>Returns a locator that resolves to the first visible search input.</summary>
        public ILocator GetSearchBox()
        {
            // After EnsureSearchVisibleAsync, pick the first visible candidate
            return SearchInputCandidates.First;
        }
    }
}
