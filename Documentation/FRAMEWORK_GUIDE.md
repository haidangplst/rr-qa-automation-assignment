# QA Automation Framework - Page Object Model (POM) with Playwright and Chrome

This framework implements the Page Object Model pattern for automated testing using Microsoft Playwright with NUnit in C#.

## Project Structure

```
PlaywrightTests/
├── Fixtures/
│   └── BrowserFixture.cs          # Browser setup and teardown
├── PageObjects/
│   ├── BasePage.cs                 # Base class for all page objects
│   └── ExamplePage.cs              # Example page object implementation
├── Tests/
│   ├── ExampleTest.cs              # Legacy test file
│   └── ExamplePageTests.cs         # Tests using POM pattern
├── Utilities/
│   ├── BrowserUtils.cs             # Browser-related utilities
│   └── ElementUtils.cs             # Element interaction utilities
└── PlaywrightTests.csproj
```

## Key Features

### 1. **Browser Fixture** (BrowserFixture.cs)
- Manages browser lifecycle with Chrome
- Handles context and page creation/cleanup
- Provides one-time setup and teardown for browser instances
- Per-test context and page isolation

### 2. **Base Page Object** (BasePage.cs)
- Abstract base class for all page objects
- Common methods for element interaction:
  - Navigation (NavigateToAsync)
  - Clicking (ClickAsync)
  - Text input (FillAsync)
  - Text retrieval (GetTextAsync)
  - Element visibility checks (IsElementVisibleAsync)
  - And more...

### 3. **Example Page Object** (ExamplePage.cs)
- Concrete implementation of BasePage
- Demonstrates selector definition as constants
- Implements page-specific actions and assertions

### 4. **Utilities**
- **BrowserUtils.cs**: Chrome configuration, screenshots, cookies
- **ElementUtils.cs**: Advanced element interactions (scroll, hover, double-click, etc.)

### 5. **Tests** (ExamplePageTests.cs)
- Uses POM pattern with proper Arrange-Act-Assert structure
- Clear, maintainable test code
- Easy to extend for additional page objects

## Running Tests

### Run all tests:
```bash
dotnet test
```

### Run specific test class:
```bash
dotnet test --filter "ClassName=ExamplePageTests"
```

### Run with verbose output:
```bash
dotnet test --verbosity detailed
```

### Run tests in headed mode (see browser):
Edit BrowserFixture.cs and set `Headless = false` in LaunchAsync options.

## Creating New Page Objects

1. Create a new class inheriting from BasePage
2. Define selectors as private constants
3. Implement page-specific methods
4. Use in test classes

### Example:
```csharp
public class LoginPage : BasePage
{
    private const string UsernameFieldSelector = "input[name='username']";
    private const string PasswordFieldSelector = "input[name='password']";
    private const string LoginButtonSelector = "button[type='submit']";

    public LoginPage(IPage page) : base(page) { }

    public async Task LoginAsync(string username, string password)
    {
        await FillAsync(UsernameFieldSelector, username);
        await FillAsync(PasswordFieldSelector, password);
        await ClickAsync(LoginButtonSelector);
    }
}
```

## Best Practices

1. **Keep selectors as constants** at the class level
2. **Use meaningful method names** that describe user actions
3. **Encapsulate element interactions** within page objects
4. **Avoid assertions in page objects** - assertions belong in tests
5. **Use BasePage methods** to reduce code duplication
6. **Leverage utilities** for common operations

## Browser Configuration

Chrome is configured with:
- Headless mode (default) - set to false for debugging
- Disabled automation features
- Memory optimization flags
- HTTPS errors ignored for testing

## Dependencies

- Microsoft.Playwright (v1.59.0)
- Microsoft.Playwright.NUnit
- NUnit (v4.3.2)
- .NET 10.0

## CI/CD Integration

The framework is ready for CI/CD pipelines. Tests run in headless mode by default, making them suitable for automated testing environments.
