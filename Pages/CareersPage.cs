using OpenQA.Selenium;
using LabCorp.UITest.Drivers;

namespace LabCorp.UITest.Pages
{
    public class CareersPage
    {
        private readonly IWebDriver _driver;
        public CareersPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void SearchForPosition(string position)
        {
            // Try to find the search input box (fallback to first text input if not known)
            IWebElement searchBox = null;
            try
            {
                searchBox = WaitHelpers.WaitForElementById(_driver, "search-box", 10);
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: find first visible text input
                var inputs = _driver.FindElements(By.XPath("//input[@type='text']"));
                foreach (var input in inputs)
                {
                    if (input.Displayed && input.Enabled)
                    {
                        searchBox = input;
                        break;
                    }
                }
            }
            if (searchBox == null)
                throw new NoSuchElementException("Search input box not found.");

            searchBox.Clear();
            searchBox.SendKeys(position);

            // Use the correct search button selector by id
            var searchButton = WaitHelpers.WaitForElementById(_driver, "ph-search-backdrop", 10);
            searchButton.Click();
        }
    }
}
