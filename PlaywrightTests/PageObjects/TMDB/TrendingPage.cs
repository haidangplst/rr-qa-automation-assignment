using Microsoft.Playwright;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// Trending Page - Shows trending movies and TV shows
/// </summary>
public class TrendingPage : TMDBBasePage
{
    // Results
    private const string ResultCardSelector = "[class*='card'], [class*='result-item']";
    private const string ResultTitleSelector = "h3, h2, [class*='title']";
    private const string ResultRatingSelector = "[class*='rating'], [class*='vote']";

    // Pagination
    private const string NextPageButtonSelector = "button:has-text('Next'), [aria-label*='next']";
    private const string PreviousPageButtonSelector = "button:has-text('Prev'), [aria-label*='previous']";

    public TrendingPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to Trending category
    /// </summary>
    public async Task NavigateToTrendingAsync()
    {
        await ClickToNavigateToTrendingPage();
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Navigated to Trending page");
    }

    /// <summary>
    /// Verify page is on Trending category
    /// </summary>
    public async Task<bool> IsTrendingPageAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsCount = await GetResultsCountAsync();
        return resultsCount > 0;
    }

    /// <summary>
    /// Get all trending items titles
    /// </summary>
    public async Task<int> GetTrendingItemsAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsSelector = "[class*='result'], [class*='card'], [class*='item']";
        return await Page.Locator(resultsSelector).CountAsync();
    }

    /// <summary>
    /// Get trending items with ratings
    /// </summary>
    public async Task<List<(string? Title, double? Rating)>> GetTrendingItemsWithRatingsAsync()
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
}
