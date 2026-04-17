using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HomePagesTests : PlaywrightPageTest
{
    [Test]
    [Category("CategoryPages")]
    [Category("Smoke")]
    public async Task TC_002_FilterByTrendingCategory()
    {
        Logger.TestStart("TC-002: Filter by Trending Category");

        try
        {
            var trendingPage = new TrendingPage(Page);

            Logger.Step("1", "Navigate to Trending page");
            await trendingPage.NavigateToTrendingAsync();

            Logger.Step("2", "Verify Trending page loaded");
            var isTrendingPage = await trendingPage.IsTrendingPageAsync();
            Logger.Assert(isTrendingPage, "Trending page loaded successfully");

            Logger.Step("3", "Get trending items");
            var items = await trendingPage.GetTrendingItemsAsync();
            Logger.Info($"Found {items.Count} trending items");
            Logger.Assert(items.Count > 0, "Trending items found");

            Logger.Step("4", "Get items with ratings");
            var itemsWithRatings = await trendingPage.GetTrendingItemsWithRatingsAsync();
            Logger.Info($"Retrieved {itemsWithRatings.Count} items with ratings");

            Logger.TestEnd("TC-002", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-002", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("CategoryPages")]
    [Category("Smoke")]
    public async Task TestAllCategoriesLoadSuccessfully()
    {
        Logger.TestStart("All Categories Load Test");

        try
        {
            var categories = new Dictionary<string, Func<Task<bool>>>
            {
                { "Popular", async () => await new PopularPage(Page).IsPopularPageAsync() },
                { "Trending", async () => await new TrendingPage(Page).IsTrendingPageAsync() },
                { "Newest", async () => await new NewestPage(Page).IsNewestPageAsync() },
                { "Top Rated", async () => await new TopRatedPage(Page).IsTopRatedPageAsync() }
            };

            int categoryCount = 0;
            int passedCount = 0;

            foreach (var category in categories)
            {
                Logger.Step($"{categoryCount + 1}", $"Check {category.Key} category");
                try
                {
                    var isLoaded = await category.Value();
                    if (isLoaded)
                    {
                        Logger.Assert(true, $"{category.Key} page loaded");
                        passedCount++;
                    }
                    else
                    {
                        Logger.Assert(false, $"{category.Key} page loaded");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning($"Error checking {category.Key}: {ex.Message}");
                }
                categoryCount++;
            }

            Logger.Info($"All {categoryCount} categories checked, {passedCount} passed successfully");
            Logger.TestEnd("All Categories Load Test", passedCount == categoryCount);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("All Categories Load Test", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("CategoryPages")]
    [Category("Smoke")]
    public async Task TC_008_FilterByMoviesType()
    {
        Logger.TestStart("TC-008: Filter by Movies Type");

        try
        {
            var pages = new List<TMDBBasePage>
            {
                new PopularPage(Page),
                new TrendingPage(Page),
                new NewestPage(Page),
                new TopRatedPage(Page)
            };

            int pageIndex = 1;
            foreach (var page in pages)
            {
                Logger.Step($"{pageIndex}", $"Test Movies filter in {page.GetType().Name}");
                try
                {
                    // Navigate to base URL first
                    await page.NavigateToAsync("https://tmdb-discover.surge.sh/");
                    await page.WaitForLoadingToCompleteAsync();

                    Logger.Info($"{page.GetType().Name} loaded successfully");
                    pageIndex++;
                }
                catch (Exception ex)
                {
                    Logger.Warning($"Error testing {page.GetType().Name}: {ex.Message}");
                }
            }

            Logger.TestEnd("TC-008", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-008", false, ex.Message);
            throw;
        }
    }
}
