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

        public async Task AssertLoadedAsync()
        {
            await Expect(Page).ToHaveURLAsync(new Regex(@"quality-and-safety", RegexOptions.IgnoreCase));
            await ExpectTitleContainsAsync("Quality and safety");
        }

        /// <summary>Navigates to "Continually improving services" via link/button/card.</summary>
        public async Task GoToContinuallyImprovingAsync()
        {
            var target = Page.GetByRole(AriaRole.Link, new() { Name = "Continually improving services", Exact=false});
            if (!await target.First.IsVisibleAsync(new()))
            {
                target = Page.GetByRole(AriaRole.Button, new() { Name = "Continually improving services", Exact=false });
                if (!await target.First.IsVisibleAsync(new() ))
                    target = Page.Locator("section, article, div, li").Filter(new() { HasText = "Continually improving services" }).First;
            }

            await Task.WhenAll(
                Page.WaitForLoadStateAsync(LoadState.NetworkIdle),
                target.ClickAsync()
            );
        }
    }
}
