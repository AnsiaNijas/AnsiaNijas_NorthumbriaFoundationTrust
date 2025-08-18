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

        public ILocator ResultLink(string title) =>
        ResultsContainer.GetByRole(AriaRole.Link, new() { Name = title, Exact = true });

        // Assert + click
        public async Task AssertHasResultsForAsync(string title)
        {
            await Expect(ResultLink(title)).ToBeVisibleAsync();
        }

        /// <summary>Clicks a result link by its visible title, then waits for navigation.</summary>
        public async Task ClickResultAsync(string linkText)
        {
            var link = ResultLink(linkText);
            await Expect(link).ToBeVisibleAsync();
            await Task.WhenAll(
                Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                link.ClickAsync()
            );
        }
    }
}
