using OpenQA.Selenium;
using LabCorp.UITest.Drivers;

namespace LabCorp.UITest.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        public HomePage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void GoToHomePage()
        {
            _driver.Navigate().GoToUrl("https://www.labcorp.com");
        }

        public void ClickCareersLink()
        {
            // Try to close cookie consent or overlay if present
            try
            {
                var cookieButton = _driver.FindElement(By.XPath("//button[contains(text(),'Accept')]"));
                if (cookieButton.Displayed && cookieButton.Enabled)
                {
                    cookieButton.Click();
                }
            }
            catch (NoSuchElementException)
            {
                // No cookie consent found, continue
            }

            System.Threading.Thread.Sleep(1000); // short wait for overlays

            // Try a more specific XPath for the Careers link
            string careersXPath = "//a[@href='https://careers.labcorp.com/global/en' and contains(text(),'Careers')]";
            IWebElement careersLink = null;
            try
            {
                careersLink = LabCorp.UITest.Drivers.WaitHelpers.WaitForElementToBeClickableByXPath(_driver, careersXPath, 10);
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: print all matching elements and try by href only
                var allLinks = _driver.FindElements(By.XPath("//a[contains(text(),'Careers')]"));
                System.Diagnostics.Debug.WriteLine($"Found {allLinks.Count} elements with //a[contains(text(),'Careers')]");
                foreach (var el in allLinks)
                {
                    System.Diagnostics.Debug.WriteLine($"Displayed={el.Displayed}, Enabled={el.Enabled}, Href={el.GetAttribute("href")}, OuterHTML={el.GetAttribute("outerHTML")}");
                }
                // Try to find by href only
                var hrefLinks = _driver.FindElements(By.XPath("//a[@href='https://careers.labcorp.com/global/en']"));
                foreach (var el in hrefLinks)
                {
                    if (el.Displayed && el.Enabled)
                    {
                        careersLink = el;
                        break;
                    }
                }
                if (careersLink == null)
                {
                    throw new ElementNotInteractableException("Careers link is not interactable or not found by href.");
                }
            }

            // Log element details for debugging
            System.Diagnostics.Debug.WriteLine($"Careers link: Displayed={careersLink.Displayed}, Enabled={careersLink.Enabled}, Tag={careersLink.TagName}, Location={careersLink.Location}, Size={careersLink.Size}");
            System.Diagnostics.Debug.WriteLine($"OuterHTML: {careersLink.GetAttribute("outerHTML")}");

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", careersLink);
            try
            {
                careersLink.Click();
            }
            catch (ElementNotInteractableException)
            {
                // Fallback: try JavaScript click
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", careersLink);
            }

            // Wait for navigation and check URL
            const string expectedUrl = "https://careers.labcorp.com/global/en";
            bool urlMatched = false;
            for (int i = 0; i < 15; i++) // wait up to 15 seconds
            {
                System.Threading.Thread.Sleep(1000);
                if (_driver.Url.StartsWith(expectedUrl))
                {
                    urlMatched = true;
                    break;
                }
            }
            if (!urlMatched)
            {
                throw new WebDriverException($"Navigation to Careers page failed. Current URL: {_driver.Url}");
            }
        }
    }
}
