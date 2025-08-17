using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Search results page: verify results and open a specific result.</summary>
    public class SearchResultsPage : BasePage
    {
        public SearchResultsPage(IPage page) : base(page) { }

        // === Your exact property line ===
        public ILocator ResultsContainer => Page.Locator("#page-results, .page-results");

        private ILocator ResultTitleLinks =>
            ResultsContainer.Locator("ul.results li.search-result p.title a:not([aria-hidden='true'])");

        private ILocator ResultLinkTitled(string text) =>
            ResultTitleLinks.Filter(new() { HasText = text }).First;

        /// <summary>Asserts that results container is visible and at least one link mentions the term.</summary>
        public async Task AssertHasResultsForAsync(string term)
        {
            await Expect(ResultsContainer).ToBeVisibleAsync();

            // Ensure there is at least 1 result link
            var count = await ResultTitleLinks.CountAsync();
            if (count == 0)
                throw new PlaywrightException("No search result links found under Page results.");

            // At least one link should contain the term
            await Expect(ResultTitleLinks).ToContainTextAsync(term, new() { IgnoreCase = true });
        }

        /// <summary>Clicks a result link by its visible title, then waits for navigation.</summary>
        public async Task ClickResultAsync(string linkText)
        {
            var link = ResultLinkTitled(linkText);
            await Expect(link).ToBeVisibleAsync();
            await Task.WhenAll(
                Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                link.ClickAsync()
            );
        }
    }
}
