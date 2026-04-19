using Microsoft.Playwright;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects.TMDB;

/// <summary>
/// Browser API helper class for executing and asserting browser-level APIs
/// Includes: localStorage, sessionStorage, cookies, navigator, window, document APIs
/// </summary>
public class BrowserAPIHelper
{
    private readonly IPage _page;

    public BrowserAPIHelper(IPage page)
    {
        _page = page;
    }

    #region LocalStorage APIs

    /// <summary>
    /// Set item in localStorage
    /// </summary>
    public async Task SetLocalStorageItemAsync(string key, string value)
    {
        await _page.EvaluateAsync($@"
            () => {{
                localStorage.setItem('{key}', '{value}');
            }}
        ");
        Logger.Info($"Set localStorage: {key} = {value}");
    }

    /// <summary>
    /// Get item from localStorage
    /// </summary>
    public async Task<string?> GetLocalStorageItemAsync(string key)
    {
        var value = await _page.EvaluateAsync<string?>($@"
            () => {{
                return localStorage.getItem('{key}');
            }}
        ");
        Logger.Info($"Get localStorage: {key} = {value}");
        return value;
    }

    /// <summary>
    /// Remove item from localStorage
    /// </summary>
    public async Task RemoveLocalStorageItemAsync(string key)
    {
        await _page.EvaluateAsync($@"
            () => {{
                localStorage.removeItem('{key}');
            }}
        ");
        Logger.Info($"Removed localStorage key: {key}");
    }

    /// <summary>
    /// Clear all localStorage
    /// </summary>
    public async Task ClearLocalStorageAsync()
    {
        await _page.EvaluateAsync(@"
            () => {
                localStorage.clear();
            }
        ");
        Logger.Info("Cleared all localStorage");
    }

    /// <summary>
    /// Get all localStorage keys
    /// </summary>
    public async Task<List<string>> GetAllLocalStorageKeysAsync()
    {
        var keys = await _page.EvaluateAsync<string[]>(@"
            () => {
                return Object.keys(localStorage);
            }
        ");
        return keys?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Assert localStorage contains key
    /// </summary>
    public async Task AssertLocalStorageContainsKeyAsync(string key, string context = "")
    {
        var value = await GetLocalStorageItemAsync(key);
        if (value == null)
        {
            throw new Exception($"LocalStorage assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Key '{key}' not found");
        }
        Logger.Info($"✓ LocalStorage contains key: {key}");
    }

    /// <summary>
    /// Assert localStorage key has expected value
    /// </summary>
    public async Task AssertLocalStorageValueAsync(string key, string expectedValue, string context = "")
    {
        var actualValue = await GetLocalStorageItemAsync(key);
        if (actualValue != expectedValue)
        {
            throw new Exception($"LocalStorage assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expected '{expectedValue}' but got '{actualValue}' for key '{key}'");
        }
        Logger.Info($"✓ LocalStorage value matches: {key} = {expectedValue}");
    }

    #endregion

    #region SessionStorage APIs

    /// <summary>
    /// Set item in sessionStorage
    /// </summary>
    public async Task SetSessionStorageItemAsync(string key, string value)
    {
        await _page.EvaluateAsync($@"
            () => {{
                sessionStorage.setItem('{key}', '{value}');
            }}
        ");
        Logger.Info($"Set sessionStorage: {key} = {value}");
    }

    /// <summary>
    /// Get item from sessionStorage
    /// </summary>
    public async Task<string?> GetSessionStorageItemAsync(string key)
    {
        var value = await _page.EvaluateAsync<string?>($@"
            () => {{
                return sessionStorage.getItem('{key}');
            }}
        ");
        Logger.Info($"Get sessionStorage: {key} = {value}");
        return value;
    }

    /// <summary>
    /// Clear all sessionStorage
    /// </summary>
    public async Task ClearSessionStorageAsync()
    {
        await _page.EvaluateAsync(@"
            () => {
                sessionStorage.clear();
            }
        ");
        Logger.Info("Cleared all sessionStorage");
    }

    #endregion

    #region Navigator APIs

    /// <summary>
    /// Get user agent string
    /// </summary>
    public async Task<string> GetUserAgentAsync()
    {
        var userAgent = await _page.EvaluateAsync<string>(@"
            () => {
                return navigator.userAgent;
            }
        ");
        Logger.Info($"User Agent: {userAgent}");
        return userAgent;
    }

    /// <summary>
    /// Get browser language
    /// </summary>
    public async Task<string> GetBrowserLanguageAsync()
    {
        var language = await _page.EvaluateAsync<string>(@"
            () => {
                return navigator.language;
            }
        ");
        Logger.Info($"Browser Language: {language}");
        return language;
    }

    /// <summary>
    /// Check if cookies are enabled
    /// </summary>
    public async Task<bool> AreCookiesEnabledAsync()
    {
        var enabled = await _page.EvaluateAsync<bool>(@"
            () => {
                return navigator.cookieEnabled;
            }
        ");
        Logger.Info($"Cookies Enabled: {enabled}");
        return enabled;
    }

    /// <summary>
    /// Get online status
    /// </summary>
    public async Task<bool> IsOnlineAsync()
    {
        var isOnline = await _page.EvaluateAsync<bool>(@"
            () => {
                return navigator.onLine;
            }
        ");
        Logger.Info($"Online Status: {isOnline}");
        return isOnline;
    }

    /// <summary>
    /// Assert user agent contains expected text
    /// </summary>
    public async Task AssertUserAgentContainsAsync(string expectedText, string context = "")
    {
        var userAgent = await GetUserAgentAsync();
        if (!userAgent.Contains(expectedText, StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception($"User agent assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expected to contain '{expectedText}' but got '{userAgent}'");
        }
        Logger.Info($"✓ User agent contains: {expectedText}");
    }

    #endregion

    #region Window APIs

    /// <summary>
    /// Get window dimensions
    /// </summary>
    public async Task<(int width, int height)> GetWindowSizeAsync()
    {
        var size = await _page.EvaluateAsync<Dictionary<string, int>>(@"
            () => {
                return {
                    width: window.innerWidth,
                    height: window.innerHeight
                };
            }
        ");
        Logger.Info($"Window Size: {size["width"]}x{size["height"]}");
        return (size["width"], size["height"]);
    }

    /// <summary>
    /// Get scroll position
    /// </summary>
    public async Task<(int x, int y)> GetScrollPositionAsync()
    {
        var position = await _page.EvaluateAsync<Dictionary<string, int>>(@"
            () => {
                return {
                    x: window.scrollX || window.pageXOffset,
                    y: window.scrollY || window.pageYOffset
                };
            }
        ");
        Logger.Info($"Scroll Position: X={position["x"]}, Y={position["y"]}");
        return (position["x"], position["y"]);
    }

    /// <summary>
    /// Scroll to position
    /// </summary>
    public async Task ScrollToAsync(int x, int y)
    {
        await _page.EvaluateAsync($@"
            () => {{
                window.scrollTo({x}, {y});
            }}
        ");
        Logger.Info($"Scrolled to: X={x}, Y={y}");
    }

    /// <summary>
    /// Scroll to bottom of page
    /// </summary>
    public async Task ScrollToBottomAsync()
    {
        await _page.EvaluateAsync(@"
            () => {
                window.scrollTo(0, document.body.scrollHeight);
            }
        ");
        Logger.Info("Scrolled to bottom of page");
    }

    /// <summary>
    /// Assert window size matches expected dimensions
    /// </summary>
    public async Task AssertWindowSizeAsync(int expectedWidth, int expectedHeight, string context = "")
    {
        var (actualWidth, actualHeight) = await GetWindowSizeAsync();
        if (actualWidth != expectedWidth || actualHeight != expectedHeight)
        {
            throw new Exception($"Window size assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expected {expectedWidth}x{expectedHeight} but got {actualWidth}x{actualHeight}");
        }
        Logger.Info($"✓ Window size matches: {expectedWidth}x{expectedHeight}");
    }

    #endregion

    #region Document APIs

    /// <summary>
    /// Get document title
    /// </summary>
    public async Task<string> GetDocumentTitleAsync()
    {
        var title = await _page.EvaluateAsync<string>(@"
            () => {
                return document.title;
            }
        ");
        Logger.Info($"Document Title: {title}");
        return title;
    }

    /// <summary>
    /// Get document ready state
    /// </summary>
    public async Task<string> GetDocumentReadyStateAsync()
    {
        var readyState = await _page.EvaluateAsync<string>(@"
            () => {
                return document.readyState;
            }
        ");
        Logger.Info($"Document Ready State: {readyState}");
        return readyState;
    }

    /// <summary>
    /// Get document URL
    /// </summary>
    public async Task<string> GetDocumentURLAsync()
    {
        var url = await _page.EvaluateAsync<string>(@"
            () => {
                return document.URL;
            }
        ");
        Logger.Info($"Document URL: {url}");
        return url;
    }

    /// <summary>
    /// Get all links on page
    /// </summary>
    public async Task<List<string>> GetAllLinksAsync()
    {
        var links = await _page.EvaluateAsync<string[]>(@"
            () => {
                return Array.from(document.querySelectorAll('a[href]'))
                    .map(link => link.href);
            }
        ");
        Logger.Info($"Found {links?.Length ?? 0} links on page");
        return links?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Assert document ready state is complete
    /// </summary>
    public async Task AssertDocumentReadyAsync(string context = "")
    {
        var readyState = await GetDocumentReadyStateAsync();
        if (readyState != "complete")
        {
            throw new Exception($"Document ready state assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expected 'complete' but got '{readyState}'");
        }
        Logger.Info("✓ Document is ready (complete)");
    }

    /// <summary>
    /// Assert document title contains expected text
    /// </summary>
    public async Task AssertDocumentTitleContainsAsync(string expectedText, string context = "")
    {
        var title = await GetDocumentTitleAsync();
        if (!title.Contains(expectedText, StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception($"Document title assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expected to contain '{expectedText}' but got '{title}'");
        }
        Logger.Info($"✓ Document title contains: {expectedText}");
    }

    #endregion

    #region Cookie APIs (using Playwright's built-in)

    /// <summary>
    /// Get all cookies
    /// </summary>
    public async Task<IReadOnlyList<BrowserContextCookiesResult>> GetAllCookiesAsync()
    {
        var cookies = await _page.Context.CookiesAsync();
        Logger.Info($"Found {cookies.Count} cookies");
        return cookies;
    }

    /// <summary>
    /// Get specific cookie by name
    /// </summary>
    public async Task<BrowserContextCookiesResult?> GetCookieAsync(string name)
    {
        var cookies = await _page.Context.CookiesAsync();
        var cookie = cookies.FirstOrDefault(c => c.Name == name);
        Logger.Info($"Cookie '{name}': {(cookie != null ? "Found" : "Not found")}");
        return cookie;
    }

    /// <summary>
    /// Set cookie
    /// </summary>
    public async Task SetCookieAsync(string name, string value, string? domain = null, string? path = "/")
    {
        await _page.Context.AddCookiesAsync(new[]
        {
            new Cookie
            {
                Name = name,
                Value = value,
                Domain = domain ?? new Uri(_page.Url).Host,
                Path = path
            }
        });
        Logger.Info($"Set cookie: {name} = {value}");
    }

    /// <summary>
    /// Clear all cookies
    /// </summary>
    public async Task ClearAllCookiesAsync()
    {
        await _page.Context.ClearCookiesAsync();
        Logger.Info("Cleared all cookies");
    }

    /// <summary>
    /// Assert cookie exists
    /// </summary>
    public async Task AssertCookieExistsAsync(string name, string context = "")
    {
        var cookie = await GetCookieAsync(name);
        if (cookie == null)
        {
            throw new Exception($"Cookie assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Cookie '{name}' not found");
        }
        Logger.Info($"✓ Cookie exists: {name}");
    }

    /// <summary>
    /// Assert cookie has expected value
    /// </summary>
    public async Task AssertCookieValueAsync(string name, string expectedValue, string context = "")
    {
        var cookie = await GetCookieAsync(name);
        if (cookie == null)
        {
            throw new Exception($"Cookie assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Cookie '{name}' not found");
        }
        if (cookie.Value != expectedValue)
        {
            throw new Exception($"Cookie value assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expected '{expectedValue}' but got '{cookie.Value}' for cookie '{name}'");
        }
        Logger.Info($"✓ Cookie value matches: {name} = {expectedValue}");
    }

    #endregion

    #region Network & Performance APIs

    /// <summary>
    /// Get page load performance timing
    /// </summary>
    public async Task<Dictionary<string, double>> GetPerformanceTimingAsync()
    {
        var timing = await _page.EvaluateAsync<Dictionary<string, double>>(@"
            () => {
                const perf = performance.timing;
                return {
                    navigationStart: perf.navigationStart,
                    domContentLoadedEventEnd: perf.domContentLoadedEventEnd,
                    loadEventEnd: perf.loadEventEnd,
                    domComplete: perf.domComplete,
                    domInteractive: perf.domInteractive
                };
            }
        ");

        var pageLoadTime = timing["loadEventEnd"] - timing["navigationStart"];
        Logger.Info($"Page Load Time: {pageLoadTime}ms");

        return timing;
    }

    /// <summary>
    /// Assert page load time is within threshold
    /// </summary>
    public async Task AssertPageLoadTimeAsync(double maxMilliseconds, string context = "")
    {
        var timing = await GetPerformanceTimingAsync();
        var pageLoadTime = timing["loadEventEnd"] - timing["navigationStart"];

        if (pageLoadTime > maxMilliseconds)
        {
            Logger.Warning($"Page load time ({pageLoadTime}ms) exceeded threshold ({maxMilliseconds}ms)");
            throw new Exception($"Performance assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Page load time {pageLoadTime}ms exceeded {maxMilliseconds}ms");
        }
        Logger.Info($"✓ Page load time within threshold: {pageLoadTime}ms <= {maxMilliseconds}ms");
    }

    #endregion

    #region Custom JavaScript Execution

    /// <summary>
    /// Execute custom JavaScript and return result
    /// </summary>
    public async Task<T> ExecuteJavaScriptAsync<T>(string script, string description = "")
    {
        if (!string.IsNullOrEmpty(description))
        {
            Logger.Info($"Executing JS: {description}");
        }

        var result = await _page.EvaluateAsync<T>(script);
        Logger.Info($"JS execution result: {result}");
        return result;
    }

    /// <summary>
    /// Execute custom JavaScript without return value
    /// </summary>
    public async Task ExecuteJavaScriptAsync(string script, string description = "")
    {
        if (!string.IsNullOrEmpty(description))
        {
            Logger.Info($"Executing JS: {description}");
        }

        await _page.EvaluateAsync(script);
        Logger.Info("JS execution completed");
    }

    /// <summary>
    /// Assert JavaScript expression evaluates to true
    /// </summary>
    public async Task AssertJavaScriptExpressionAsync(string expression, string context = "")
    {
        var result = await _page.EvaluateAsync<bool>($"() => {{ return {expression}; }}");
        if (!result)
        {
            throw new Exception($"JavaScript assertion failed{(!string.IsNullOrEmpty(context) ? $" ({context})" : "")}: Expression '{expression}' evaluated to false");
        }
        Logger.Info($"✓ JavaScript expression is true: {expression}");
    }

    #endregion
}
