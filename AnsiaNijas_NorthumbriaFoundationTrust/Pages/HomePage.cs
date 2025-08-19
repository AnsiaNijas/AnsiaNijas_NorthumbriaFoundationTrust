using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using AnsiaNijas_NorthumbriaFoundationTrust.Utilities;
using System.Xml.Linq;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Northumbria homepage: open, accept cookies, perform a search.</summary>
    public class HomePage : GeneralUtils
    {
        private readonly string _baseUrl;      

        public HomePage(IPage page, string baseUrl)       
            : base(page)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public ILocator nhsIcon => Page.GetByRole(AriaRole.Link, new() { Name = "Northumbria Healthcare NHS" });
        public ILocator heading => Page.GetByRole(AriaRole.Heading, new() { Name = "We provide a range of health" });
        public ILocator searchBox => Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your search" });
        public ILocator searchButton =>  Page.GetByRole(AriaRole.Button, new() { Name = "Search", Exact = false });
        public ILocator searchResults => Page.GetByRole(AriaRole.Link, new() { Name = "1000 Pages found that matched" });
        public ILocator pageResults => Page.GetByRole(AriaRole.Heading, new() { Name = "Page results" });
        public ILocator qASLink => Page.GetByRole(AriaRole.Link, new() { Name = "Quality and safety" });

public async Task OpenAsync()
        {
            await GoToAsync("/");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page).ToHaveURLAsync(_baseUrl + "/");
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Northumbria Healthcare NHS" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "We provide a range of health" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your search" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Search" })).ToBeVisibleAsync();

        }

        /// <summary>Type a term into the search input (does not submit).</summary>
        public async Task EnterSearchTermAsync(string term)
        {
            await Assertions.Expect(searchBox).ToBeVisibleAsync();     // verify visible
            await Assertions.Expect(searchBox).ToBeEditableAsync();    // verify editable

            // verify placeholder text exists/looks right
            await Assertions.Expect(searchBox).ToHaveAttributeAsync(
                "placeholder", new Regex("What can we help you to find today?", RegexOptions.IgnoreCase));            
            await searchBox.ClearAsync(); // clear then type
            await Assertions.Expect(searchBox).ToBeEmptyAsync();   // verify cleared
            await searchBox.FillAsync(term);
            await Assertions.Expect(searchBox).ToHaveValueAsync(term); // verify value set
        }

        /// <summary>Submit search via clicking a "Search" button /Enter key.</summary>
        public async Task SubmitSearchByTriggerAsync(string trigger)
        {
            if (trigger == "search")
            {
                await Assertions.Expect(searchButton).ToBeVisibleAsync();
                await Assertions.Expect(searchButton).ToBeEnabledAsync();
                await Task.WhenAll(
                   Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded),
                   searchButton.ClickAsync()
               );
            }
            else 
            {
                await Page.Keyboard.PressAsync("Enter");
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            await Assertions.Expect(searchResults).ToBeVisibleAsync();
            await Assertions.Expect(pageResults).ToBeVisibleAsync();
            await Assertions.Expect(qASLink).ToBeVisibleAsync();
        }
    }
}
