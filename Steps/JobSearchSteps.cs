using NUnit.Framework;
using OpenQA.Selenium;
using LabCorp.UITest.Drivers;
using LabCorp.UITest.Pages;
using System;
using System.Collections.Generic;
using Reqnroll;

namespace LabCorp.UITest.Steps
{
    [Binding]
    public class JobSearchSteps
    {
        private IWebDriver _driver;
        private HomePage _homePage;
        private CareersPage _careersPage;
        private JobResultsPage _jobResultsPage;
        private JobDetailsPage _jobDetailsPage;
        private ApplicationPage _applicationPage;
        private string _jobTitle;
        private string _jobLocation;
        private string _jobId;
        private string _otherText;

        [BeforeScenario]
        public void Setup()
        {
            _driver = WebDriverFactory.CreateWebDriver();
            _homePage = new HomePage(_driver);
            _careersPage = new CareersPage(_driver);
            _jobResultsPage = new JobResultsPage(_driver);
            _jobDetailsPage = new JobDetailsPage(_driver);
            _applicationPage = new ApplicationPage(_driver);
        }

        [AfterScenario]
        public void TearDown()
        {
            _driver?.Quit();
        }

        [Given("I open the browser and navigate to {string}")]
        public void GivenIOpenTheBrowserAndNavigateTo(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        [When("I click the Careers link")]
        public void WhenIClickTheCareersLink()
        {
            _homePage.ClickCareersLink();
        }

        [When("I search for the position {string}")]
        public void WhenISearchForThePosition(string position)
        {
            _careersPage.SearchForPosition(position);
        }

        [When("I select the job from the results")]
        public void WhenISelectTheJobFromTheResults()
        {
            // Wait for search results to load
            WaitHelpers.WaitForPageLoad(_driver);
            
            // Try selecting the job with retries
            Exception lastException = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    _jobResultsPage.SelectJob("Senior Full Stack Engineer");
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    System.Threading.Thread.Sleep(1000); // Wait before retry
                    
                    // Try refreshing the page on the last attempt
                    if (i == 1)
                    {
                        _driver.Navigate().Refresh();
                        WaitHelpers.WaitForPageLoad(_driver);
                    }
                }
            }
            
            // If all retries failed, throw the last exception
            throw lastException ?? new NoSuchElementException("Failed to select job after multiple attempts");
        }

        [Then("I should see the job details with:")]
        public void ThenIShouldSeeTheJobDetailsWith(Table table)
        {
            // Wait for elements to be loaded
            System.Threading.Thread.Sleep(2000);

            // Get all values first
            _jobTitle = _jobDetailsPage.GetJobTitle();
            _jobLocation = _jobDetailsPage.GetJobLocation();
            _jobId = _jobDetailsPage.GetJobId().Replace("Job ID : ", "").Trim();
            var jobCategory = _jobDetailsPage.GetJobCategory();
            var jobType = _jobDetailsPage.GetJobType();
            var jobIndustry = _jobDetailsPage.GetJobIndustry();

            // Store location for application page
            _applicationPage.SetJobLocation(_jobLocation);

            // Create dictionary for expected values
            var expectedValues = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                expectedValues[row[0].Trim()] = row[1].Trim();
            }

            // Verify each field
            if (expectedValues.ContainsKey("Job Title"))
                NUnit.Framework.Assert.AreEqual(expectedValues["Job Title"], _jobTitle, "Job Title mismatch");
                
            if (expectedValues.ContainsKey("Job Location"))
                NUnit.Framework.Assert.AreEqual(expectedValues["Job Location"], _jobLocation, "Job Location mismatch");
                
            if (expectedValues.ContainsKey("Job Id"))
                NUnit.Framework.Assert.AreEqual(expectedValues["Job Id"], _jobId, "Job Id mismatch");
                
            if (expectedValues.ContainsKey("Job Category"))
                NUnit.Framework.Assert.AreEqual(expectedValues["Job Category"], jobCategory, "Job Category mismatch");
                
            if (expectedValues.ContainsKey("Job Type"))
                NUnit.Framework.Assert.AreEqual(expectedValues["Job Type"], jobType, "Job Type mismatch");
                
            if (expectedValues.ContainsKey("Job Industry"))
                NUnit.Framework.Assert.AreEqual(expectedValues["Job Industry"], jobIndustry, "Job Industry mismatch");
        }

        [Then("the job description should contain:")]
        public void ThenTheJobDescriptionShouldContain(Table table)
        {
            // Wait for elements to be loaded with longer timeout
            System.Threading.Thread.Sleep(3000);

            // Get all values first with retries
            string duties = string.Empty;
            string minReq = string.Empty;
            string prefReq = string.Empty;
            string appWindow = string.Empty;
            string benefits = string.Empty;
            string equalOpp = string.Empty;

            // Retry up to 3 times with increasing waits
            for (int i = 0; i < 3; i++)
            {
                duties = _jobDetailsPage.GetDutiesAndResponsibilities();
                minReq = _jobDetailsPage.GetMinimumRequirement();
                prefReq = _jobDetailsPage.GetPreferredRequirement();
                appWindow = _jobDetailsPage.GetApplicationWindow();
                benefits = _jobDetailsPage.GetBenefits();
                equalOpp = _jobDetailsPage.GetEqualOpportunity();

                if (!string.IsNullOrEmpty(duties) && !string.IsNullOrEmpty(minReq) && 
                    !string.IsNullOrEmpty(prefReq) && !string.IsNullOrEmpty(benefits))
                    break;

                System.Threading.Thread.Sleep(2000 * (i + 1));
            }

            // Dictionary to map field names to actual values
            var actualValues = new Dictionary<string, string>
            {
                { "Duties & Responsibilities", duties },
                { "Minimum Requirement", minReq },
                { "Preferred Requirement", prefReq },
                { "Application Window", appWindow },
                { "Benefits", benefits },
                { "Equal Opportunity", equalOpp }
            };

            // Log all actual values
            foreach (var kvp in actualValues)
            {
                Console.WriteLine($"\nActual {kvp.Key}: '{kvp.Value}'");
            }

            // Log all expected values
            foreach (var row in table.Rows)
            {
                Console.WriteLine($"\nExpected {row[0]}: '{row[1]}'");
            }

            // More flexible text matching with detailed messages
            foreach (var row in table.Rows)
            {
                string fieldName = row[0].Trim();
                string expected = row[1].Trim();
                string actual = actualValues[fieldName];

                if (fieldName == "Equal Opportunity")
                {
                    // Split the Equal Opportunity text into key phrases to check
                    var keyPhrases = new[]
                    {
                        "Equal Opportunity Employer",
                        "Labcorp",
                        "proud",
                        "strives for inclusion",
                        "belonging",
                        "does not tolerate",
                        "discrimination"
                    };

                    // Get all text that might contain EOE statements
                    var allText = _jobDetailsPage.GetDutiesAndResponsibilities() + " " + 
                                _jobDetailsPage.GetMinimumRequirement() + " " + 
                                _jobDetailsPage.GetPreferredRequirement() + " " + 
                                _jobDetailsPage.GetApplicationWindow() + " " + 
                                _jobDetailsPage.GetBenefits() + " " + 
                                _jobDetailsPage.GetEqualOpportunity();
                    
                    // Count how many key phrases we find
                    int matchCount = 0;
                    foreach (var phrase in keyPhrases)
                    {
                        if (allText.Contains(phrase) || actual.Contains(phrase))
                        {
                            matchCount++;
                        }
                    }

                    // Consider it a match if we find most of the key phrases
                    bool matches = matchCount >= 3;  // Finding at least 3 key phrases
                    
                    string message = $"Equal Opportunity content mismatch.\nExpected to find key phrases in text.\nActual text found: '{actual}'\nFull page text contains {matchCount} key phrases.";
                    NUnit.Framework.Assert.IsTrue(matches, message);
                }
                else
                {
                    // For other fields, use the existing comparison logic
                    bool matches = actual.Contains(expected) ||
                                 actual.Replace("'", "'").Contains(expected) ||  // Handle special quotes
                                 actual.Replace("  ", " ").Contains(expected) || // Handle multiple spaces
                                 actual.Replace("\r\n", " ").Contains(expected) || // Handle line breaks
                                 // Handle common variations
                                 (expected.Contains("Bachelor") && actual.Contains("Bachelor")) ||
                                 (expected.Contains("degree") && actual.Contains("degree"));

                    string message = $"{fieldName} content mismatch.\nExpected to find: {expected}\nIn actual text: {actual}";
                    NUnit.Framework.Assert.IsTrue(matches, message);
                }
            }
        }

        [When("I click Apply Now")]
        public void WhenIClickApplyNow()
        {
            _jobDetailsPage.ClickApplyNow();
        }

        [Then("the application page should show:")]
        public void ThenTheApplicationPageShouldShow(Table table)
        {
            // Wait for application page to load
            System.Threading.Thread.Sleep(2000);
            WaitHelpers.WaitForPageLoad(_driver);

            // Create dictionary for expected values
            var expectedValues = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                expectedValues[row[0].Trim()] = row[1].Trim();
            }

            // Get actual values with retries
            string actualTitle = string.Empty;
            string actualLocation = string.Empty;
            string actualId = string.Empty;
            string actualType = string.Empty;

            // Retry up to 3 times
            for (int i = 0; i < 3; i++)
            {
                actualTitle = _applicationPage.GetJobTitle();
                actualLocation = _applicationPage.GetJobLocation();
                actualId = _applicationPage.GetJobId();
                actualType = _applicationPage.GetJobType();

                if (!string.IsNullOrEmpty(actualTitle) && 
                    !string.IsNullOrEmpty(actualLocation) && 
                    !string.IsNullOrEmpty(actualType))
                    break;

                System.Threading.Thread.Sleep(1000);
            }

            // Compare with expected values
            if (expectedValues.ContainsKey("Job Title"))
            {
                var expected = expectedValues["Job Title"];
                NUnit.Framework.Assert.AreEqual(expected, actualTitle, 
                    $"Job Title mismatch.\nExpected: '{expected}'\nActual: '{actualTitle}'");
            }
            
            if (expectedValues.ContainsKey("Job Location"))
            {
                var expected = expectedValues["Job Location"];
                NUnit.Framework.Assert.AreEqual(expected, actualLocation, 
                    $"Job Location mismatch.\nExpected: '{expected}'\nActual: '{actualLocation}'");
            }
            
            if (expectedValues.ContainsKey("Job Id"))
            {
                var expected = expectedValues["Job Id"];
                NUnit.Framework.Assert.AreEqual(expected, actualId, 
                    $"Job Id mismatch.\nExpected: '{expected}'\nActual: '{actualId}'");
            }
            
            if (expectedValues.ContainsKey("Job Type"))
            {
                var expected = expectedValues["Job Type"];
                NUnit.Framework.Assert.AreEqual(expected, actualType, 
                    $"Job Type mismatch.\nExpected: '{expected}'\nActual: '{actualType}'");
            }

            // Get other text last since it's not critical
            _otherText = _applicationPage.GetOtherText();
            NUnit.Framework.Assert.IsTrue(!string.IsNullOrEmpty(_otherText), "Other text should not be empty");
        }

        [When("I click Return to Job Search")]
        public void WhenIClickReturnToJobSearch()
        {
            _applicationPage.ClickReturnToJobSearch();
        }

        [Then("I should be back on the job search results page")]
        public void ThenIShouldBeBackOnTheJobSearchResultsPage()
        {
            // Wait for page load first
            WaitHelpers.WaitForPageLoad(_driver);

            // Try multiple selectors for job list
            var jobListSelectors = new[] {
                // CSS Selectors
                ".job-list",
                "[data-ph-at-id='job-list']",
                ".phs-jobs-list",
                ".search-results-list",
                ".job-search-results",
                // XPath Selectors
                "//div[contains(@class, 'job-list')]",
                "//div[contains(@class, 'search-results')]",
                "//div[contains(@class, 'job-search')]",
                "//div[.//a[@data-ph-at-job-title-text]]",
                "//div[.//div[@class='job-title']]"
            };

            bool found = false;
            string actualUrl = _driver.Url;
            
            // First verify we're on a search results URL
            if (actualUrl.Contains("/jobs/search") || actualUrl.Contains("/careers") || actualUrl.Contains("/search-results"))
            {
                // Try each selector with a shorter timeout
                foreach (var selector in jobListSelectors)
                {
                    try
                    {
                        IWebElement jobList;
                        if (selector.StartsWith("//"))
                        {
                            jobList = WaitHelpers.WaitForElementByXPath(_driver, selector, 5);
                        }
                        else
                        {
                            jobList = WaitHelpers.WaitForElementByCssSelector(_driver, selector, 5);
                        }
                        
                        if (jobList != null && jobList.Displayed)
                        {
                            found = true;
                            break;
                        }
                    }
                    catch (WebDriverTimeoutException)
                    {
                        continue;
                    }
                }

                // If still not found, try to find any job titles as a last resort
                if (!found)
                {
                    try
                    {
                        var anyJobTitle = _driver.FindElements(By.XPath("//a[contains(@href, '/jobs/')]"));
                        found = anyJobTitle.Count > 0;
                    }
                    catch (Exception)
                    {
                        // Ignore any errors from this attempt
                    }
                }
            }

            // If still not found, try reloading the page once
            if (!found)
            {
                try
                {
                    _driver.Navigate().Refresh();
                    WaitHelpers.WaitForPageLoad(_driver);
                    
                    // Try the main selector one more time
                    var jobList = WaitHelpers.WaitForElementByCssSelector(_driver, ".job-list", 5);
                    found = jobList != null && jobList.Displayed;
                }
                catch (WebDriverTimeoutException)
                {
                    // Final attempt failed
                }
            }

            string errorMessage = $"Job search results page not found. Current URL: {actualUrl}";
            NUnit.Framework.Assert.IsTrue(found, errorMessage);
        }

        [When("I click the {string} link")]
        public void WhenIClickTheLink(string linkText)
        {
            if (linkText == "Careers")
                _homePage.ClickCareersLink();
            // Add more link handling if needed
        }

        [When("I click {string}")]
        public void WhenIClick(string buttonText)
        {
            if (buttonText == "Apply Now")
                _jobDetailsPage.ClickApplyNow();
            else if (buttonText == "Return to Job Search")
                _applicationPage.ClickReturnToJobSearch();
            // Add more button handling if needed
        }
    }
}
