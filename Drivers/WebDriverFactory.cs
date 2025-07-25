using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LabCorp.UITest.Drivers
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateWebDriver()
        {
            // You can extend this to support multiple browsers
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            return new ChromeDriver(options);
        }
    }
}
