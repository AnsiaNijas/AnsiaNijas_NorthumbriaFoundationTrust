using System.Threading.Tasks;
using Microsoft.Playwright;
using Reqnroll;
using AnsiaNijas_NorthumbriaFoundationTrust.Pages;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Tests.StepDefinitions
{
    [Binding] // Reqnroll discovers this class and its step methods
    public class SearchSteps_POM
    {
        private readonly ScenarioContext _ctx;
        private IPage Page => (IPage)_ctx["Page"]; // set in Hooks.BeforeScenario

        public SearchSteps_POM(ScenarioContext ctx) => _ctx = ctx;

        // --- Given ---
        [Given(@"I navigate to the Northumbria NHS homepage")]
        public async Task GivenINavigateToHome()
        {
            var baseUrl = (string)_ctx["BaseUrl"];
            var home = new HomePage(Page, baseUrl);
            await home.OpenAsync();
        }

        // --- When ---
        [When(@"I enter ""(.*)"" in the search box")]
        public async Task WhenIEnterInTheSearchBox(string term)
        {
            _ctx["term"] = term;
            var baseUrl = (string)_ctx["BaseUrl"];
            var home = new HomePage(Page, baseUrl);
            await home.EnterSearchTermAsync(term);
        }

        [When(@"I perform the search by clicking the search button")]
        public async Task WhenIClickSearch()
        {
            // the site reliably submits on Enter; the POM also supports a button if present
            var baseUrl = (string)_ctx["BaseUrl"];
            var home = new HomePage(Page, baseUrl);
            await home.SubmitSearchByButtonAsync();
        }

        [When(@"I perform the search by enter")]
        public async Task WhenIPressEnter()
        {
            var baseUrl = (string)_ctx["BaseUrl"];
            var home = new HomePage(Page, baseUrl);
            await home.SubmitSearchByEnterAsync();
        }

        // --- Then ---
        [Then(@"I should see search results related to ""(.*)""")]
        public async Task ThenIShouldSeeResults(string expected)
        {
            var results = new SearchResultsPage(Page);
            await results.AssertHasResultsForAsync(expected);
        }

        [Then(@"I can click the ""(.*)"" link from the results")]
        public async Task ThenIClickResultLink(string title)
        {
            var results = new SearchResultsPage(Page);
            await results.ClickResultAsync(title);
        }

        [Then(@"I navigate to the ""Continually improving services"" page")]
        public async Task ThenINavigateToThePage()
        {
            var qas = new QualityAndSafetyPage(Page);
            await qas.AssertLoadedAsync();
            await qas.GoToContinuallyImprovingAsync(); // clicks the “Continually improving services” box/link
        }

        [Then(@"I should see relevant information about ""(.*)""")]
        public async Task ThenISeeRelevantInfo(string info)
        {
            await new ContinuallyImprovingServicesPage(Page).AssertContentVisibleAsync(info);
        }
    }
}
