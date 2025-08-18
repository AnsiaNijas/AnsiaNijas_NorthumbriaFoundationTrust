using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Pages
{
    /// <summary>Destination page: assert the section content is present.</summary>
    public class ContinuallyImprovingServicesPage : BasePage
    {
        public ContinuallyImprovingServicesPage(IPage page) : base(page) { }
        public ILocator main => Page.Locator("main, #content, .content");
        public ILocator header=> main.GetByRole(AriaRole.Heading, new() { Level = 1 });
        public ILocator headerPara => main.Locator("p, li").First;
     
        public async Task AssertContentVisibleAsync(string info)
        {
            await ExpectTitleContainsAsync(info);                    
            await Expect(main).ToBeVisibleAsync();            
            if (await header.CountAsync() > 0)
                await Expect(header.First).ToBeVisibleAsync();

            await Expect(headerPara).ToBeVisibleAsync();
        }
    }
}
