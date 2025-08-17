using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using Northumbria.Tests.Components;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Northumbria homepage: open, accept cookies, perform a search.</summary>
    public class HomePage : BasePage
    {
        public readonly SiteHeader SiteHeader;
        public HomePage(IPage page) : base(page) => SiteHeader = new SiteHeader(page);

        public async Task OpenAsync()
        {
            await GoToAsync("/");
            await Expect(Page).ToHaveURLAsync(new Regex(@"northumbria\.nhs\.uk", RegexOptions.IgnoreCase));
            await AcceptCookiesIfAnyAsync();
        }

        public async Task AcceptCookiesIfAnyAsync()
        {
            try
            {
                var accept = Page.GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = false }).First;
                if (await accept.IsVisibleAsync(new()))
                    await accept.ClickAsync();
            }
            catch { /* best-effort only */ }
        }

        /// <summary>Type a term into the search input (does not submit).</summary>
        public async Task EnterSearchTermAsync(string term)
        {
            await SiteHeader.EnsureSearchVisibleAsync();
            var search = SiteHeader.GetSearchBox();
            await search.ClickAsync();
            await search.FillAsync(term);
            await Expect(search).ToHaveValueAsync(term);
        }

        /// <summary>Submit search via Enter key (reliable on this site).</summary>
        public async Task SubmitSearchByEnterAsync()
        {
            await Page.Keyboard.PressAsync("Enter");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>Submit search via clicking a "Search" button (if present).</summary>
        public async Task SubmitSearchByButtonAsync()
        {
            var btn = Page.GetByRole(AriaRole.Button, new() { Name = "Search", Exact=false });
            if (!await btn.First.IsVisibleAsync(new()))
                btn = Page.Locator("button:has-text('Search'), [type=submit]");

            if (await btn.First.IsVisibleAsync())
            {
                await Task.WhenAll(
                    Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                    btn.First.ClickAsync()
                );
            }
            else
            {
                // Fallback to Enter if button isn't there
                await SubmitSearchByEnterAsync();
            }
        }
    }
}
