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
    private const string DiscoveryOptionTitle = "//aside//p[text() = 'DISCOVER OPTIONS']";
    private const string DiscoveryOptionFilterTypeName = "//aside//p[text()='DISCOVER OPTIONS']/following-sibling::div//p";
    private const string NavigationBar = "//nav//ul//li//a";

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
    private const string ResultCardSelector = "[class*='card'], [class*='result-item'], [class*='movie-item']";
    private const string ResultTitleSelector = "[class*='title'], h2, h3";
    private const string ResultRatingSelector = "[class*='rating'], [class*='vote']";

    // Pagination
    private const string NextPageButtonSelector = "button:has-text('Next'), button:has-text('→')";
    private const string PreviousPageButtonDisabledSelector = "//li[@class = 'previous disabled']//a[@aria-label = 'Previous page']";
    private const string PageNumberButtonSelector = "//li//a[contains(@aria-label, 'Page')]";
    private const string TheDefaultPage = "//li//a[@aria-label = 'Page 1 is your current page']";
    // Messages
    private const string NoResultsMessageSelector = "[class*='no-results'], [class*='empty']";
    private const string SomethingWentWrongMessage = "//div[text() = 'Something went wrong! Please try again later.']";

    public static string PageSelectorTemplate(int page = 1)
        => $"//li//a[@aria-label='Page {page}']";

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
    /// Wait for Discovery Option fields to be displayed and verify filter type names
    /// </summary>
    public async Task WaitForDiscoveryOptionFieldsDisplayedAsync()
    {
        await WaitForElementAsync(DiscoveryOptionTitle);
        await WaitForElementAsync(TypeDropdownMovieSelectoṛ);  // the Movie option is default sellected

        // Get all filter type names from Discovery Options
        var filterTypeNames = new List<string>();
        var filterTypeLocators = Page.Locator(DiscoveryOptionFilterTypeName);
        int count = await filterTypeLocators.CountAsync();

        Logger.Info($"Found {count} Discovery Option filter types");

        for (int i = 0; i < count; i++)
        {
            var text = await filterTypeLocators.Nth(i).TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var cleanText = text.Trim();
                filterTypeNames.Add(cleanText);
                Logger.Info($"  - Filter type {i + 1}: {cleanText}");
            }
        }

        // Verify expected filter types exist
        var expectedFilterTypes = new List<string>
    {
        "TYPE",
        "GENRE",
        "YEAR",
        "RATINGS"
    };

        var missingFilters = new List<string>();
        foreach (var expected in expectedFilterTypes)
        {
            if (!filterTypeNames.Any(f => f.Equals(expected, StringComparison.OrdinalIgnoreCase)))
            {
                missingFilters.Add(expected);
            }
        }

        if (missingFilters.Any())
        {
            var errorMessage = $"Missing expected filter types: {string.Join(", ", missingFilters)}";
            Logger.Warning(errorMessage);
            throw new Exception(errorMessage);
        }
        else
        {
            Logger.Info($"✓ All expected Discovery Option filter types are displayed");
        }
    }

    public async Task VerifyTheNavigationBarMenu()
    {
        // Get all filter type names from Discovery Options
        var filterTypeNames = new List<string>();
        var filterTypeLocators = Page.Locator(NavigationBar);
        int count = await filterTypeLocators.CountAsync();

        Logger.Info($"Found {count} Discovery Option filter types");

        for (int i = 0; i < count; i++)
        {
            var text = await filterTypeLocators.Nth(i).TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var cleanText = text.Trim();
                filterTypeNames.Add(cleanText);
                Logger.Info($"  - Filter type {i + 1}: {cleanText}");
            }
        }

        // Verify expected filter types exist
        var expectedFilterTypes = new List<string>
    {
        "Popular",
        "Trend",
        "Newest",
        "Top rated"
    };

        var missingFilters = new List<string>();
        foreach (var expected in expectedFilterTypes)
        {
            if (!filterTypeNames.Any(f => f.Equals(expected, StringComparison.OrdinalIgnoreCase)))
            {
                missingFilters.Add(expected);
            }
        }

        if (missingFilters.Any())
        {
            var errorMessage = $"Missing expected menu: {string.Join(", ", missingFilters)}";
            Logger.Warning(errorMessage);
            throw new Exception(errorMessage);
        }
        else
        {
            Logger.Info($"✓ All expected Menu Option are displayed");
        }

        await WaitForElementAsync(GetNavigationBarSelectedSelector("white", "popular"));  //validate the menu is sellected

        await WaitForElementAsync(GetNavigationBarSelectedSelector("blue", "trend"));  //validate the menu is sellected

        await WaitForElementAsync(GetNavigationBarSelectedSelector("blue", "top"));  //validate the menu is sellected

        await WaitForElementAsync(GetNavigationBarSelectedSelector("blue", "new"));  //validate the menu is sellected
    }

    public async Task VerifyTheSearchBoxDisplayed()
    {
        await WaitForElementAsync(SearchInputSelector);
    }

    /// <summary>
    /// Verify the default displayed page is page 1 and previous button is disabled
    /// </summary>
    public async Task VerifyTheDefaultDisplayedPage()
    {
        await WaitForElementAsync(TheDefaultPage);

        // Get current page number
        var currentPage = await GetCurrentPageNumberAsync();
        Logger.Info($"Current page: {currentPage}");

        // Verify current page should be 1
        if (currentPage != 1)
        {
            var errorMessage = $"Expected current page to be 1, but found page {currentPage}";
            Logger.Error(errorMessage);
            throw new Exception(errorMessage);
        }
        else
        {
            Logger.Info($"✓ Current page is 1 as expected");
        }

        // Verify previous button is disabled
        await WaitForElementAsync(PreviousPageButtonDisabledSelector);
        Logger.Info($"✓ Previous page button is disabled as expected");
    }

    /// <summary>
    /// Get all page numbers from pagination buttons, print and return the list
    /// </summary>
    /// <returns>List of page numbers as strings</returns>
    public async Task<List<string>> ValidateThePageSelectorNumber()
    {
        await WaitForLoadingToCompleteAsync();

        var pageNumbers = new List<string>();
        var pageButtonLocators = Page.Locator(PageNumberButtonSelector);
        int count = await pageButtonLocators.CountAsync();

        Logger.Info($"Found {count} page number buttons");

        for (int i = 0; i < count; i++)
        {
            var text = await pageButtonLocators.Nth(i).TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var cleanText = text.Trim();
                pageNumbers.Add(cleanText);
                Logger.Info($"  Page button {i + 1}: {cleanText}");
            }
        }

        // Print summary
        Logger.Info($"📄 Pagination Summary:");
        Logger.Info($"   Total page buttons: {pageNumbers.Count}");
        Logger.Info($"   Page numbers: {string.Join(", ", pageNumbers)}");

        return pageNumbers;
    }

    /// <summary>
    /// Validate error messages and fail test if any errors found
    /// </summary>
    /// <param name="errorMessages">List of error messages to check</param>
    /// <param name="context">Context description for the error</param>
    public void ValidateNoErrors(List<string> errorMessages, string context = "Validation")
    {
        if (errorMessages != null && errorMessages.Count > 0)
        {
            var errorMess = string.Join("; ", errorMessages);
            Logger.Error($"❌ {context} failed with {errorMessages.Count} error(s):");

            foreach (var error in errorMessages)
            {
                Logger.Error($"  - {error}");
            }

            // Mark test as failed and throw exception to stop execution
            throw new Exception($"{context} failed: {errorMess}");
        }
        else
        {
            Logger.Info($"✓ {context} passed - no errors found");
        }
    }

    /// <summary>
    /// Validate a single error message and fail test if error exists
    /// </summary>
    /// <param name="errorMess">Error message to check</param>
    /// <param name="context">Context description for the error</param>
    public void ValidateNoError(string errorMess, string context = "Validation")
    {
        if (!string.IsNullOrEmpty(errorMess) && errorMess.Length > 0)
        {
            Logger.Error($"❌ {context} failed: {errorMess}");

            // Mark test as failed and throw exception to stop execution
            throw new Exception($"{context} failed: {errorMess}");
        }
        else
        {
            Logger.Info($"✓ {context} passed - no errors found");
        }
    }

    /// <summary>
    /// Select a pagination page - if page number is 0 or negative, select a random available page
    /// </summary>
    /// <param name="page">Page number to select (0 or negative for random selection)</param>
    /// <returns>The actual page number that was selected</returns>
    public async Task<int> SelectPaginationPage(int page)
    {
        int selectedPage = page;

        // If page is 0 or negative, select a random page
        if (page <= 0)
        {
            // Get available page numbers
            var pageNumbers = await ValidateThePageSelectorNumber();

            if (pageNumbers.Count == 0)
            {
                throw new Exception("No pagination buttons found");
            }

            // Select a random page from available pages
            var random = new Random();
            int randomIndex = random.Next(0, pageNumbers.Count);

            // Parse the page number from string
            if (int.TryParse(pageNumbers[randomIndex], out int randomPage))
            {
                selectedPage = randomPage;
                Logger.Info($"🎲 Randomly selected page: {selectedPage} from available pages: {string.Join(", ", pageNumbers)}");
            }
            else
            {
                throw new Exception($"Failed to parse page number from: {pageNumbers[randomIndex]}");
            }
        }
        else
        {
            Logger.Info($"Selecting page number: {selectedPage}");
        }

        await ClickAsync(PageSelectorTemplate(selectedPage));
        await VerifyCurrentPageIsAsync(selectedPage);

        return selectedPage;
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
    //public async Task GoToPreviousPageAsync()
    //{
    //    var prevButton = Page.Locator(PreviousPageButtonSelector);
    //    if (await prevButton.IsEnabledAsync())
    //    {
    //        await prevButton.ClickAsync();
    //        await WaitForLoadingToCompleteAsync();
    //    }
    //}

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

    ///// <summary>
    ///// Check if previous page button is enabled
    ///// </summary>
    //public async Task<bool> IsPreviousPageEnabledAsync()
    //{
    //    return await Page.Locator(PreviousPageButtonSelector).IsEnabledAsync();
    //}

    /// <summary>
    /// Get current page number from UI
    /// </summary>
    public async Task<int> GetCurrentPageNumberAsync()
    {
        var currentPageSelector = "[aria-current='page']";
        var pageText = await GetTextAsync(currentPageSelector);
        if (int.TryParse(pageText?.Trim(), out int page))
        {
            return page;
        }
        return 1; // Default to page 1
    }

    /// <summary>
    /// Verify the current page matches the expected page number
    /// </summary>
    /// <param name="expectedPage">The expected page number</param>
    public async Task VerifyCurrentPageIsAsync(int expectedPage)
    {
        // Check if error element is visible first
        string? errorMess = null;
        var isErrorVisible = await IsElementVisibleAsync(SomethingWentWrongMessage);

        if (isErrorVisible)
        {
            // Only get text if element is visible
            errorMess = await GetTextAsync(SomethingWentWrongMessage);
        }

        // Fail test if error message exists and is not empty
        if (!string.IsNullOrEmpty(errorMess) && errorMess.Length > 0)
        {
            Logger.Error($"❌ Error message detected: {errorMess}");
            throw new Exception($"Page verification failed: {errorMess}");
        }

        var currentPage = await GetCurrentPageNumberAsync();

        Logger.Info($"Expected page: {expectedPage}");
        Logger.Info($"Current page: {currentPage}");

        if (currentPage == expectedPage)
        {
            Logger.Info($"✓ Current page matches expected page {expectedPage}");
        }
        else
        {
            var errorMessage = $"Page mismatch: Expected page {expectedPage}, but current page is {currentPage}";
            Logger.Error(errorMessage);
            throw new Exception(errorMessage);
        }
    }

    /// <summary>
    /// Get all page numbers from pagination buttons and validate them
    /// </summary>
    /// <returns>List of page numbers as strings</returns>
    public async Task<List<string>> ValidateThePageSelectorNumberDisplayed()
    {
        await WaitForLoadingToCompleteAsync();

        var pageNumbers = new List<string>();
        var pageButtonLocators = Page.Locator(PageNumberButtonSelector);
        int count = await pageButtonLocators.CountAsync();

        Logger.Info($"Found {count} page number buttons");

        for (int i = 0; i < count; i++)
        {
            var text = await pageButtonLocators.Nth(i).TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var cleanText = text.Trim();
                pageNumbers.Add(cleanText);
                Logger.Info($"  Page button {i + 1}: {cleanText}");
            }
        }

        // Print summary
        Logger.Info($"📄 Pagination Summary:");
        Logger.Info($"   Total page buttons: {pageNumbers.Count}");
        Logger.Info($"   Page numbers: {string.Join(", ", pageNumbers)}");

        return pageNumbers;
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
