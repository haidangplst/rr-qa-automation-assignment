using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public abstract class BasePage
{
    protected IPage Page { get; }

    protected BasePage(IPage page)
    {
        Page = page;
    }

    /// <summary>
    /// Navigate to a specific URL
    /// </summary>
    public async Task NavigateToAsync(string url)
    {
        await Page.GotoAsync(url);
    }

    /// <summary>
    /// Click on an element by locator
    /// </summary>
    public async Task ClickAsync(string selector)
    {
        await Page.Locator(selector).ClickAsync();
    }

    /// <summary>
    /// Fill text in an input field
    /// </summary>
    public async Task FillAsync(string selector, string text)
    {
        await Page.Locator(selector).FillAsync(text);
    }

    /// <summary>
    /// Get text content of an element
    /// </summary>
    public async Task<string?> GetTextAsync(string selector)
    {
        return await Page.Locator(selector).TextContentAsync();
    }

    /// <summary>
    /// Wait for an element to be visible
    /// </summary>
    public async Task WaitForElementAsync(string selector)
    {
        await Page.Locator(selector).WaitForAsync();
    }

    /// <summary>
    /// Check if element is visible
    /// </summary>
    public async Task<bool> IsElementVisibleAsync(string selector)
    {
        try
        {
            await Page.Locator(selector).WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get page title
    /// </summary>
    public async Task<string> GetTitleAsync()
    {
        return await Page.TitleAsync();
    }

    /// <summary>
    /// Get current URL
    /// </summary>
    public string GetCurrentUrl()
    {
        return Page.Url;
    }

    /// <summary>
    /// Wait for navigation to complete
    /// </summary>
    public async Task WaitForNavigationAsync(Func<Task> action)
    {
        await Task.WhenAll(
            Page.WaitForNavigationAsync(),
            action()
        );
    }
}
