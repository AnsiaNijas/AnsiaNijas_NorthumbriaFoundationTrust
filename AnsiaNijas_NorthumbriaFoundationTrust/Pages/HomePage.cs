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
        private readonly string _baseUrl;                
        public readonly SiteHeader SiteHeader;

        public HomePage(IPage page, string baseUrl)       
            : base(page)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            SiteHeader = new SiteHeader(page);
        }

        public ILocator cookieAccept => Page.GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = false }).First;
        public ILocator searchButton =>  Page.GetByRole(AriaRole.Button, new() { Name = "Search", Exact = false });
        public ILocator searchButtonByText => Page.Locator("button:has-text('Search'), [type=submit]");
        public async Task OpenAsync()
        {
            await GoToAsync("/");
            await Expect(Page).ToHaveURLAsync(_baseUrl +"/");
            await AcceptCookiesIfAnyAsync();
        }

        public async Task AcceptCookiesIfAnyAsync()
        {
            try
            {
                if (await cookieAccept.IsVisibleAsync(new()))
                    await cookieAccept.ClickAsync();
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
            var btn= searchButton;
            if (!await btn.First.IsVisibleAsync(new()))
                btn = searchButtonByText;

            if (await btn.First.IsVisibleAsync())
            {
                await Task.WhenAll(
                    Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                    btn.First.ClickAsync()
                );
            }
            
        }
    }
}
