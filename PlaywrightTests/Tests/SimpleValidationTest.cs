using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SimpleValidationTest : PageTest
{
    [Test]
    [Category("Smoke")]
    public async Task TC_Simple_PageLoadsSuccessfully()
    {
        Logger.TestStart("Simple Test: Page Load Validation");

        try
        {
            Logger.Step("1", "Navigate to Example domain");
            await Page.GotoAsync("https://example.com");

            Logger.Step("2", "Wait for page to load");
            await Page.WaitForLoadStateAsync();

            Logger.Step("3", "Verify page title");
            var title = await Page.TitleAsync();
            Logger.Info($"Page title: {title}");

            Logger.Assert(!string.IsNullOrEmpty(title), "Page has a title");
            Logger.Assert(title.Contains("Example"), "Title contains 'Example'");

            Logger.Step("4", "Verify main heading is visible");
            var heading = await Page.Locator("h1").TextContentAsync();
            Logger.Info($"Main heading: {heading}");

            Logger.Assert(heading?.Contains("Example") ?? false, "Heading contains 'Example'");

            Logger.Step("5", "Keep browser open for 3 seconds so you can see it");
            await Task.Delay(3000); // Wait 3 seconds so you can see the browser

            Logger.TestEnd("Simple Test", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("Simple Test", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    public async Task TC_Simple_NavigationWorks()
    {
        Logger.TestStart("Simple Test: Navigation");

        try
        {
            Logger.Step("1", "Navigate to example.com");
            await Page.GotoAsync("https://example.com");

            Logger.Step("2", "Verify URL");
            var url = Page.Url;
            Logger.Info($"Current URL: {url}");

            Logger.Assert(url.Contains("example.com"), "URL is correct");

            Logger.TestEnd("Simple Test", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("Simple Test", false, ex.Message);
            throw;
        }
    }
}
