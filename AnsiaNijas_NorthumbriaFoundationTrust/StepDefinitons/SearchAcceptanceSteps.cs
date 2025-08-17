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
            await new HomePage(Page).OpenAsync();
        }

        // --- When ---
        [When(@"I enter ""(.*)"" in the search box")]
        public async Task WhenIEnterInTheSearchBox(string term)
        {
            _ctx["term"] = term;
            var home = new HomePage(Page);
            await home.EnterSearchTermAsync(term);
        }

        [When(@"I perform the search by clicking the search button")]
        public async Task WhenIClickSearch()
        {
            // the site reliably submits on Enter; the POM also supports a button if present
            var home = new HomePage(Page);
            await home.SubmitSearchByButtonAsync();
        }

        [When(@"I perform the search by enter")]
        public async Task WhenIPressEnter()
        {
            var home = new HomePage(Page);
            await home.SubmitSearchByEnterAsync();
        }

        // --- Then ---
        [Then(@"I should see search results related to ""(.*)""")]
        public async Task ThenIShouldSeeResults(string expected)
        {
            var results = new SearchResultsPage(Page);
            await results.AssertHasResultsForAsync(expected);
        }

        [Then(@"results will be returned based on the entered search term")]
        public async Task ThenResultsReturnedForStoredTerm()
        {
            var term = _ctx.TryGetValue("term", out var o) ? o?.ToString() ?? "" : "";
            var results = new SearchResultsPage(Page);
            await results.AssertHasResultsForAsync(term);
        }

        [Then(@"I can click the ""(.*)"" link from the results")]
        public async Task ThenIClickResultLink(string title)
        {
            var results = new SearchResultsPage(Page);
            await results.ClickResultAsync(title);
        }

        [Then(@"I navigate to the ""(.*)"" page")]
        public async Task ThenINavigateToThePage(string sectionName)
        {
            var qas = new QualityAndSafetyPage(Page);
            await qas.AssertLoadedAsync();
            await qas.GoToContinuallyImprovingAsync(); // clicks the “Continually improving services” box/link
        }

        [Then(@"I should see relevant information about the section")]
        public async Task ThenISeeRelevantInfo()
        {
            await new ContinuallyImprovingServicesPage(Page).AssertContentVisibleAsync();
        }
    }
}
