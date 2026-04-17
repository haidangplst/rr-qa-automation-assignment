# QA Automation Assignment - TMDB Discover Platform

A complete test automation framework using **Playwright**, **C#/.NET 10**, **NUnit**, and the **Page Object Model** design pattern for comprehensive testing of the TMDB Discover platform.

> Automated testing of the TMDB Discover movie/TV show discovery platform with full coverage of filtering, pagination, search, and edge cases.

## 🚀 Quick Start

**Windows:**
```bash
setup.bat
```

**Mac/Linux:**
```bash
bash setup-tests.sh
```

## 📋 What's Included

✅ **Page Object Model Architecture** - Reusable, maintainable page objects  
✅ **Chrome Browser Support** - Configured for headless and headed testing  
✅ **NUnit Test Framework** - Parallel test execution support  
✅ **Playwright Automation** - Cross-browser capable, headless ready  
✅ **Utility Classes** - Browser and element helpers  
✅ **Configuration Management** - Easy test configuration  
✅ **Complete Documentation** - Setup guides and examples  

## 📁 Project Structure

```
PlaywrightTests/
├── Configuration/
│   └── TestConfig.cs              # Test configuration settings
├── Fixtures/
│   └── BrowserFixture.cs          # Browser lifecycle management
├── PageObjects/
│   ├── BasePage.cs                 # Base page object class
│   └── ExamplePage.cs              # Example page object
├── Tests/
│   ├── ExampleTest.cs              # Legacy test file
│   └── ExamplePageTests.cs         # POM-based tests
├── Utilities/
│   ├── BrowserUtils.cs             # Browser utilities
│   └── ElementUtils.cs             # Element utilities
└── PlaywrightTests.csproj
```

## 🎯 Core Components

### 1. **BrowserFixture** (Fixtures/BrowserFixture.cs)
Manages the complete browser lifecycle:
- Chrome launch configuration
- Context and page creation
- Setup/teardown for each test
- Resource cleanup

### 2. **BasePage** (PageObjects/BasePage.cs)
Foundation for all page objects with methods for:
- Navigation
- Element interaction (click, fill, hover)
- Text retrieval
- Element visibility checks
- Utility operations

### 3. **Page Objects**
Concrete classes (e.g., ExamplePage.cs) that:
- Encapsulate selectors as constants
- Implement page-specific actions
- Use BasePage methods for interactions
- Represent user workflows

### 4. **Tests**
Test classes that:
- Use page objects for interactions
- Follow Arrange-Act-Assert pattern
- Are independent and readable
- Support parallel execution

## 🧪 Running Tests

### All Tests
```bash
dotnet test
```

### Specific Test Class
```bash
dotnet test --filter "ClassName=ExamplePageTests"
```

### Specific Test Method
```bash
dotnet test --filter "Name=ShouldNavigateToExampleCom"
```

### Verbose Output
```bash
dotnet test --verbosity detailed
```

### With Browser Visible (headed mode)
Edit `Fixtures/BrowserFixture.cs` and set `Headless = false`

## 📝 Creating a New Page Object

1. Create a new class inheriting from `BasePage`
2. Define selectors as private constants
3. Implement page-specific methods
4. Use in test classes

### Example: LoginPage
```csharp
using Microsoft.Playwright;
using PlaywrightTests.PageObjects;

public class LoginPage : BasePage
{
    private const string UsernameSelector = "input[id='username']";
    private const string PasswordSelector = "input[id='password']";
    private const string LoginButtonSelector = "button[type='submit']";
    private const string ErrorMessageSelector = ".error-message";

    public LoginPage(IPage page) : base(page) { }

    public async Task LoginAsync(string username, string password)
    {
        await FillAsync(UsernameSelector, username);
        await FillAsync(PasswordSelector, password);
        await ClickAsync(LoginButtonSelector);
    }

    public async Task<string?> GetErrorMessageAsync()
    {
        return await GetTextAsync(ErrorMessageSelector);
    }
}
```

### Example: Test Using LoginPage
```csharp
[TestFixture]
public class LoginTests
{
    private LoginPage? _loginPage;

    [SetUp]
    public void SetUp()
    {
        _loginPage = new LoginPage(BrowserFixture.Page!);
    }

    [Test]
    public async Task ShouldLoginSuccessfully()
    {
        await _loginPage!.NavigateToAsync("https://example.com/login");
        await _loginPage.LoginAsync("user@example.com", "password123");
        
        Assert.That(BrowserFixture.Page!.Url, Does.Contain("dashboard"));
    }
}
```

## ⚙️ Configuration

### Environment Variables
```bash
HEADLESS=true              # Run in headless mode
TIMEOUT=5000               # Default element timeout (ms)
NAV_TIMEOUT=30000          # Navigation timeout (ms)
RECORD_VIDEO=false         # Record test videos
BASE_URL=https://app.com   # Application base URL
```

### Programmatic Configuration
Edit `Configuration/TestConfig.cs`:
```csharp
TestConfig.Headless = true;
TestConfig.DefaultTimeout = 5000;
TestConfig.ViewportWidth = 1920;
TestConfig.ViewportHeight = 1080;
```

## 🔧 Browser Configuration

Chrome is configured with:
- **Headless mode** - Set to false for debugging
- **Disabled automation detection** - Looks like a real user
- **Memory optimization** - For CI/CD environments
- **HTTPS errors ignored** - For testing environments

Configure in: `Fixtures/BrowserFixture.cs`

## 📚 Best Practices

✅ **Selectors:** Keep as constants at class level  
✅ **Method names:** Describe user actions (LoginAsync, SubmitFormAsync)  
✅ **No assertions in page objects:** Only in tests  
✅ **Use BasePage methods:** Reduces duplication  
✅ **Meaningful waits:** Use IsElementVisibleAsync before interactions  
✅ **Parallel tests:** Leverage NUnit's [Parallelizable] attribute  

## 🐛 Troubleshooting

**Playwright browsers not installed:**
```bash
playwright install chromium
```

**Tests timing out:**
Increase timeout in `BrowserFixture.cs` or use `WaitForElementAsync()`

**Port conflicts:**
Change port in launch options or use context options

**Element not found:**
- Check selector accuracy
- Add wait before interaction
- Use `IsElementVisibleAsync()` to verify

## 📖 Documentation Files

- **FRAMEWORK_GUIDE.md** - Detailed framework documentation
- **SETUP_GUIDE.html** - Step-by-step setup instructions
- **README.md** - This file

## 🎨 Utilities

### BrowserUtils
- Chrome launch configuration
- Screenshot capture
- Cookie management
- Context options

### ElementUtils
- Scroll operations
- Attribute retrieval
- Class verification
- Multi-element operations
- Advanced interactions (hover, double-click)

## 🔄 CI/CD Ready

The framework is optimized for continuous integration:
- ✅ Headless mode by default
- ✅ Parallel test execution
- ✅ Environment variable configuration
- ✅ Screenshot/video recording on failure
- ✅ Cross-platform support (Windows, Mac, Linux)

## 📦 Dependencies

```xml
<PackageReference Include="Microsoft.Playwright.NUnit" Version="1.59.0" />
<PackageReference Include="NUnit" Version="4.3.2" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.0" />
```

## 🤝 Contributing

When adding new features:
1. Extend BasePage for common interactions
2. Create page objects for new pages/features
3. Add tests using POM pattern
4. Update documentation

---

## 🎬 TMDB Discover Test Suite

This repository includes a comprehensive test automation suite for the **TMDB Discover Platform** (https://tmdb-discover.surge.sh/), a movie and TV show discovery application.

### Test Coverage

#### Filtering Tests (24 tests)
- ✅ **Category Filters**: Popular, Trending, Newest, Top Rated
- ✅ **Type Filters**: Movies, TV Shows
- ✅ **Title Search**: Exact and partial matches
- ✅ **Year Filters**: Single and range selections
- ✅ **Rating Filters**: High, medium, low ratings
- ✅ **Genre Filters**: Single and multiple genres
- ✅ **Combined Filters**: Multiple simultaneous filters

#### Pagination Tests (7 tests)
- ✅ **Navigation**: Next, Previous, Page Jump
- ✅ **Filter Persistence**: Filters maintained across pages
- ✅ **Edge Cases**: First page, last page behavior

#### Negative Tests (5 tests)
- ⚠️ **URL Slug Access**: Known issue - Direct URL slugs don't work
- ⚠️ **Last Page Pagination**: Known issue - High page numbers fail
- ✅ **Invalid Input Handling**
- ✅ **No Results Handling**

#### UI & Accessibility Tests (6 tests)
- ✅ **Page Load Completeness**
- ✅ **Responsive Design**: Desktop, Tablet, Mobile
- ✅ **Keyboard Navigation**
- ✅ **Color Contrast**

#### Performance Tests (3 tests)
- ✅ **Load Time Validation**
- ✅ **Filter Response Speed**
- ✅ **Large Dataset Handling**

**Total: 45+ Comprehensive Test Cases**

### Running TMDB Tests

```bash
# Run all TMDB tests
dotnet test PlaywrightTests/Tests/TMDB/TMDBFilteringTests.cs

# Run specific test category
dotnet test --filter "Category=CategoryFilters"
dotnet test --filter "Category=Pagination"
dotnet test --filter "Category=NegativeTests"

# Run smoke tests only (fast)
dotnet test --filter "Category=Smoke"

# Generate HTML report
dotnet test --logger "html;LogFileName=tmdb-report.html"
```

### TMDB Test Documentation

📄 **TMDB_TEST_SPECIFICATIONS.md**
- Detailed test scenarios (40+ test cases)
- Step-by-step test descriptions
- Data assertions and API contracts
- Success criteria and known issues

📄 **DEFECTS_AND_KNOWN_ISSUES.md**
- Critical issues found during testing
- Known limitations
- Severity and impact analysis
- Recommended fixes

📄 **CI_CD_INTEGRATION_APPROACH.md**
- CI/CD pipeline strategies
- GitHub Actions, Azure Pipelines, Jenkins examples
- Deployment gates and failure handling
- Performance optimization

### Page Objects

```
PageObjects/TMDB/
├── TMDBBasePage.cs     # Base class with common TMDB methods
└── TMDBHomePage.cs     # Main page with filters and results
```

**Example Usage:**
```csharp
var homePage = new TMDBHomePage(page);
await homePage.NavigateToHomeAsync();
await homePage.FilterByPopularAsync();
await homePage.FilterByMoviesTypeAsync();
var results = await homePage.GetAllResultTitlesAsync();
```

### Logging & Reporting

The test suite includes comprehensive logging:

```csharp
Logger.TestStart("TC-001: Filter by Popular");
Logger.Step("1", "Navigate to home");
Logger.ApiCall("GET", url, statusCode, duration);
Logger.Assert(condition, "Assertion description");
Logger.TestEnd("TC-001", passed);
```

**Output:**
- 📊 **Console Logs**: Colored, real-time test execution
- 📄 **File Logs**: Detailed logs in `Logs/` directory
- 🌐 **HTML Report**: Visual report with charts and statistics
- 📸 **Screenshots**: Captured on test failure
- 🔗 **API Calls**: Captured and validated

### API Testing

The suite validates backend API calls:

```csharp
var apiHelper = new APITestHelper(page);
await apiHelper.StartNetworkCapturingAsync();

// After test execution
var apiCalls = apiHelper.GetAllNetworkCalls();
var errorCalls = apiHelper.GetErrorResponses();
var report = apiHelper.GenerateAPIReport();
```

**Assertions:**
- ✓ Response status codes
- ✓ Response data structure
- ✓ API parameter validation
- ✓ Error handling

### Known Issues

#### Issue #1: URL Slug Navigation
**Severity:** High  
Direct URL access with slugs (e.g., `/popular`) doesn't work.

#### Issue #2: Last Page Pagination
**Severity:** Medium  
Pagination fails on pages beyond ~15.

See **DEFECTS_AND_KNOWN_ISSUES.md** for details.

### Test Results Example

```
================================================================================
TEST EXECUTION SUMMARY
================================================================================
Total Tests:  45
✓ Passed:     40
✗ Failed:     5
Pass Rate:    88.9%
================================================================================

Failed Tests:
  ✗ TC-028: Last Page Navigation
  ✗ TC-030: URL Slug Access
  ✗ TC-031: Multiple Slug URLs
  ✗ TC-032: Invalid Page Number
  ✗ TC-043: Load Time Validation
```

### Test Execution Strategy

**Smoke Tests** (5-10 min) - Fast validation of core features
```bash
dotnet test --filter "Category=Smoke"
```

**Regression Tests** (15-20 min) - Full feature coverage
```bash
dotnet test PlaywrightTests/Tests/TMDB/
```

**Nightly Tests** (30+ min) - Extended suite + performance
```bash
dotnet test --filter "Category!=Performance" --parallel
```

### CI/CD Integration

The suite is ready for CI/CD with:
- ✅ Parallel execution support
- ✅ HTML and console reporting
- ✅ Environment variable configuration
- ✅ Failure detection and logging
- ✅ Cross-platform compatibility

See **CI_CD_INTEGRATION_APPROACH.md** for implementation examples.

---

**Happy Testing! 🚀**