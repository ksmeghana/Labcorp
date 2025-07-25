# LabCorp UI Test Automation Framework

## Overview
This project is a UI test automation framework for LabCorp's career website. It uses Selenium WebDriver with C# and follows the Page Object Model pattern. The framework is built using Reqnroll (a BDD framework) and NUnit for test execution.

## Technology Stack
- **Language:** C# (.NET Framework 4.7.2)
- **Test Framework:** NUnit 3.13.3
- **BDD Framework:** Reqnroll 2.4.1
- **UI Automation:** Selenium WebDriver 4.34.0
- **Browser Driver:** ChromeDriver 138.0.7204.16800

## Project Structure
```
LabCorp.UITest/
??? Features/                 # Reqnroll feature files
?   ??? SoftwareEngineeringJobs.feature
?   ??? DataScienceJobs.feature
??? Pages/                   # Page Object Model classes
?   ??? HomePage.cs
?   ??? CareersPage.cs
?   ??? JobResultsPage.cs
?   ??? JobDetailsPage.cs
?   ??? ApplicationPage.cs
??? Steps/                   # Step definition files
?   ??? JobSearchSteps.cs
??? Drivers/                 # WebDriver and wait helper classes
?   ??? WebDriverFactory.cs
?   ??? WaitHelpers.cs
??? Hooks/                   # Test hooks for setup and teardown
?   ??? TestHooks.cs
??? Logging/                 # Logging functionality
    ??? LoggingService.cs
```

## Features
- **Page Object Model:** Clear separation of test logic and page interactions
- **Robust Wait Mechanisms:** Smart waits and retries for better test stability
- **Flexible Selectors:** Multiple selector strategies for finding elements
- **Error Handling:** Comprehensive error handling and retry mechanisms
- **Logging:** Detailed logging for test execution and failures
- **Data-Driven:** Table-driven test scenarios for better test maintenance

## Key Components

### Page Objects
- **HomePage:** Handles navigation to careers section
- **CareersPage:** Manages job search functionality
- **JobResultsPage:** Handles job listing and selection
- **JobDetailsPage:** Manages job details verification
- **ApplicationPage:** Handles job application process

### Helpers
- **WebDriverFactory:** Creates and configures WebDriver instances
- **WaitHelpers:** Provides various wait mechanisms for better stability

### Tests
The framework includes tests for:
- Job search functionality
- Job details verification
- Application process verification
- Navigation between different pages

## Getting Started

### Prerequisites
- Visual Studio 2022
- .NET Framework 4.7.2
- Chrome Browser

### Installation
1. Clone the repository
2. Restore NuGet packages
3. Build the solution
4. Run the tests using Test Explorer

### Running Tests
Tests can be run through:
- Visual Studio Test Explorer
- Command line: `dotnet test`
- CI/CD pipeline integration

## Writing Tests
Tests are written in Gherkin syntax:

```gherkin
Feature: Software Engineering Jobs
As a job seeker
I want to search for software engineering jobs
So that I can find relevant positions at LabCorp

Scenario: Search for Senior Full Stack Engineer position
    Given I open the browser and navigate to "https://www.labcorp.com/"
    When I click the Careers link
    And I search for the position "Senior Full Stack Engineer"
    Then I should see the job details with:
        | Job Title    | Senior Full Stack Engineer |
        | Job Location | Burlington, NC            |
```

## Best Practices
1. Use Page Object Model for maintainability
2. Implement robust wait mechanisms
3. Handle dynamic content appropriately
4. Use data-driven approach where possible
5. Implement proper logging
6. Follow BDD best practices

## Common Issues and Solutions
1. **Element Not Found:**
   - Use WaitHelpers class with appropriate timeouts
   - Implement retry mechanisms
   - Use multiple selector strategies

2. **Test Flakiness:**
   - Implement smart waits
   - Use proper page load verifications
   - Handle dynamic content properly

## Contributing
1. Follow C# coding standards
2. Write clean, maintainable code
3. Include proper documentation
4. Add appropriate error handling
5. Write comprehensive tests

## License
Copyright © 2025 HP