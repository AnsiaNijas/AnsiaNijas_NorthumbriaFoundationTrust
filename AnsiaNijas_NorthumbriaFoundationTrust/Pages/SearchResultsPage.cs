using System.Threading.Tasks;
using AnsiaNijas_NorthumbriaFoundationTrust.Utilities;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Search results page: verify results and open a specific result.</summary>
    public class SearchResultsPage : GeneralUtils
    {
        public SearchResultsPage(IPage page) : base(page) { }

        // === Your exact property line ===
        public ILocator searchResults => Page.GetByRole(AriaRole.Link, new() { Name = "1000 Pages found that matched" });
        public ILocator pageResults => Page.GetByRole(AriaRole.Heading, new() { Name = "Page results" });
        public ILocator ResultsContainer => Page.Locator("#page-results, .page-results");
        public ILocator ResultLink(string title) =>
        ResultsContainer.GetByRole(AriaRole.Link, new() { Name = title, Exact = true });
        public ILocator QaSHeading => Page.GetByRole(AriaRole.Heading,new () { Name= "Quality and safety" });
        public ILocator mainHeading => Page.Locator(".main-breadcrumbs > .container > .row > .col-xs-24 > .core-style");
        public ILocator cISBox => Page.GetByRole(AriaRole.Link, new () { Name = "Continually improving services" });

// Assert + click
public async Task AssertHasResultsForAsync(string title)
        {
            await Assertions.Expect(searchResults).ToBeVisibleAsync();
            await Assertions.Expect(pageResults).ToBeVisibleAsync();
            await Expect(ResultLink(title)).ToBeVisibleAsync();
        }

        /// <summary>Clicks a result link by its visible title, then waits for navigation.</summary>
        public async Task ClickResultAsync(string title)
        {
            var link = ResultLink(title);
            await Expect(link).ToBeVisibleAsync();
            await Task.WhenAll(
                Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                link.ClickAsync()

            );
            await Expect(QaSHeading).ToBeVisibleAsync();
            await Expect(mainHeading).ToBeVisibleAsync();
            await Expect(cISBox).ToBeVisibleAsync();
        }
    }
}
