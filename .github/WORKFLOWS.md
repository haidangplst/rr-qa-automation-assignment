# GitHub Actions CI/CD Workflows

This project uses GitHub Actions to automate testing and building across multiple browsers and test categories.

## Available Workflows

### 1. 🧪 Test with Category Filter (`test-with-category.yml`)

Runs tests with optional category filtering and browser/headless configuration.

**Triggers:**
- **Push to main/develop**: Runs Smoke tests on Chrome headless
- **Pull requests**: Runs Smoke tests on Chrome headless
- **Manual dispatch** (workflow_dispatch): Select category, browser, and headless mode
- **Scheduled**: Daily at 2 AM UTC (Smoke tests)

**Manual Trigger Options:**
```
Test Category: 
  - (empty - all tests)
  - Smoke
  - CategoryPages
  - CategoryFilters
  - TypeFilters
  - TitleSearch
  - Pagination
  - CombinedFilters
  - NegativeTests
  - UIValidation
  - And more...

Browser Type:
  - Chrome
  - Firefox
  - WebKit

Headless Mode:
  - true (recommended for CI)
  - false (for visual debugging)
```

**Example Usage:**
- Run all tests: Leave category empty
- Run Smoke tests: Category = "Smoke"
- Run Category tests on Firefox: Category = "CategoryPages", Browser = "Firefox"
- Run tests with browser window visible: Headless = "false"

### 2. 🌐 Comprehensive Browser Test Matrix (`test-matrix.yml`)

Runs extensive test matrix across all browser combinations to ensure compatibility.

**Triggers:**
- **Manual dispatch**: Select optional category to test
- **Scheduled**: Weekly on Monday at 3 AM UTC (all combinations)

**Browser Combinations Tested:**
- Chrome + Headless
- Chrome + Headed
- Firefox + Headless
- Firefox + Headed
- WebKit + Headless

(WebKit + Headed excluded as it's experimental in CI)

**Total Combinations:** 5 browser configurations

### 3. 🔨 Build Verification (`build.yml`)

Quick build verification to ensure code compiles correctly.

**Triggers:**
- **Push to main/develop**
- **Pull requests**

**Steps:**
- Restore dependencies
- Build solution in Release mode
- Static analysis (extensible)
- Display build artifacts

## How to Use

### Run Tests via GitHub Actions UI

1. **Go to Actions tab** in your GitHub repository
2. **Select a workflow**:
   - "Test with Category Filter" - for single run with options
   - "Comprehensive Browser Test Matrix" - for multi-browser tests
   - "Build Verification" - for build checks
3. **Click "Run workflow"** (if workflow_dispatch enabled)
4. **Fill in parameters** as needed
5. **Monitor progress** in the workflow run
6. **Download artifacts** after completion

### Automatic Triggers

- **Every push to main/develop**: Builds code and runs Smoke tests
- **Daily at 2 AM UTC**: Smoke tests on Chrome (test-with-category.yml)
- **Weekly Monday 3 AM UTC**: All test categories across all browsers (test-matrix.yml)

## Test Categories

All available categories in the test suite:

| Category | Purpose |
|----------|---------|
| Smoke | Quick sanity checks |
| CategoryPages | Tests for category page functionality |
| CategoryFilters | Tests for category filtering |
| TypeFilters | Tests for type filtering |
| TitleSearch | Tests for title search functionality |
| Pagination | Tests for pagination |
| CombinedFilters | Tests combining multiple filters |
| NegativeTests | Tests for error handling |
| UIValidation | Tests for UI element validation |

## Environment Variables

The workflows set these environment variables for test configuration:

```bash
BROWSER_TYPE=Chrome|Firefox|WebKit    # Default: Chrome
HEADLESS=true|false                    # Default: true
TEST_CATEGORY=<category_name>          # Default: empty (all tests)
```

## Artifacts & Reports

### Generated Artifacts

1. **Test Results** (`.xml` files)
   - NUnit format test results
   - Retention: 30 days

2. **HTML Reports** (in `PlaywrightTests/Reports/`)
   - Enhanced HTML test reports
   - Browser configuration details
   - Test execution summary

3. **Playwright Artifacts** (if tests fail or when headed mode)
   - Test videos
   - Screenshots
   - Logs
   - Retention: 7 days

### Download Artifacts

1. Click on the workflow run
2. Scroll to "Artifacts" section
3. Download desired artifact (e.g., test-results, reports)

## Environment Setup for Local Testing

To match CI environment locally:

```bash
# Run Smoke tests on Chrome headless
BROWSER_TYPE=Chrome HEADLESS=true dotnet test --filter "Category=Smoke"

# Run all CategoryPages tests on Firefox
BROWSER_TYPE=Firefox HEADLESS=true dotnet test --filter "Category=CategoryPages"

# Run tests with browser window visible
BROWSER_TYPE=Chrome HEADLESS=false dotnet test
```

## Configuration Files

- `.github/workflows/test-with-category.yml` - Category filtering and manual triggers
- `.github/workflows/test-matrix.yml` - Comprehensive browser matrix testing
- `.github/workflows/build.yml` - Build verification

## Best Practices

1. **Use Smoke category for PRs**: Faster feedback (most workflows default to Smoke)
2. **Run full matrix weekly**: Catch cross-browser issues
3. **Monitor test reports**: Downloaded artifacts contain detailed results
4. **Check build status**: Ensure build passes before running tests
5. **Use headed mode locally**: Better debugging experience on your machine

## Troubleshooting

### Tests timing out
- Headed mode (HEADLESS=false) may timeout in CI
- Consider increasing timeout-minutes in workflow
- Run headless mode in CI instead

### Tests failing on specific browser
- Download artifacts to see detailed logs
- Run locally with same browser: `BROWSER_TYPE=Firefox HEADLESS=true dotnet test`
- Check browser-specific test output in reports

### Build failures
- Check Build Verification workflow results
- Review build logs in GitHub Actions
- Ensure all dependencies are specified in .csproj

## Next Steps

To extend these workflows:

1. Add more test categories as needed
2. Configure Slack/Teams notifications
3. Add code coverage reporting
4. Integrate with project management tools
5. Add performance benchmarking
6. Configure environment-specific testing

## References

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [NUnit Filter Syntax](https://docs.nunit.org/articles/nunit/running-tests/Test-Selection-Language.html)
- [Playwright NUnit Documentation](https://playwright.dev/dotnet/docs/test-runners)
- [EnricoMi publish-unit-test-result-action](https://github.com/EnricoMi/publish-unit-test-result-action)
