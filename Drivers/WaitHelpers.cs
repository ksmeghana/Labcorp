using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace LabCorp.UITest.Drivers
{
    public static class WaitHelpers
    {
        public static IWebElement WaitForElementById(IWebDriver driver, string id, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => d.FindElement(By.Id(id)));
        }

        public static IWebElement WaitForElementByXPath(IWebDriver driver, string xpath, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => {
                try
                {
                    var element = d.FindElement(By.XPath(xpath));
                    return element.Displayed ? element : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }

        public static IWebElement WaitForElementByCssSelector(IWebDriver driver, string cssSelector, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => {
                try
                {
                    var element = d.FindElement(By.CssSelector(cssSelector));
                    return element.Displayed ? element : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }

        public static IWebElement WaitForElementToBeClickableByXPath(IWebDriver driver, string xpath, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => {
                try
                {
                    var el = d.FindElement(By.XPath(xpath));
                    return (el.Displayed && el.Enabled) ? el : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }

        public static bool WaitForElementToBeVisible(IWebDriver driver, By by, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(d => d.FindElement(by).Displayed);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public static void WaitForPageLoad(IWebDriver driver, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
