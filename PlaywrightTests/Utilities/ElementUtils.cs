using Microsoft.Playwright;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Utility class for common element operations and assertions
/// </summary>
public static class ElementUtils
{
    /// <summary>
    /// Scroll element into view
    /// </summary>
    public static async Task ScrollIntoViewAsync(ILocator locator)
    {
        await locator.ScrollIntoViewIfNeededAsync();
    }

    /// <summary>
    /// Get element attribute value
    /// </summary>
    public static async Task<string?> GetAttributeAsync(ILocator locator, string attributeName)
    {
        return await locator.GetAttributeAsync(attributeName);
    }

    /// <summary>
    /// Check if element has specific class
    /// </summary>
    public static async Task<bool> HasClassAsync(ILocator locator, string className)
    {
        var classes = await locator.GetAttributeAsync("class");
        return classes?.Contains(className) ?? false;
    }

    /// <summary>
    /// Get number of matching elements
    /// </summary>
    public static async Task<int> GetElementCountAsync(IPage page, string selector)
    {
        return await page.Locator(selector).CountAsync();
    }

    /// <summary>
    /// Wait for multiple elements to be visible
    /// </summary>
    public static async Task WaitForElementsAsync(IPage page, string selector, int count)
    {
        var locator = page.Locator(selector);
        await locator.First.WaitForAsync();

        // Wait until we have the expected count
        int retries = 0;
        while (await locator.CountAsync() < count && retries < 10)
        {
            await Task.Delay(500);
            retries++;
        }
    }

    /// <summary>
    /// Double-click an element
    /// </summary>
    public static async Task DoubleClickAsync(ILocator locator)
    {
        await locator.ClickAsync(new LocatorClickOptions { ClickCount = 2 });
    }

    /// <summary>
    /// Right-click (context menu) an element
    /// </summary>
    public static async Task RightClickAsync(ILocator locator)
    {
        await locator.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
    }

    /// <summary>
    /// Hover over an element
    /// </summary>
    public static async Task HoverAsync(ILocator locator)
    {
        await locator.HoverAsync();
    }
}
