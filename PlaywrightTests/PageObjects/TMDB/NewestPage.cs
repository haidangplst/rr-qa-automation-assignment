using Microsoft.Playwright;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// Newest Page - Shows newest movies and TV shows
/// </summary>
public class NewestPage : TMDBBasePage
{
    private const string BaseUrl = "https://tmdb-discover.surge.sh/";

    // Filter Options
    private const string MovieTypeSelector = "button:has-text('Movies')";
    private const string TVShowTypeSelector = "button:has-text('TV Shows')";

    // Results
    private const string ResultCardSelector = "[class*='card'], [class*='result-item']";
    private const string ResultTitleSelector = "h3, h2, [class*='title']";
    private const string ResultReleaseDateSelector = "[class*='date'], [class*='release']";
    private const string ResultRatingSelector = "[class*='rating'], [class*='vote']";

    // Pagination
    private const string NextPageButtonSelector = "button:has-text('Next'), [aria-label*='next']";
    private const string PreviousPageButtonSelector = "button:has-text('Prev'), [aria-label*='previous']";

    public NewestPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to Newest category
    /// </summary>
    public async Task NavigateToNewestAsync()
    {
        await ClickToNavigateToNewestPage();
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Navigated to Newest page");
    }

    /// <summary>
    /// Verify page is on Newest category
    /// </summary>
    public async Task<bool> IsNewestPageAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsCount = await GetResultsCountAsync();
        return resultsCount > 0;
    }

    /// <summary>
    /// Filter to Movies only on Newest page
    /// </summary>
    public async Task FilterToMoviesAsync()
    {
        await ClickAsync(MovieTypeSelector);
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Filtered to Movies on Newest page");
    }

    /// <summary>
    /// Filter to TV Shows only on Newest page
    /// </summary>
    public async Task FilterToTVShowsAsync()
    {
        await ClickAsync(TVShowTypeSelector);
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Filtered to TV Shows on Newest page");
    }

    /// <summary>
    /// Get all newest items titles
    /// </summary>
    public async Task<int> GetNewestItemsAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsSelector = "[class*='result'], [class*='card'], [class*='item']";
        return await Page.Locator(resultsSelector).CountAsync();
    }

    /// <summary>
    /// Get newest items with release dates
    /// </summary>
    public async Task<List<(string? Title, string? ReleaseDate)>> GetNewestItemsWithDatesAsync()
    {
        var items = new List<(string?, string?)>();
        var cardLocators = Page.Locator(ResultCardSelector);
        int count = await cardLocators.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var card = cardLocators.Nth(i);
            var title = await card.Locator(ResultTitleSelector).TextContentAsync();
            var date = await card.Locator(ResultReleaseDateSelector).TextContentAsync();

            items.Add((title?.Trim(), date?.Trim()));
        }

        return items;
    }

    /// <summary>
    /// Navigate to next page of newest items
    /// </summary>
    public async Task GoToNextPageAsync()
    {
        var nextButton = Page.Locator(NextPageButtonSelector);
        if (await nextButton.IsEnabledAsync())
        {
            await nextButton.ClickAsync();
            await WaitForLoadingToCompleteAsync();
            Logger.Info("Navigated to next page of Newest");
        }
    }

    /// <summary>
    /// Navigate to previous page of newest items
    /// </summary>
    public async Task GoToPreviousPageAsync()
    {
        var prevButton = Page.Locator(PreviousPageButtonSelector);
        if (await prevButton.IsEnabledAsync())
        {
            await prevButton.ClickAsync();
            await WaitForLoadingToCompleteAsync();
            Logger.Info("Navigated to previous page of Newest");
        }
    }
}
