using PlaywrightTests.Configuration;

namespace PlaywrightTests.Configuration;

/// <summary>
/// Provides browser test matrix for parameterized testing
/// </summary>
public static class BrowserTestMatrix
{
    /// <summary>
    /// Get all browser configurations for testing
    /// </summary>
    public static IEnumerable<BrowserConfiguration> GetBrowserConfigurations()
    {
        // Chrome configurations
        yield return new BrowserConfiguration(BrowserType.Chrome, Headless: true);
        yield return new BrowserConfiguration(BrowserType.Chrome, Headless: false);

        // Firefox configurations
        yield return new BrowserConfiguration(BrowserType.Firefox, Headless: true);
        yield return new BrowserConfiguration(BrowserType.Firefox, Headless: false);

        // WebKit configurations
        yield return new BrowserConfiguration(BrowserType.WebKit, Headless: true);
        yield return new BrowserConfiguration(BrowserType.WebKit, Headless: false);
    }

    /// <summary>
    /// Get browser configurations filtered by environment variable
    /// Set BROWSER_TYPE=Chrome to only test Chrome
    /// </summary>
    public static IEnumerable<BrowserConfiguration> GetConfiguredBrowsers()
    {
        var envBrowser = Environment.GetEnvironmentVariable("BROWSER_TYPE");
        var all = GetBrowserConfigurations();

        if (string.IsNullOrEmpty(envBrowser))
            return all;

        if (Enum.TryParse<BrowserType>(envBrowser, out var selectedType))
            return all.Where(cfg => cfg.Type == selectedType);

        return all;
    }

    /// <summary>
    /// Get quick test matrix (Chrome and Firefox, headless only)
    /// </summary>
    public static IEnumerable<BrowserConfiguration> GetQuickTestMatrix()
    {
        yield return new BrowserConfiguration(BrowserType.Chrome, Headless: true);
        yield return new BrowserConfiguration(BrowserType.Firefox, Headless: true);
    }

    /// <summary>
    /// Get comprehensive test matrix (all combinations)
    /// </summary>
    public static IEnumerable<BrowserConfiguration> GetComprehensiveTestMatrix()
    {
        return GetBrowserConfigurations();
    }
}
