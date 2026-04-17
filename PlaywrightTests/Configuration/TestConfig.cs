using Microsoft.Playwright;

namespace PlaywrightTests.Configuration;

/// <summary>
/// Browser type enumeration
/// </summary>
public enum BrowserType
{
    Chrome,
    Firefox,
    WebKit
}

/// <summary>
/// Configuration class for Playwright test execution
/// </summary>
public class TestConfig
{
    /// <summary>
    /// Selected browser type
    /// </summary>
    public static BrowserType SelectedBrowser { get; set; } = BrowserType.Chrome;

    /// <summary>
    /// Base URL for the application under test
    /// </summary>
    public static string BaseUrl { get; set; } = "https://tmdb-discover.surge.sh/";

    /// <summary>
    /// Default timeout for element operations (in milliseconds)
    /// </summary>
    public static int DefaultTimeout { get; set; } = 5000;

    /// <summary>
    /// Default timeout for navigation (in milliseconds)
    /// </summary>
    public static int NavigationTimeout { get; set; } = 30000;

    /// <summary>
    /// Browser headless mode
    /// </summary>
    public static bool Headless { get; set; } = true;

    /// <summary>
    /// Enable video recording for failed tests
    /// </summary>
    public static bool RecordVideo { get; set; } = false;

    /// <summary>
    /// Viewport width
    /// </summary>
    public static int ViewportWidth { get; set; } = 1920;

    /// <summary>
    /// Viewport height
    /// </summary>
    public static int ViewportHeight { get; set; } = 1080;

    /// <summary>
    /// Number of parallel tests
    /// </summary>
    public static int ParallelTests { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Load configuration from environment variables
    /// </summary>
    public static void LoadFromEnvironment()
    {
        // Browser type: BROWSER_TYPE=Chrome|Firefox|WebKit
        if (Enum.TryParse<BrowserType>(Environment.GetEnvironmentVariable("BROWSER_TYPE"), out var browserType))
            SelectedBrowser = browserType;

        if (bool.TryParse(Environment.GetEnvironmentVariable("HEADLESS"), out var headless))
            Headless = headless;

        if (int.TryParse(Environment.GetEnvironmentVariable("TIMEOUT"), out var timeout))
            DefaultTimeout = timeout;

        if (int.TryParse(Environment.GetEnvironmentVariable("NAV_TIMEOUT"), out var navTimeout))
            NavigationTimeout = navTimeout;

        if (bool.TryParse(Environment.GetEnvironmentVariable("RECORD_VIDEO"), out var recordVideo))
            RecordVideo = recordVideo;

        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        if (!string.IsNullOrEmpty(baseUrl))
            BaseUrl = baseUrl;
    }

    /// <summary>
    /// Get browser type name
    /// </summary>
    public static string GetBrowserTypeName() => SelectedBrowser.ToString();

    /// <summary>
    /// Get Chrome launch arguments
    /// </summary>
    public static string[] GetChromeLaunchArgs()
    {
        return new[]
        {
            "--disable-blink-features=AutomationControlled",
            "--disable-dev-shm-usage",
            "--no-first-run",
            "--no-default-browser-check",
            "--disable-gpu"
        };
    }

    /// <summary>
    /// Get Firefox launch arguments
    /// </summary>
    public static string[] GetFirefoxLaunchArgs()
    {
        return new[]
        {
            "-use-temp-profile"
        };
    }

    /// <summary>
    /// Get Chrome launch options based on current configuration
    /// </summary>
    public static BrowserTypeLaunchOptions GetChromeLaunchOptions()
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = Headless,
            Args = GetChromeLaunchArgs()
        };
    }

    /// <summary>
    /// Get Firefox launch options based on current configuration
    /// </summary>
    public static BrowserTypeLaunchOptions GetFirefoxLaunchOptions()
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = Headless,
            Args = GetFirefoxLaunchArgs()
        };
    }

    /// <summary>
    /// Get WebKit launch options based on current configuration
    /// </summary>
    public static BrowserTypeLaunchOptions GetWebKitLaunchOptions()
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = Headless
        };
    }

    /// <summary>
    /// Get launch options for the selected browser
    /// </summary>
    public static BrowserTypeLaunchOptions GetBrowserLaunchOptions()
    {
        return SelectedBrowser switch
        {
            BrowserType.Chrome => GetChromeLaunchOptions(),
            BrowserType.Firefox => GetFirefoxLaunchOptions(),
            BrowserType.WebKit => GetWebKitLaunchOptions(),
            _ => throw new ArgumentException($"Unknown browser type: {SelectedBrowser}")
        };
    }

    /// <summary>
    /// Get browser context options
    /// </summary>
    public static BrowserNewContextOptions GetContextOptions()
    {
        var options = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = ViewportWidth, Height = ViewportHeight },
            IgnoreHTTPSErrors = true,
        };

        if (RecordVideo)
        {
            var videoDir = Path.Combine(AppContext.BaseDirectory, "Videos");
            Directory.CreateDirectory(videoDir);
            options.RecordVideoDir = videoDir;
        }

        return options;
    }
}
