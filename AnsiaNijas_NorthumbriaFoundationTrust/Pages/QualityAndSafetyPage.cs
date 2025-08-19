using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnsiaNijas_NorthumbriaFoundationTrust.Utilities;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Quality & Safety landing page, with navigation onward.</summary>
    public class QualityAndSafetyPage : GeneralUtils
    {
        public QualityAndSafetyPage(IPage page) : base(page) { }

        public ILocator contImprServLink => Page.GetByRole(AriaRole.Link, new () { Name = "Continually improving services", Exact=false});
        public ILocator contImprServRole => Page.GetByRole(AriaRole.Button, new () { Name = "Continually improving services", Exact=false });
        public ILocator contImprServText => Page.Locator("section, article, div, li").Filter(new() { HasText = "Continually improving services" }).First;
        public ILocator contImprServHeading => Page.GetByRole(AriaRole.Heading, new() {Name = "Continually improving services" });
        public ILocator contImprServMain => Page.Locator(".main-breadcrumbs > .container > .row > .col-xs-24 > .core-style");
        public ILocator contImprServContent => Page.GetByText("We work hard to provide the");
        public async Task AssertLoadedAsync()
        {
            await Expect(Page).ToHaveURLAsync(new Regex(@"quality-and-safety", RegexOptions.IgnoreCase));
            await ExpectTitleContainsAsync("Quality and safety");
        }

        /// <summary>Navigates to "Continually improving services" via link/button/card.</summary>
        public async Task GoToContinuallyImprovingAsync()
        {
                await Expect(contImprServLink).ToBeVisibleAsync();
                await Task.WhenAll(
                Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                contImprServLink.ClickAsync()
            );
            await Expect(contImprServHeading).ToBeVisibleAsync();
            await Expect(contImprServMain).ToBeVisibleAsync();
            await Expect(contImprServContent).ToBeVisibleAsync();
        }
    }
}
