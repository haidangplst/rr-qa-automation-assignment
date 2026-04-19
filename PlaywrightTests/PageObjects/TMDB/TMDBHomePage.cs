using Microsoft.Playwright;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// HomePage for TMDB Discover - Main landing page with filters and results
/// </summary>
public class TMDBHomePage : TMDBBasePage
{
    private const string BaseUrl = "https://tmdb-discover.surge.sh/";

    private const string TypeDropdownMovieSelectoṛ = "//span[contains(@class, 'indicatorSeparator')]//..//..//div[text()='Movie']";
    private const string TypeDropdownTVShowsSelectoṛ = "//span[contains(@class, 'indicatorSeparator')]//..//..//div[text()='TV Shows']";


    // Filter Buttons
    private const string PopularButtonSelector = "button:has-text('Popular')";
    private const string TrendingButtonSelector = "button:has-text('Trending')";
    private const string NewestButtonSelector = "button:has-text('Newest')";
    private const string TopRatedButtonSelector = "button:has-text('Top Rated')";

    // Type Filters
    private const string MoviesTypeSelector = "button:has-text('Movies')";
    private const string TVShowsTypeSelector = "button:has-text('TV Shows')";

    // Search and Filter Elements
    private const string SearchInputSelector = "input[placeholder*='SEARCH']";
    private const string GenreFilterSelector = "[class*='genre'], select[name='genre']";
    private const string YearFilterSelector = "[class*='year'], select[name='year']";
    private const string RatingFilterSelector = "[class*='rating'], select[name='rating']";

    // Results
    private const string ResultsContainerSelector = "[class*='results'], [class*='grid'], [class*='list']";
    private const string ResultCardSelector = "[class*='card'], [class*='result-item'], [class*='movie-item']";
    private const string ResultTitleSelector = "[class*='title'], h2, h3";
    private const string ResultRatingSelector = "[class*='rating'], [class*='vote']";

    // Pagination
    private const string NextPageButtonSelector = "button:has-text('Next'), button:has-text('→')";
    private const string PreviousPageButtonSelector = "button:has-text('Prev'), button:has-text('←')";
    private const string PageNumberButtonSelector = "button[aria-label*='page']";
    private const string PaginationContainerSelector = "[class*='pagination']";

    // Messages
    private const string NoResultsMessageSelector = "[class*='no-results'], [class*='empty']";
    private const string LoadingIndicatorSelector = "[class*='loading'], [class*='spinner']";

    public TMDBHomePage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Navigate to TMDB Discover home page
    /// </summary>
    public async Task NavigateToHomeAsync()
    {
        await NavigateToAsync(BaseUrl);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Click on Popular filter
    /// </summary>
    public async Task FilterByPopularAsync()
    {
        await ClickAsync(PopularButtonSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Click on Trending filter
    /// </summary>
    public async Task FilterByTrendingAsync()
    {
        await ClickAsync(TrendingButtonSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Click on Newest filter
    /// </summary>
    public async Task FilterByNewestAsync()
    {
        await ClickAsync(NewestButtonSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Click on Top Rated filter
    /// </summary>
    public async Task FilterByTopRatedAsync()
    {
        await ClickAsync(TopRatedButtonSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Filter by Movies type
    /// </summary>
    public async Task FilterByMoviesTypeAsync()
    {
        await ClickAsync(MoviesTypeSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Filter by TV Shows type
    /// </summary>
    public async Task FilterByTVShowsTypeAsync()
    {
        await ClickAsync(TVShowsTypeSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Search by title
    /// </summary>
    public async Task SearchByTitleAsync(string title)
    {
        await FillAsync(SearchInputSelector, title);
        await Page.Keyboard.PressAsync("Enter");
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Clear search input
    /// </summary>
    public async Task ClearSearchAsync()
    {
        var searchInput = Page.Locator(SearchInputSelector);
        await searchInput.FillAsync("");
        await Page.Keyboard.PressAsync("Enter");
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Get search input value
    /// </summary>
    public async Task<string?> GetSearchValueAsync()
    {
        return await Page.Locator(SearchInputSelector).InputValueAsync();
    }

    /// <summary>
    /// Click on Genre filter option
    /// </summary>
    public async Task SelectGenreAsync(string genreName)
    {
        var genreSelector = $"button:has-text('{genreName}'), label:has-text('{genreName}')";
        await ClickAsync(genreSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Select year from filter
    /// </summary>
    public async Task SelectYearAsync(string year)
    {
        var yearSelector = $"option[value='{year}'], button:has-text('{year}')";
        await ClickAsync(yearSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Select rating from filter
    /// </summary>
    public async Task SelectRatingAsync(string rating)
    {
        var ratingSelector = $"option[value='{rating}'], button:has-text('{rating}')";
        await ClickAsync(ratingSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Get all result titles
    /// </summary>
    public async Task<List<string?>> GetAllResultTitlesAsync()
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
    /// Get all result ratings
    /// </summary>
    public async Task<List<double>> GetAllResultRatingsAsync()
    {
        var ratings = new List<double>();
        var ratingLocators = Page.Locator(ResultCardSelector).Locator(ResultRatingSelector);
        int count = await ratingLocators.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var ratingText = await ratingLocators.Nth(i).TextContentAsync();
            if (double.TryParse(ratingText?.Trim(), out double rating))
            {
                ratings.Add(rating);
            }
        }

        return ratings;
    }

    /// <summary>
    /// Check if no results message is displayed
    /// </summary>
    public async Task<bool> IsNoResultsMessageDisplayedAsync()
    {
        return await IsElementVisibleAsync(NoResultsMessageSelector);
    }

    /// <summary>
    /// Get no results message text
    /// </summary>
    public async Task<string?> GetNoResultsMessageAsync()
    {
        return await GetTextAsync(NoResultsMessageSelector);
    }

    /// <summary>
    /// Navigate to next page
    /// </summary>
    public async Task GoToNextPageAsync()
    {
        var nextButton = Page.Locator(NextPageButtonSelector);
        if (await nextButton.IsEnabledAsync())
        {
            await nextButton.ClickAsync();
            await WaitForLoadingToCompleteAsync();
        }
    }

    /// <summary>
    /// Navigate to previous page
    /// </summary>
    public async Task GoToPreviousPageAsync()
    {
        var prevButton = Page.Locator(PreviousPageButtonSelector);
        if (await prevButton.IsEnabledAsync())
        {
            await prevButton.ClickAsync();
            await WaitForLoadingToCompleteAsync();
        }
    }

    /// <summary>
    /// Go to specific page number
    /// </summary>
    public async Task GoToPageAsync(int pageNumber)
    {
        var pageSelector = $"button[aria-label*='{pageNumber}'], button:has-text('{pageNumber}')";
        await ClickAsync(pageSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Check if next page button is enabled
    /// </summary>
    public async Task<bool> IsNextPageEnabledAsync()
    {
        return await Page.Locator(NextPageButtonSelector).IsEnabledAsync();
    }

    /// <summary>
    /// Check if previous page button is enabled
    /// </summary>
    public async Task<bool> IsPreviousPageEnabledAsync()
    {
        return await Page.Locator(PreviousPageButtonSelector).IsEnabledAsync();
    }

    /// <summary>
    /// Get current page number from UI
    /// </summary>
    public async Task<int> GetCurrentPageNumberAsync()
    {
        var currentPageSelector = "[class*='current'], [aria-current='page']";
        var pageText = await GetTextAsync(currentPageSelector);
        if (int.TryParse(pageText?.Trim(), out int page))
        {
            return page;
        }
        return 1; // Default to page 1
    }

    /// <summary>
    /// Check if category filter is active
    /// </summary>
    public async Task<bool> IsCategoryActiveAsync(string categoryName)
    {
        var selector = $"button:has-text('{categoryName}')[class*='active'], button:has-text('{categoryName}'):not([disabled])";
        try
        {
            var element = Page.Locator(selector);
            return await element.IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verify page title
    /// </summary>
    public async Task<bool> IsTitleCorrectAsync(string expectedTitle)
    {
        var title = await GetTitleAsync();
        return title.Contains(expectedTitle);
    }

    /// <summary>
    /// Get filter panel HTML for debugging
    /// </summary>
    public async Task<string?> GetFilterPanelHTMLAsync()
    {
        var filterPanel = "[class*='filter'], aside, .sidebar";
        return await Page.Locator(filterPanel).InnerHTMLAsync();
    }

    /// <summary>
    /// Filter to Movies only on Popular page
    /// </summary>
    public async Task FilterToMoviesAsync()
    {
        // Check current selection
        var currentSelection = await Page.EvaluateAsync<string>(@"
            () => {
                const singleValue = document.querySelector('.css-1uccc91-singleValue');
                return singleValue ? singleValue.textContent.trim() : '';
            }
        ");

        if (currentSelection == "Movie")
        {
            Logger.Info("Movie filter already selected");
            return;
        }

        await ClickAsync(TypeDropdownTVShowsSelectoṛ);
        await Page.Locator("[class*='option']").Filter(new() { HasText = "Movie" }).ClickAsync();

        await WaitForLoadingToCompleteAsync();

        var sellectedOption = await Page.EvaluateAsync<string>(@"
            () => {
                const singleValue = document.querySelector('.css-1uccc91-singleValue');
                return singleValue ? singleValue.textContent.trim() : '';
            }
        ");
        sellectedOption.Equals("Movie", StringComparison.OrdinalIgnoreCase);

        Logger.Info("Filtered to Movies on Popular page");
    }

    /// <summary>
    /// Filter to TV Shows only on Popular page
    /// </summary>
    public async Task FilterToTVShowsAsync()
    {
        var currentSelection = await Page.EvaluateAsync<string>(@"
            () => {
                const singleValue = document.querySelector('.css-1uccc91-singleValue');
                return singleValue ? singleValue.textContent.trim() : '';
            }
        ");

        if (currentSelection == "TV Shows")
        {
            Logger.Info("Movie filter already selected");
            return;
        }

        await ClickAsync(TypeDropdownMovieSelectoṛ);
        await Page.Locator("[class*='option']").Filter(new() { HasText = "TV Shows" }).ClickAsync();

        await WaitForLoadingToCompleteAsync();

        var sellectedOption = await Page.EvaluateAsync<string>(@"
            () => {
                const singleValue = document.querySelector('.css-1uccc91-singleValue');
                return singleValue ? singleValue.textContent.trim() : '';
            }
        ");
        sellectedOption.Equals("TV Shows", StringComparison.OrdinalIgnoreCase);

        Logger.Info("Filtered to Movies on Popular page");
    }
}
