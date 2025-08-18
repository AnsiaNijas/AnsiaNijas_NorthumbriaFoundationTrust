using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Quality & Safety landing page, with navigation onward.</summary>
    public class QualityAndSafetyPage : BasePage
    {
        public QualityAndSafetyPage(IPage page) : base(page) { }

        public ILocator contImprServLink => Page.GetByRole(AriaRole.Link, new () { Name = "Continually improving services", Exact=false});
        public ILocator contImprServRole => Page.GetByRole(AriaRole.Button, new () { Name = "Continually improving services", Exact=false });
        public ILocator contImprServText => Page.Locator("section, article, div, li").Filter(new() { HasText = "Continually improving services" }).First;
            
        public async Task AssertLoadedAsync()
        {
            await Expect(Page).ToHaveURLAsync(new Regex(@"quality-and-safety", RegexOptions.IgnoreCase));
            await ExpectTitleContainsAsync("Quality and safety");
        }

        /// <summary>Navigates to "Continually improving services" via link/button/card.</summary>
        public async Task GoToContinuallyImprovingAsync()
        {
            var target = contImprServLink;
            if (!await target.First.IsVisibleAsync(new()))
            {
                target = contImprServRole;
                if (!await target.First.IsVisibleAsync(new() ))
                    target = contImprServLink;
            }

            await Task.WhenAll(
                Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                target.ClickAsync()
            );
        }
    }
}
