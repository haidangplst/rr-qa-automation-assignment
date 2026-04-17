using Microsoft.Playwright;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Factory for creating browser instances based on configuration
/// </summary>
public class BrowserFactory
{
    /// <summary>
    /// Create a browser instance based on TestConfig settings
    /// </summary>
    public static async Task<IBrowser> CreateBrowserAsync(IPlaywright playwright)
    {
        var browserType = TestConfig.SelectedBrowser;
        var headless = TestConfig.Headless;

        Console.WriteLine($"🌐 Creating {browserType} browser (Headless: {headless})...");

        return browserType switch
        {
            Configuration.BrowserType.Chrome => await CreateChromeAsync(playwright),
            Configuration.BrowserType.Firefox => await CreateFirefoxAsync(playwright),
            Configuration.BrowserType.WebKit => await CreateWebKitAsync(playwright),
            _ => throw new ArgumentException($"Unknown browser type: {browserType}")
        };
    }

    /// <summary>
    /// Create Chrome browser instance
    /// </summary>
    private static async Task<IBrowser> CreateChromeAsync(IPlaywright playwright)
    {
        try
        {
            var options = TestConfig.GetChromeLaunchOptions();
            var browser = await playwright.Chromium.LaunchAsync(options);
            Console.WriteLine("✅ Chrome browser launched successfully");
            return browser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to launch Chrome: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Create Firefox browser instance
    /// </summary>
    private static async Task<IBrowser> CreateFirefoxAsync(IPlaywright playwright)
    {
        try
        {
            var options = TestConfig.GetFirefoxLaunchOptions();
            var browser = await playwright.Firefox.LaunchAsync(options);
            Console.WriteLine("✅ Firefox browser launched successfully");
            return browser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to launch Firefox: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Create WebKit browser instance
    /// </summary>
    private static async Task<IBrowser> CreateWebKitAsync(IPlaywright playwright)
    {
        try
        {
            var options = TestConfig.GetWebKitLaunchOptions();
            // Note: WebKit is available as webkit property in newer Playwright versions
            var browser = await playwright.Webkit.LaunchAsync(options);
            Console.WriteLine("✅ WebKit browser launched successfully");
            return browser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to launch WebKit: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get browser name with headless mode
    /// </summary>
    public static string GetBrowserConfigurationName()
    {
        var browserName = TestConfig.GetBrowserTypeName();
        var mode = TestConfig.Headless ? "Headless" : "Headed";
        return $"{browserName}_{mode}";
    }
}

