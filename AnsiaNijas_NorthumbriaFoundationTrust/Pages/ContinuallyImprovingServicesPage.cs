using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Destination page: assert the section content is present.</summary>
    public class ContinuallyImprovingServicesPage : BasePage
    {
        public ContinuallyImprovingServicesPage(IPage page) : base(page) { }

        public async Task AssertContentVisibleAsync()
        {
            await ExpectTitleContainsAsync("Continually improving services");

            var main = Page.Locator("main, #content, .content");
            await Expect(main).ToBeVisibleAsync();

            var h1 = main.GetByRole(AriaRole.Heading, new() { Level = 1 });
            if (await h1.CountAsync() > 0)
                await Expect(h1.First).ToBeVisibleAsync();

            await Expect(main.Locator("p, li").First).ToBeVisibleAsync();
        }
    }
}
