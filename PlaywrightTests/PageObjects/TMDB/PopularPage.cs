using Microsoft.Playwright;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// Popular Page - Shows popular movies and TV shows
/// </summary>
public class PopularPage : TMDBBasePage
{
    private const string BaseUrl = "https://tmdb-discover.surge.sh/";

    // Page Indicators
    private const string PageTitleSelector = "h1";
    private const string CategoryLabelSelector = "[class*='title'], [class*='header']";

    // Filter Options
    private const string MovieTypeSelector = "button:has-text('Movies')";
    private const string TVShowTypeSelector = "button:has-text('TV Shows')";

    // Results
    private const string ResultCardSelector = "[class*='card'], [class*='result-item']";
    private const string ResultTitleSelector = "h3, h2, [class*='title']";
    private const string ResultRatingSelector = "[class*='rating'], [class*='vote']";
    private const string ResultImageSelector = "img";

    // Pagination
    private const string NextPageButtonSelector = "button:has-text('Next'), [aria-label*='next']";
    private const string PreviousPageButtonSelector = "button:has-text('Prev'), [aria-label*='previous']";

    public PopularPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to Popular category
    /// </summary>
    public async Task NavigateToPopularAsync()
    {
        await NavigateToAsync(BaseUrl);
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Navigated to Popular page");
    }

    /// <summary>
    /// Verify page is on Popular category
    /// </summary>
    public async Task<bool> IsPopularPageAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsCount = await GetResultsCountAsync();
        return resultsCount > 0;
    }

    /// <summary>
    /// Filter to Movies only on Popular page
    /// </summary>
    public async Task FilterToMoviesAsync()
    {
        await ClickAsync(MovieTypeSelector);
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Filtered to Movies on Popular page");
    }

    /// <summary>
    /// Filter to TV Shows only on Popular page
    /// </summary>
    public async Task FilterToTVShowsAsync()
    {
        await ClickAsync(TVShowTypeSelector);
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Filtered to TV Shows on Popular page");
    }

    /// <summary>
    /// Get all popular movies/shows titles
    /// </summary>
    public async Task<List<string?>> GetPopularItemsAsync()
    {
        var titles = new List<string?>();
        var titleLocators = Page.Locator(ResultCardSelector).Locator(ResultTitleSelector);
        int count = await titleLocators.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var title = await titleLocators.Nth(i).TextContentAsync();
            if (!string.IsNullOrEmpty(title))
            {
                titles.Add(title.Trim());
            }
        }

        return titles;
    }

    /// <summary>
    /// Get popular items with their ratings
    /// </summary>
    public async Task<List<(string? Title, double? Rating)>> GetPopularItemsWithRatingsAsync()
    {
        var items = new List<(string?, double?)>();
        var cardLocators = Page.Locator(ResultCardSelector);
        int count = await cardLocators.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var card = cardLocators.Nth(i);
            var title = await card.Locator(ResultTitleSelector).TextContentAsync();
            var ratingText = await card.Locator(ResultRatingSelector).TextContentAsync();

            double? rating = null;
            if (double.TryParse(ratingText?.Trim(), out double parsedRating))
            {
                rating = parsedRating;
            }

            items.Add((title?.Trim(), rating));
        }

        return items;
    }

    /// <summary>
    /// Navigate to next page of popular items
    /// </summary>
    public async Task GoToNextPageAsync()
    {
        var nextButton = Page.Locator(NextPageButtonSelector);
        if (await nextButton.IsEnabledAsync())
        {
            await nextButton.ClickAsync();
            await WaitForLoadingToCompleteAsync();
            Logger.Info("Navigated to next page of Popular");
        }
    }

    /// <summary>
    /// Navigate to previous page of popular items
    /// </summary>
    public async Task GoToPreviousPageAsync()
    {
        var prevButton = Page.Locator(PreviousPageButtonSelector);
        if (await prevButton.IsEnabledAsync())
        {
            await prevButton.ClickAsync();
            await WaitForLoadingToCompleteAsync();
            Logger.Info("Navigated to previous page of Popular");
        }
    }
}
