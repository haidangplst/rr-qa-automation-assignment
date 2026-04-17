using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Fixtures;

[SetUpFixture]
public class BrowserFixture
{
    private static IBrowserContext? _context;
    private static IPage? _page;
    private static IBrowser? _browser;
    private static IPlaywright? _playwright;

    public static IBrowser? Browser => _browser;
    public static IPage? Page => _page;
    public static IBrowserContext? Context => _context;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
        {
            // Load configuration from environment
            TestConfig.LoadFromEnvironment();

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("🔧 TEST CONFIGURATION");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Browser: {TestConfig.GetBrowserTypeName()}");
            Console.WriteLine($"Headless: {TestConfig.Headless}");
            Console.WriteLine($"Base URL: {TestConfig.BaseUrl}");
            Console.WriteLine($"Viewport: {TestConfig.ViewportWidth}x{TestConfig.ViewportHeight}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

            _playwright = await Playwright.CreateAsync();
            _browser = await BrowserFactory.CreateBrowserAsync(_playwright);

            _context = await _browser.NewContextAsync(TestConfig.GetContextOptions());
            _page = await _context.NewPageAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in BrowserFixture.OneTimeSetUp: {ex.Message}");
            throw;
        }
    }

    [SetUp]
    public async Task SetUp()
    {
        try
        {
            // Ensure we have a fresh page for each test
            if (_page != null)
            {
                await _page.CloseAsync();
            }

            _page = await _context!.NewPageAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in BrowserFixture.SetUp: {ex.Message}");
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            if (_page != null)
            {
                await _page.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in BrowserFixture.TearDown: {ex.Message}");
        }
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        try
        {
            // Generate report after all tests complete
            Console.WriteLine("\n📊 Generating test report...");
            var reportGenerator = new EnhancedHTMLReportGenerator();
            var reportPath = reportGenerator.GenerateReport();
            Console.WriteLine($"✅ Report generated: {reportPath}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating report: {ex.Message}");
        }

        try
        {
            if (_page != null)
            {
                await _page.CloseAsync();
            }
            if (_context != null)
            {
                await _context.CloseAsync();
            }
            if (_browser != null)
            {
                await _browser.CloseAsync();
                Console.WriteLine($"✅ {TestConfig.GetBrowserTypeName()} browser closed\n");
            }
            _playwright?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in BrowserFixture.OneTimeTearDown: {ex.Message}");
        }
    }
}

