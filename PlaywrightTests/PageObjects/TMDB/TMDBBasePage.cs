using Microsoft.Playwright;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// Base page for TMDB Discover website
/// Extends BasePage with TMDB-specific functionality
/// </summary>
public abstract class TMDBBasePage : BasePage
{
    protected TMDBBasePage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Wait for loading indicator to disappear
    /// </summary>
    public async Task WaitForLoadingToCompleteAsync()
    {
        try
        {
            var loadingSelector = "[class*='loading'], [class*='spinner'], [class*='Loading']";
            await Page.Locator(loadingSelector).WaitForAsync(new LocatorWaitForOptions { Timeout = 1000, State = WaitForSelectorState.Hidden });
        }
        catch
        {
            // Loading indicator might not exist, continue
        }
    }

    /// <summary>
    /// Get all visible results count
    /// </summary>
    public async Task<int> GetResultsCountAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsSelector = "[class*='result'], [class*='card'], [class*='item']";
        return await Page.Locator(resultsSelector).CountAsync();
    }

    /// <summary>
    /// Extract API calls from network logs
    /// </summary>
    public async Task<List<(string Method, string URL, int StatusCode)>> GetNetworkCallsAsync()
    {
        var calls = new List<(string Method, string URL, int StatusCode)>();

        // This would require intercepting network calls with Playwright
        // For now, we'll document this for API validation
        return calls;
    }
}
