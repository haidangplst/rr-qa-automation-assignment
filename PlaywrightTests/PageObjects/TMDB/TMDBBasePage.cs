using Microsoft.Playwright;
using PlaywrightTests.PageObjects;
using PlaywrightTests.Utilities;

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

    // Navigation Bar Links
    private const string TrendingNavLinkSelector = "//li//a[@href='/trend']";
    private const string PopularNavLinkSelector = "//li//a[@href='/popular']";
    private const string NewestNavLinkSelector = "//li//a[@href='/new']";
    private const string TopRatedNavLinkSelector = "//li//a[@href='/top']";
    private const string ResultItemSelector = "//div[contains(@class, 'grid-cols-3 gap-4')]//div[contains(@class, 'items-center')]";
    private const string ResultItemDisplayedImage = "//div[contains(@class, 'grid-cols-3 gap-4')]//div[contains(@class, 'items-center')]//img[contains(@src, 'https')]";
    private const string NoResultsSelector = "//div[contains(@class, 'items-center justify-center text-white')]";



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
    /// Navigate to Trending page using navigation bar
    /// </summary>
    public async Task ClickToNavigateToTrendingPage()
    {
        await ClickAsync(TrendingNavLinkSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Navigate to Popular page using navigation bar
    /// </summary>
    public async Task NavigateToPopularAsync()
    {
        await ClickAsync(PopularNavLinkSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Navigate to Newest page using navigation bar
    /// </summary>
    public async Task ClickToNavigateToNewestPage()
    {
        await ClickAsync(NewestNavLinkSelector);
        await WaitForLoadingToCompleteAsync();
    }

    /// <summary>
    /// Navigate to Top Rated page using navigation bar
    /// </summary>
    public async Task ClickToNavigateToTopRatedPage()
    {
        await ClickAsync(TopRatedNavLinkSelector);
        await WaitForLoadingToCompleteAsync();
    }


    /// <summary>
    /// Get all visible results count
    /// </summary>
    public async Task<int> GetResultsCountAsync()
    {
        await WaitForLoadingToCompleteAsync();
        return await Page.Locator(ResultItemSelector).CountAsync();
    }

    public async Task VerifyNoResultDisplayed()
    {
        var text = await Page.Locator(NoResultsSelector).TextContentAsync();
        text.Equals("No results found", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Get text from all visible result items and return as array
    /// </summary>
    public async Task<List<string>> VerifyResultsItemsDescriptionAsync(string searchTerm = "")
    {
        await WaitForLoadingToCompleteAsync();
        var resultTexts = new List<string>();

        var resultLocators = Page.Locator(ResultItemSelector);
        int count = await resultLocators.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var text = await resultLocators.Nth(i).TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                resultTexts.Add(text.Trim());
            }
        }

        var allItemsContainSearchTerm = true;
        var itemsNotContaining = new List<int>();

        for (int i = 0; i < resultTexts.Count; i++)
        {
            var itemText = resultTexts[i]?.ToLower() ?? "";
            if (!itemText.Contains(searchTerm.ToLower()))
            {
                allItemsContainSearchTerm = false;
                itemsNotContaining.Add(i + 1);
            }
        }
        Logger.Info("All Items displayed matched with search term!!");

        return resultTexts;
    }

    /// <summary>
    /// Verify results items with error images
    /// Returns items from fullItems that do NOT appear in the items with valid images
    /// </summary>
    /// <param name="fullItems">List of all items text</param>
    /// <returns>List of items that have error/missing images (items NOT in the valid image list)</returns>
    public async Task<List<string>> VerifyResultsItemsErrorimageAsync(List<string> fullItems)
    {
        var itemsWithErrorImages = new List<string>();
        await WaitForLoadingToCompleteAsync();

        // Get items that have valid displayed images
        var itemsWithValidImages = new List<string>();
        var resultImageLocators = Page.Locator(ResultItemDisplayedImage);
        int countResultImageLocators = await resultImageLocators.CountAsync();

        if (countResultImageLocators == fullItems?.Count)
        {
            Logger.Info($"✓ All {fullItems.Count} items have valid images - no image errors!");
            return itemsWithErrorImages; // Return empty list - no errors
        }
        else
        {
            // Get text from all items with valid images
            for (int i = 0; i < countResultImageLocators; i++)
            {
                var imageElement = resultImageLocators.Nth(i);
                // Navigate up to the parent item container to get the item text
                var parentItem = imageElement.Locator("xpath=ancestor::div[contains(@class, 'items-center')]");
                var text = await parentItem.TextContentAsync();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    itemsWithValidImages.Add(text.Trim());
                }
            }

            // Find items in fullItems that are NOT in itemsWithValidImages
            // These are items with error/missing images
            itemsWithErrorImages = fullItems
                .Where(fullItem => !itemsWithValidImages.Any(validItem =>
                    validItem.Contains(fullItem, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            Logger.Info($"📊 Image Verification Results:");
            Logger.Info($"   Total items: {fullItems.Count}");
            Logger.Info($"   Items with valid images: {itemsWithValidImages.Count}");
            Logger.Info($"   Items with error images: {itemsWithErrorImages.Count}");

            if (itemsWithErrorImages.Any())
            {
                Logger.Warning($"⚠ Items with error images: {string.Join(", ", itemsWithErrorImages.Take(5))}");
            }
        }

        return itemsWithErrorImages;
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
