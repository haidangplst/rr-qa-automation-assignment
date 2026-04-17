using Microsoft.Playwright;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Utility class for common browser operations
/// </summary>
public static class BrowserUtils
{
    /// <summary>
    /// Get Chrome launch options with custom settings
    /// </summary>
    public static BrowserTypeLaunchOptions GetChromeOptions(bool headless = true)
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = headless,
            Channel = "chrome",
            Args = new[]
            {
                "--disable-blink-features=AutomationControlled",
                "--disable-dev-shm-usage",
                "--no-first-run",
                "--no-default-browser-check"
            }
        };
    }

    /// <summary>
    /// Create context with specific viewport size
    /// </summary>
    public static BrowserNewContextOptions GetContextOptions(int width = 1920, int height = 1080)
    {
        return new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = width, Height = height },
            IgnoreHTTPSErrors = true,
        };
    }

    /// <summary>
    /// Wait for element and take screenshot
    /// </summary>
    public static async Task TakeScreenshotAsync(IPage page, string fileName)
    {
        var screenshotDir = Path.Combine(AppContext.BaseDirectory, "Screenshots");
        Directory.CreateDirectory(screenshotDir);

        var filePath = Path.Combine(screenshotDir, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = filePath });
    }

    /// <summary>
    /// Get all cookies from the current context
    /// </summary>
    public static async Task<List<BrowserContextCookiesResult>> GetCookiesAsync(IBrowserContext context)
    {
        var cookies = await context.CookiesAsync();
        return cookies.ToList();
    }
}
