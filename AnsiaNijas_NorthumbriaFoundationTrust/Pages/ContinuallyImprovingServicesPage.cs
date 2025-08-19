using System.Threading.Tasks;
using AnsiaNijas_NorthumbriaFoundationTrust.Utilities;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Destination page: assert the section content is present.</summary>
    public class ContinuallyImprovingServicesPage : GeneralUtils
    {
        public ContinuallyImprovingServicesPage(IPage page) : base(page) { }
        public ILocator contImprServHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Continually improving services" });
        public ILocator contImprServMain => Page.Locator(".main-breadcrumbs > .container > .row > .col-xs-24 > .core-style");
        public ILocator contImprServContent => Page.GetByText("We work hard to provide the");

        public async Task AssertContentVisibleAsync(string info)
        {
            await Expect(contImprServHeading).ToBeVisibleAsync();
            await Expect(contImprServMain).ToBeVisibleAsync();
            await Expect(contImprServContent).ToBeVisibleAsync();
        }
    }
}
