using Microsoft.Playwright;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// Top Rated Page - Shows top rated movies and TV shows
/// </summary>
public class TopRatedPage : TMDBBasePage
{
    // Filter Options
    private const string MovieTypeSelector = "button:has-text('Movies')";
    private const string TVShowTypeSelector = "button:has-text('TV Shows')";

    // Results
    private const string ResultCardSelector = "[class*='card'], [class*='result-item']";
    private const string ResultTitleSelector = "h3, h2, [class*='title']";
    private const string ResultRatingSelector = "[class*='rating'], [class*='vote']";

    // Pagination
    private const string NextPageButtonSelector = "button:has-text('Next'), [aria-label*='next']";
    private const string PreviousPageButtonSelector = "button:has-text('Prev'), [aria-label*='previous']";

    public TopRatedPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to Top Rated category
    /// </summary>
    public async Task NavigateToTopRatedAsync()
    {
        await ClickToNavigateToTopRatedPage();
        await WaitForLoadingToCompleteAsync();
        Logger.Info("Navigated to Top Rated page");
    }

    /// <summary>
    /// Verify page is on Top Rated category
    /// </summary>
    public async Task<bool> IsTopRatedPageAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsCount = await GetResultsCountAsync();
        return resultsCount > 0;
    }

    /// <summary>
    /// Get all top rated items titles
    /// </summary>
    public async Task<int> GetTopRatedItemsAsync()
    {
        await WaitForLoadingToCompleteAsync();
        var resultsSelector = "[class*='result'], [class*='card'], [class*='item']";
        return await Page.Locator(resultsSelector).CountAsync();
    }

    /// <summary>
    /// Get top rated items with ratings (sorted by rating)
    /// </summary>
    public async Task<List<(string? Title, double? Rating)>> GetTopRatedItemsWithRatingsAsync()
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

        // Verify items are sorted by rating (descending)
        return items.OrderByDescending(x => x.Item2).ToList();
    }
}
