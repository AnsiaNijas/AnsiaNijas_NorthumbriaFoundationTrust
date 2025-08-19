using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace AnsiaNijas_NorthumbriaFoundationTrust.Utilities
{
    /// <summary>Base class with common helpers for all pages.</summary>
    public abstract class GeneralUtils
    {
        protected readonly IPage Page;
        protected GeneralUtils(IPage page) => Page = page;

        public virtual Task GoToAsync(string relative = "/") => Page.GotoAsync(relative);

        protected static Regex Rx(string text, bool ignoreCase = true) =>
            new Regex(text, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

        protected Task ExpectTitleContainsAsync(string text) =>
            Expect(Page).ToHaveTitleAsync(Rx(Regex.Escape(text)));
    }
}
