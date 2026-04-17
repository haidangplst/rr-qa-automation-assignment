using Microsoft.Playwright;
using NUnit.Framework;
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
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true, // Set to false to see the browser
            });

            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in BrowserFixture.OneTimeSetUp: {ex.Message}");
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
            Console.WriteLine($"Error in BrowserFixture.SetUp: {ex.Message}");
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
            Console.WriteLine($"Error in BrowserFixture.TearDown: {ex.Message}");
        }
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        try
        {
            // Generate report after all tests complete
            Console.WriteLine("Generating test report...");
            var reportGenerator = new EnhancedHTMLReportGenerator();
            var reportPath = reportGenerator.GenerateReport();
            Console.WriteLine($"Report generated: {reportPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating report: {ex.Message}");
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
            }
            _playwright?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in BrowserFixture.OneTimeTearDown: {ex.Message}");
        }
    }
}
