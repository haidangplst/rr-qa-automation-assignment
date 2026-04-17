using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TopRatedPagesTests : PlaywrightPageTest
{
    [Test]
    [Category("CategoryPages")]
    [Category("Smoke")]
    public async Task TC_004_FilterByTopRatedCategory()
    {
        Logger.TestStart("TC-004: Filter by Top Rated Category");

        try
        {
            var topRatedPage = new TopRatedPage(Page);

            Logger.Step("1", "Navigate to Top Rated page");
            await topRatedPage.NavigateToTopRatedAsync();

            Logger.Step("2", "Verify Top Rated page loaded");
            var isTopRatedPage = await topRatedPage.IsTopRatedPageAsync();
            Logger.Assert(isTopRatedPage, "Top Rated page loaded successfully");

            Logger.Step("3", "Get top rated items");
            var items = await topRatedPage.GetTopRatedItemsAsync();
            Logger.Info($"Found {items.Count} top rated items");
            Logger.Assert(items.Count > 0, "Top Rated items found");

            Logger.Step("4", "Get items with ratings");
            var itemsWithRatings = await topRatedPage.GetTopRatedItemsWithRatingsAsync();
            Logger.Info($"Retrieved {itemsWithRatings.Count} items with ratings");

            Logger.Step("5", "Verify items are sorted by rating (descending)");
            var isSorted = await topRatedPage.VerifyRatingsSortedDescendingAsync();
            Logger.Assert(isSorted, "Items sorted by rating in descending order");

            Logger.TestEnd("TC-004", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-004", false, ex.Message);
            throw;
        }
    }
}
