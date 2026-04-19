# Testing Strategy & Framework Documentation

## Table of Contents
1. [Testing Strategy](#testing-strategy)
2. [Test Cases Generated](#test-cases-generated)
3. [Test Automation Framework](#test-automation-framework)
4. [How to Run Tests](#how-to-run-tests)
5. [Test Design Techniques](#test-design-techniques)
6. [Design Patterns Used](#design-patterns-used)

---

## 1. Testing Strategy

### Overall Approach
Our testing strategy follows a **risk-based, exploratory, and regression-focused** approach to ensure comprehensive coverage of the TMDB Discover application. The strategy emphasizes:

- **Early Detection**: Smoke tests run first to catch critical failures
- **Stability**: Regression tests ensure existing functionality remains intact
- **Coverage**: Functional tests validate all user workflows
- **Quality**: Defect tracking and reporting for continuous improvement

### Test Pyramid Strategy

```
		/\
	   /  \
	  / UI \         ← End-to-End Tests (Playwright)
	 /------\
	/        \       ← Integration Tests (API + UI)
   / API Tests\
  /------------\     ← Unit Tests (Future)
 /______________\
```

**Current Implementation Focus**: E2E UI Testing + API Testing (when available)

### Testing Levels

1. **Smoke Tests** (Critical Path)
   - Verify core functionality works
   - Run on every deployment
   - Fast execution (< 2 minutes)
   - Covers: Navigation, Search, Pagination, Filters

2. **Regression Tests** (Full Coverage)
   - Comprehensive feature validation
   - Run before releases
   - Covers: All user scenarios, edge cases, error handling

3. **Exploratory Tests** (Ad-hoc)
   - Manual testing for new features
   - Defect investigation
   - Usability testing

---

## 2. Test Cases Generated

### 2.1 Test Case Categories

#### **Category 1: UI Validation & Navigation (TC_001 - TC_005)**

| Test ID | Test Name | Priority | Reason for Selection |
|---------|-----------|----------|---------------------|
| TC_001 | Validate_TheUIDisplayedInDefaultPage | Critical | Ensures landing page loads correctly with all discovery options |
| TC_001_1 | Validate_ThePagination | High | Pagination is core navigation - must display correct page numbers |
| TC_001_2 | Validate_ClickOnThePagination | High | User must be able to navigate between pages reliably |
| TC_002 | Validate_FilterByPopularCategory | Critical | Popular filter is the default view - most used feature |
| TC_003 | Validate_FilterByTVShowsType | Medium | Type switching is key filtering capability |
| TC_004 | Validate_ToggleBetweenMoviesAndTVShows | Medium | Tests state management when switching filters |
| TC_005 | Validate_CannotDirectAccessUrlToPopularPage | High | Security/routing test - prevents broken deep links |

**Rationale**: These tests cover the **Golden Path** - the most common user workflows. Any failure here means the application is unusable for most users.

#### **Category 2: Search Functionality (TC_006 - TC_008)**

| Test ID | Test Name | Priority | Reason for Selection |
|---------|-----------|----------|---------------------|
| TC_006 | SearchByTitle_ExactMatch | Critical | Search is primary discovery method - must work accurately |
| TC_007 | Validate_TheItemsHasImageErrorDisplayed | Medium | Image errors degrade user experience - quality check |
| TC_008 | Validate_NoItemsDisplayed | High | Negative test - validates error handling for invalid searches |

**Rationale**: Search is the **primary user interaction**. We test:
- Happy path (exact match)
- Quality (image validation)
- Error handling (no results)

#### **Category 3: Filter Categories (TC_009 - TC_011)**

| Test ID | Test Name | Priority | Reason for Selection |
|---------|-----------|----------|---------------------|
| TC_009 | FilterByNewestCategory | Medium | Validates newest releases are displayed with dates |
| TC_010 | FilterByTopRatedCategory | Medium | Ensures rating-based filtering works |
| TC_011 | FilterByTrendingCategory | Medium | Trending filter validates dynamic content loading |

**Rationale**: Each category filter represents a **unique API endpoint/data source**. Testing all ensures backend integration works.

### 2.2 Why These Test Cases?

#### **Risk-Based Selection Criteria**

1. **High Business Impact**
   - Search and Browse functionality (TC_006, TC_002) → Core user needs
   - Pagination (TC_001_2) → Essential for large datasets

2. **High Failure Probability**
   - Direct URL access (TC_005) → Common routing bug
   - Image loading errors (TC_007) → Network/CDN issues
   - Filter toggling (TC_004) → State management bugs

3. **Regulatory/Quality Requirements**
   - No broken images (TC_007) → Professional appearance
   - Proper error messages (TC_008) → User experience standard

4. **Technical Complexity**
   - Async data loading (all tests) → Timing issues common
   - Dynamic pagination (TC_001_1) → Complex DOM manipulation

---

## 3. Test Automation Framework

### 3.1 Framework Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Test Layer                         │
│  PopularPagesTests | TrendingPagesTests | ...       │
└─────────────────────────────────────────────────────┘
						↓
┌─────────────────────────────────────────────────────┐
│              Page Object Layer                       │
│  TMDBHomePage | PopularPage | BasePage              │
└─────────────────────────────────────────────────────┘
						↓
┌─────────────────────────────────────────────────────┐
│              Utilities Layer                         │
│  Logger | TestExecutionLogger | HTMLReportGenerator  │
└─────────────────────────────────────────────────────┘
						↓
┌─────────────────────────────────────────────────────┐
│              Playwright Core                         │
│  Browser Automation | Network Interception          │
└─────────────────────────────────────────────────────┘
```

### 3.2 Technologies & Libraries Used

#### **Core Framework Stack**

| Library | Version | Purpose | Why Chosen |
|---------|---------|---------|------------|
| **Microsoft.Playwright** | Latest | Browser automation | Modern, fast, multi-browser support, auto-wait, network interception |
| **NUnit** | 4.x | Test runner | Industry standard, parallel execution, rich assertions |
| **NUnit3TestAdapter** | 5.x | VS integration | Seamless Visual Studio Test Explorer integration |
| **.NET 10** | Latest | Runtime | Latest C# features, performance improvements |

#### **Reporting & Logging**

| Component | Technology | Purpose |
|-----------|------------|---------|
| **Console Logger** | Custom `Logger.cs` | Real-time test execution feedback |
| **HTML Reports** | `EnhancedHTMLReportGenerator` | Executive summary with step-by-step details |
| **Test Execution Logger** | `TestExecutionLogger` | Structured data collection for reporting |
| **File Logging** | Custom file writer | Persistent logs for debugging |

#### **Utilities & Helpers**

| Utility | Purpose |
|---------|---------|
| `TestConfig` | Centralized configuration (timeouts, URLs) |
| `BrowserFactory` | Browser instance management |
| `ElementUtils` | Reusable element interaction helpers |
| `APITestHelper` | API testing support (future expansion) |

### 3.3 Framework Features

✅ **Page Object Model** - Maintainable, reusable page interactions
✅ **Parallel Execution** - Faster test runs with `ParallelScope.Self`
✅ **Retry Logic** - Automatic retry for flaky navigation
✅ **Wait Strategies** - Smart waits with `NetworkIdle`, element visibility checks
✅ **Rich Reporting** - HTML reports with screenshots, logs, and execution timeline
✅ **Step-by-Step Logging** - Detailed execution trace for debugging
✅ **Error Handling** - Graceful failure with meaningful error messages
✅ **Configuration Management** - Easy environment switching

---

## 4. How to Run Tests

### 4.1 Prerequisites

```bash
# 1. Install .NET 10 SDK
# Download from: https://dotnet.microsoft.com/download/dotnet/10.0

# 2. Install Playwright browsers
cd PlaywrightTests
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### 4.2 Running Tests via Command Line

#### **Run All Tests**
```bash
dotnet test
```

#### **Run Specific Test Category**
```bash
# Run Smoke Tests only
dotnet test --filter "Category=Smoke"

# Run Popular Page Tests
dotnet test --filter "Category=PopularPageTests"

# Run Category Pages Tests
dotnet test --filter "Category=CategoryPages"
```

#### **Run Specific Test Class**
```bash
dotnet test --filter "FullyQualifiedName~PlaywrightTests.Tests.TMDB.PopularPagesTests"
```

#### **Run Single Test**
```bash
dotnet test --filter "Name=TC_001_Validate_TheUIDisplayedInDefaulePage"
```

#### **Run with Detailed Output**
```bash
dotnet test --logger "console;verbosity=detailed"
```

### 4.3 Running Tests via Visual Studio

1. **Open Test Explorer**
   - View → Test Explorer (Ctrl+E, T)

2. **Run Tests**
   - **All Tests**: Click "Run All" ▶️
   - **By Category**: Group by Traits → Right-click category → Run
   - **Single Test**: Right-click test → Run

3. **Debug Tests**
   - Right-click test → Debug
   - Set breakpoints in test or page object code

### 4.4 Running Tests via PowerShell Scripts

```powershell
# Run smoke tests and generate report
cd PlaywrightTests
dotnet test --filter "Category=Smoke" --logger "console;verbosity=normal"

# View HTML report (auto-opens after test run completes)
# Reports are in: PlaywrightTests/Reports/report_*.html
```

### 4.5 Viewing Test Reports

#### **HTML Report**
```bash
# Reports are auto-generated after test run
# Location: PlaywrightTests/Reports/report_HH_dd_MM_yyyy.html

# Open latest report
cd PlaywrightTests/Reports
# Double-click the latest .html file
```

**Report Contents**:
- ✅ Execution summary (pass/fail counts, duration)
- 📊 Test details with step-by-step execution
- 🕒 Timestamps and duration for each step
- ❌ Failure details with stack traces
- 📝 Logs and assertions

#### **Console Output**
Real-time logs appear in console:
```
06:26:40.269 | INFO | Found 4 Discovery Option filter types
06:26:40.275 | INFO |   - Filter type 1: Type
06:26:40.326 | INFO | ✓ Current page is 1 as expected
✓ PASSED: TC-001
```

#### **Log Files**
```
PlaywrightTests/Logs/test_log_yyyyMMdd_HHmmss.log
```

### 4.6 Parallel Execution Configuration

Tests run in parallel at the **test class level** (`ParallelScope.Self`):

```csharp
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PopularPagesTests : PlaywrightPageTest
```

**Benefits**:
- ✅ Faster execution (multiple browsers in parallel)
- ✅ Each test has isolated browser context
- ✅ No shared state between tests

---

## 5. Test Design Techniques

### 5.1 Equivalence Partitioning

**Applied To**: Search functionality

| Input Class | Test Case | Representative Value |
|-------------|-----------|---------------------|
| Valid exact match | TC_006 | "Inception" → Returns results |
| Invalid search term | TC_008 | "prty" → No results |
| Partial match | (Future) | "Incep" → Returns Inception |

**Rationale**: Reduce test cases by grouping similar inputs into partitions.

### 5.2 Boundary Value Analysis

**Applied To**: Pagination

| Boundary | Test Case | Value |
|----------|-----------|-------|
| First page | TC_001_1 | Page 1 (default) |
| Low page numbers | TC_001_2 | Page 2-4 |
| High page numbers | (Avoided in TC_001_2) | 56000+ causes errors |

**Rationale**: Bugs often occur at boundaries (first/last page, min/max values).

### 5.3 State Transition Testing

**Applied To**: Filter toggling (TC_004)

```
[Movie] → Click TV Shows → [TV Shows] → Click Movie → [Movie]
   ↓                            ↓
Verify results              Verify results
```

**States**:
1. Initial state: Movie filter selected
2. Transition: Click TV Shows filter
3. New state: TV Shows filter selected
4. Transition: Click Movie filter
5. Final state: Movie filter selected

**Validation**: Each state must show correct results.

### 5.4 Error Guessing

**Applied To**: Navigation and error handling

| Potential Error | Test Case | Validation |
|----------------|-----------|------------|
| Direct URL access fails | TC_005 | Verify 404 page |
| Missing images | TC_007 | Check for broken image placeholders |
| No search results | TC_008 | Verify empty state message |
| Pagination errors | TC_001_2 | Check for "Something went wrong" message |

**Rationale**: Experienced testers anticipate common failure modes.

### 5.5 Decision Table Testing

**Applied To**: Discovery options validation

| Type | Genre | Year | Rating | Expected Result |
|------|-------|------|--------|----------------|
| Movie | Any | Any | Any | Show movie results |
| TV Shows | Any | Any | Any | Show TV show results |
| (Combined filters not yet tested) |

**Future Enhancement**: Test all filter combinations.

### 5.6 Negative Testing

**Test Cases**:
- TC_005: Invalid URL access → 404
- TC_008: Invalid search → No results
- TC_001_2: High page numbers → Error message

**Philosophy**: System should **fail gracefully** with helpful error messages.

---

## 6. Design Patterns Used

### 6.1 Page Object Model (POM)

**Implementation**:
```csharp
public class TMDBHomePage : TMDBBasePage
{
	private const string SearchInputSelector = "input[placeholder*='SEARCH']";

	public async Task SearchByTitleAsync(string title)
	{
		await FillAsync(SearchInputSelector, title);
		await Page.Keyboard.PressAsync("Enter");
		await WaitForLoadingToCompleteAsync();
	}
}
```

**Benefits**:
- ✅ **Reusability**: Same methods used across multiple tests
- ✅ **Maintainability**: Selector changes only in one place
- ✅ **Readability**: Test code reads like business logic

**Structure**:
```
BasePage (generic methods)
  ↓
TMDBBasePage (TMDB-specific shared logic)
  ↓
TMDBHomePage, PopularPage, TrendingPage (page-specific logic)
```

### 6.2 Factory Pattern

**Implementation**: `BrowserFactory`

```csharp
public static class BrowserFactory
{
	public static async Task<IBrowser> CreateBrowserAsync(BrowserType browserType)
	{
		return browserType switch
		{
			BrowserType.Chromium => await playwright.Chromium.LaunchAsync(),
			BrowserType.Firefox => await playwright.Firefox.LaunchAsync(),
			BrowserType.WebKit => await playwright.Webkit.LaunchAsync(),
			_ => throw new ArgumentException("Invalid browser type")
		};
	}
}
```

**Benefits**:
- ✅ Centralized browser creation logic
- ✅ Easy to add new browser types
- ✅ Consistent browser configuration

### 6.3 Singleton Pattern

**Implementation**: `Logger`, `TestExecutionLogger`

```csharp
public static class Logger
{
	private static readonly object _lockObject = new object();

	public static void Info(string message)
	{
		lock (_lockObject)
		{
			// Single logger instance shared across all tests
			Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | INFO | {message}");
		}
	}
}
```

**Benefits**:
- ✅ Single log file for entire test run
- ✅ Thread-safe logging
- ✅ Centralized log management

### 6.4 Builder Pattern

**Implementation**: Test Configuration

```csharp
public static class TestConfig
{
	public static string BaseUrl => "https://tmdb-discover.surge.sh/";
	public static int NavigationTimeout => 60000;
	public static int DefaultTimeout => 30000;
}
```

**Benefits**:
- ✅ Fluent configuration setup
- ✅ Environment-specific settings
- ✅ Type-safe configuration

### 6.5 Template Method Pattern

**Implementation**: `PlaywrightPageTest` base class

```csharp
public class PlaywrightPageTest : PageTest
{
	[SetUp]
	public async Task SetUp()
	{
		// Template: Every test navigates to base URL first
		await Page.GotoAsync(TestConfig.BaseUrl);
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		// Template: Generate report after all tests
		GenerateReport();
	}
}
```

**Benefits**:
- ✅ Consistent test setup/teardown
- ✅ Automatic report generation
- ✅ DRY principle (Don't Repeat Yourself)

### 6.6 Strategy Pattern

**Implementation**: Wait strategies

```csharp
// Different wait strategies based on context
await Page.GotoAsync(url, new PageGotoOptions 
{ 
	WaitUntil = WaitUntilState.NetworkIdle // Strategy: Wait for network
});

await Page.Locator(selector).WaitForAsync(); // Strategy: Wait for element
```

**Benefits**:
- ✅ Flexible waiting mechanisms
- ✅ Context-appropriate strategies
- ✅ Reduced flakiness

### 6.7 Data-Driven Testing Pattern

**Implementation**: Parameterized search terms

```csharp
// Can be extended to:
[TestCase("Inception", 20)]
[TestCase("Avatar", 15)]
public async Task SearchByTitle(string searchTerm, int expectedMinResults)
{
	await homePage.SearchByTitleAsync(searchTerm);
	var results = await homePage.GetResultsCountAsync();
	Assert.GreaterOrEqual(results, expectedMinResults);
}
```

**Benefits**:
- ✅ One test method, multiple data sets
- ✅ Easy to add new test scenarios
- ✅ Reduced code duplication

### 6.8 Fluent Interface Pattern

**Implementation**: Test execution logging

```csharp
TestExecutionLogger.StartTest(testName, "Smoke")
	.RecordStep(1, "Navigate to page")
	.LogStepInfo("Page loaded")
	.CompleteStep()
	.CompleteTest(true);
```

**Benefits**:
- ✅ Readable, chainable API
- ✅ Self-documenting code
- ✅ Reduced boilerplate

---

## 7. Additional Best Practices

### 7.1 Test Independence
- ✅ Each test starts with fresh browser context
- ✅ No shared state between tests
- ✅ Tests can run in any order

### 7.2 Explicit Waits
- ✅ `WaitForLoadingToCompleteAsync()` instead of `Thread.Sleep()`
- ✅ Element visibility checks before interaction
- ✅ Network idle waits for AJAX calls

### 7.3 Meaningful Assertions
```csharp
// ❌ Bad
Assert.True(items > 0);

// ✅ Good
if (items == 0)
{
	throw new Exception($"Expected items but found 0 results");
}
```

### 7.4 Logging Strategy
- **INFO**: Normal execution flow
- **WARNING**: Retries, recoverable errors
- **ERROR**: Test failures, exceptions
- **STEP**: Test step execution for traceability

### 7.5 Error Recovery
```csharp
// Retry logic for navigation failures
int maxRetries = 3;
while (retryCount < maxRetries)
{
	try
	{
		await Page.GotoAsync(url);
		return;
	}
	catch (Exception ex)
	{
		Logger.Warning($"Retry {retryCount + 1}/{maxRetries}");
		await Task.Delay(2000);
	}
}
```

---

## 8. Future Enhancements

### 8.1 Planned Test Coverage
- [ ] API testing for backend endpoints
- [ ] Performance testing (page load times)
- [ ] Accessibility testing (WCAG compliance)
- [ ] Cross-browser testing (Chrome, Firefox, Safari)
- [ ] Mobile responsive testing
- [ ] Security testing (XSS, SQL injection)

### 8.2 Framework Improvements
- [ ] Screenshot capture on failure
- [ ] Video recording of test execution
- [ ] Integration with CI/CD pipeline
- [ ] Test data management (test data builder)
- [ ] Visual regression testing
- [ ] Database state validation

### 8.3 Reporting Enhancements
- [ ] Trend analysis (pass/fail over time)
- [ ] Test execution dashboard
- [ ] Slack/Teams notifications
- [ ] Defect tracking integration (Jira)
- [ ] Code coverage reports

---

## 9. Conclusion

This testing framework provides a **robust, maintainable, and scalable** solution for automated testing of the TMDB Discover application. By leveraging modern tools (Playwright, NUnit), proven design patterns (POM, Factory, Singleton), and comprehensive test design techniques (Equivalence Partitioning, BVA, State Transition), we ensure:

✅ **High Quality**: Early defect detection through smoke tests
✅ **Fast Feedback**: Parallel execution and retry logic
✅ **Maintainability**: Clear separation of concerns, reusable components
✅ **Visibility**: Rich HTML reports with step-by-step execution details
✅ **Scalability**: Easy to add new tests, pages, and features

**Next Steps**:
1. Expand test coverage to include API tests
2. Integrate with CI/CD pipeline (GitHub Actions / Azure DevOps)
3. Add visual regression testing
4. Implement accessibility testing

---

**Document Version**: 1.0
**Last Updated**: January 2025
**Maintained By**: QA Automation Team
