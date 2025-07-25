using OpenQA.Selenium;
using LabCorp.UITest.Drivers;

namespace LabCorp.UITest.Pages
{
    public class JobResultsPage
    {
        private readonly IWebDriver _driver;
        public JobResultsPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void SelectJob(string jobTitle)
        {
            IWebElement jobLink = null;
            // Try to find by attribute first
            try
            {
                jobLink = WaitHelpers.WaitForElementByXPath(_driver, $"//a[@data-ph-at-job-title-text='{jobTitle}']", 10);
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: find by span text inside job-title div
                var links = _driver.FindElements(By.XPath($"//a[.//div[@class='job-title']/span[contains(text(),'{jobTitle}')]]"));
                foreach (var link in links)
                {
                    if (link.Displayed && link.Enabled)
                    {
                        jobLink = link;
                        break;
                    }
                }
                if (jobLink == null)
                {
                    throw new NoSuchElementException($"Job link for '{jobTitle}' not found.");
                }
            }
            jobLink.Click();
        }
    }
}
