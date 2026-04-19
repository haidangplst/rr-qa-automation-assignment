# Browser API Testing Documentation

## Overview

This document details the implementation and usage of **Browser API testing** in the TMDB Discover test automation framework. Browser APIs allow us to interact with and assert browser-level functionality beyond standard UI interactions.

---

## Table of Contents

1. [What are Browser APIs?](#what-are-browser-apis)
2. [Implementation Architecture](#implementation-architecture)
3. [Browser APIs Covered](#browser-apis-covered)
4. [How to Use Browser APIs](#how-to-use-browser-apis)
5. [Test Cases with Browser API Assertions](#test-cases-with-browser-api-assertions)
6. [Best Practices](#best-practices)
7. [Examples](#examples)

---

## 1. What are Browser APIs?

Browser APIs are JavaScript interfaces provided by web browsers that allow interaction with:
- **Storage**: localStorage, sessionStorage, cookies
- **Navigator**: User agent, language, online status, geolocation
- **Window**: Size, scroll position, location
- **Document**: Title, URL, DOM elements, ready state
- **Performance**: Page load timing, network timing
- **Custom JavaScript**: Execute any JavaScript in browser context

### Why Test Browser APIs?

✅ **State Management**: Verify application correctly uses localStorage/sessionStorage
✅ **Browser Compatibility**: Ensure features work across different browsers
✅ **Performance**: Validate page load times and rendering performance
✅ **User Experience**: Check responsive behavior, scroll interactions
✅ **Security**: Verify cookies, session management
✅ **Debugging**: Extract runtime information from browser console

---

## 2. Implementation Architecture

### File Structure

```
PlaywrightTests/
├── Utilities/
│   └── BrowserAPIHelper.cs         # Browser API wrapper with assertions
├── Tests/
│   └── TMDB/
│       └── BrowserAPITests.cs      # Comprehensive Browser API test suite
```

### Class Diagram

```
┌─────────────────────────────────┐
│   BrowserAPIHelper              │
├─────────────────────────────────┤
│ - IPage _page                   │
├─────────────────────────────────┤
│ + LocalStorage APIs             │
│ + SessionStorage APIs           │
│ + Navigator APIs                │
│ + Window APIs                   │
│ + Document APIs                 │
│ + Cookie APIs                   │
│ + Performance APIs              │
│ + Custom JavaScript APIs        │
└─────────────────────────────────┘
		   ↑
		   │ uses
		   │
┌─────────────────────────────────┐
│   BrowserAPITests               │
├─────────────────────────────────┤
│ - BrowserAPIHelper _browserAPI  │
├─────────────────────────────────┤
│ + TC_API_001: LocalStorage      │
│ + TC_API_002: SessionStorage    │
│ + TC_API_003: Navigator         │
│ + TC_API_004: Window            │
│ + TC_API_005: Document          │
│ + TC_API_006: Cookies           │
│ + TC_API_007: Performance       │
│ + TC_API_008: Custom JS         │
│ + TC_API_009: Integration       │
└─────────────────────────────────┘
```

---

## 3. Browser APIs Covered

### 3.1 LocalStorage API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `SetLocalStorageItemAsync()` | Store key-value pair | - |
| `GetLocalStorageItemAsync()` | Retrieve value by key | - |
| `RemoveLocalStorageItemAsync()` | Delete specific key | - |
| `ClearLocalStorageAsync()` | Clear all localStorage | - |
| `GetAllLocalStorageKeysAsync()` | List all keys | - |
| `AssertLocalStorageContainsKeyAsync()` | Verify key exists | ✅ Throws if key missing |
| `AssertLocalStorageValueAsync()` | Verify key has expected value | ✅ Throws if value mismatch |

**Example Usage**:
```csharp
await browserAPI.SetLocalStorageItemAsync("theme", "dark");
await browserAPI.AssertLocalStorageValueAsync("theme", "dark", "Theme preference");
```

---

### 3.2 SessionStorage API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `SetSessionStorageItemAsync()` | Store session-specific data | - |
| `GetSessionStorageItemAsync()` | Retrieve session value | - |
| `ClearSessionStorageAsync()` | Clear all sessionStorage | - |

**Example Usage**:
```csharp
await browserAPI.SetSessionStorageItemAsync("searchQuery", "Inception");
var query = await browserAPI.GetSessionStorageItemAsync("searchQuery");
Assert.AreEqual("Inception", query);
```

---

### 3.3 Navigator API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `GetUserAgentAsync()` | Get browser user agent string | - |
| `GetBrowserLanguageAsync()` | Get browser language | - |
| `AreCookiesEnabledAsync()` | Check if cookies enabled | - |
| `IsOnlineAsync()` | Check network connectivity | - |
| `AssertUserAgentContainsAsync()` | Verify user agent substring | ✅ Throws if not found |

**Example Usage**:
```csharp
var userAgent = await browserAPI.GetUserAgentAsync();
await browserAPI.AssertUserAgentContainsAsync("Chrome", "Verify Chrome browser");

var isOnline = await browserAPI.IsOnlineAsync();
Assert.IsTrue(isOnline, "Browser should be online");
```

---

### 3.4 Window API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `GetWindowSizeAsync()` | Get viewport dimensions | - |
| `GetScrollPositionAsync()` | Get current scroll position | - |
| `ScrollToAsync()` | Scroll to specific position | - |
| `ScrollToBottomAsync()` | Scroll to page bottom | - |
| `AssertWindowSizeAsync()` | Verify viewport size | ✅ Throws if size mismatch |

**Example Usage**:
```csharp
var (width, height) = await browserAPI.GetWindowSizeAsync();
await browserAPI.AssertWindowSizeAsync(1920, 1080, "Desktop viewport");

await browserAPI.ScrollToBottomAsync();
var (x, y) = await browserAPI.GetScrollPositionAsync();
Assert.Greater(y, 0, "Should scroll down");
```

---

### 3.5 Document API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `GetDocumentTitleAsync()` | Get page title | - |
| `GetDocumentReadyStateAsync()` | Get document load state | - |
| `GetDocumentURLAsync()` | Get current URL | - |
| `GetAllLinksAsync()` | Get all `<a>` href values | - |
| `AssertDocumentReadyAsync()` | Verify page fully loaded | ✅ Throws if not "complete" |
| `AssertDocumentTitleContainsAsync()` | Verify title substring | ✅ Throws if not found |

**Example Usage**:
```csharp
await browserAPI.AssertDocumentReadyAsync("Page should be loaded");

var title = await browserAPI.GetDocumentTitleAsync();
await browserAPI.AssertDocumentTitleContainsAsync("TMDB", "Verify TMDB in title");

var links = await browserAPI.GetAllLinksAsync();
Logger.Info($"Found {links.Count} links on page");
```

---

### 3.6 Cookie API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `GetAllCookiesAsync()` | Get all cookies | - |
| `GetCookieAsync()` | Get specific cookie by name | - |
| `SetCookieAsync()` | Create/update cookie | - |
| `ClearAllCookiesAsync()` | Delete all cookies | - |
| `AssertCookieExistsAsync()` | Verify cookie present | ✅ Throws if missing |
| `AssertCookieValueAsync()` | Verify cookie value | ✅ Throws if mismatch |

**Example Usage**:
```csharp
await browserAPI.SetCookieAsync("sessionId", "abc123");
await browserAPI.AssertCookieExistsAsync("sessionId", "Session cookie");
await browserAPI.AssertCookieValueAsync("sessionId", "abc123", "Verify session ID");

var cookies = await browserAPI.GetAllCookiesAsync();
Logger.Info($"Found {cookies.Count} cookies");
```

---

### 3.7 Performance API

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `GetPerformanceTimingAsync()` | Get page load timing metrics | - |
| `AssertPageLoadTimeAsync()` | Verify load time < threshold | ✅ Throws if exceeds limit |

**Example Usage**:
```csharp
var timing = await browserAPI.GetPerformanceTimingAsync();
var pageLoadTime = timing["loadEventEnd"] - timing["navigationStart"];
Logger.Info($"Page loaded in {pageLoadTime}ms");

await browserAPI.AssertPageLoadTimeAsync(5000, "Page load performance");
```

**Metrics Captured**:
- `navigationStart`: When navigation started
- `domContentLoadedEventEnd`: DOM ready
- `loadEventEnd`: Page fully loaded
- `domComplete`: DOM parsing complete
- `domInteractive`: DOM interactive

---

### 3.8 Custom JavaScript Execution

| Method | Purpose | Assertion |
|--------|---------|-----------|
| `ExecuteJavaScriptAsync<T>()` | Run custom JS, return typed result | - |
| `ExecuteJavaScriptAsync()` | Run custom JS, no return | - |
| `AssertJavaScriptExpressionAsync()` | Verify JS expression = true | ✅ Throws if false |

**Example Usage**:
```csharp
// Get page height
var height = await browserAPI.ExecuteJavaScriptAsync<int>(
	"() => { return document.body.scrollHeight; }",
	"Get page height"
);

// Assert DOM elements exist
await browserAPI.AssertJavaScriptExpressionAsync(
	"document.querySelectorAll('img').length > 0",
	"Verify images present"
);

// Execute custom logic
await browserAPI.ExecuteJavaScriptAsync(@"
	() => {
		console.log('Test executed from Playwright');
		document.body.style.backgroundColor = 'yellow';
	}
", "Change background color");
```

---

## 4. How to Use Browser APIs

### 4.1 Basic Usage in Tests

```csharp
[Test]
public async Task MyTest()
{
	// Initialize BrowserAPIHelper
	var browserAPI = new BrowserAPIHelper(Page);

	// Use localStorage
	await browserAPI.SetLocalStorageItemAsync("key", "value");
	await browserAPI.AssertLocalStorageValueAsync("key", "value");

	// Check navigator
	var isOnline = await browserAPI.IsOnlineAsync();
	Assert.IsTrue(isOnline);

	// Verify document
	await browserAPI.AssertDocumentReadyAsync();
	await browserAPI.AssertDocumentTitleContainsAsync("Expected Title");
}
```

### 4.2 Integration with Page Objects

```csharp
public class TMDBHomePage : TMDBBasePage
{
	private readonly BrowserAPIHelper _browserAPI;

	public TMDBHomePage(IPage page) : base(page)
	{
		_browserAPI = new BrowserAPIHelper(page);
	}

	public async Task SaveSearchPreferenceAsync(string preference)
	{
		await _browserAPI.SetLocalStorageItemAsync("searchPref", preference);
		Logger.Info($"Saved search preference: {preference}");
	}

	public async Task VerifySearchPreferenceAsync(string expected)
	{
		await _browserAPI.AssertLocalStorageValueAsync("searchPref", expected, 
			"Verify saved search preference");
	}
}
```

---

## 5. Test Cases with Browser API Assertions

### Test Suite: BrowserAPITests.cs

| Test ID | Test Name | Category | Browser APIs Used |
|---------|-----------|----------|-------------------|
| TC_API_001 | Validate_LocalStorage_SetAndGet | BrowserAPI, Smoke | localStorage |
| TC_API_002 | Validate_SessionStorage_Operations | BrowserAPI, Smoke | sessionStorage |
| TC_API_003 | Validate_Navigator_Properties | BrowserAPI, Smoke | navigator |
| TC_API_004 | Validate_Window_APIs | BrowserAPI | window |
| TC_API_005 | Validate_Document_APIs | BrowserAPI, Smoke | document |
| TC_API_006 | Validate_Cookie_Operations | BrowserAPI | cookies |
| TC_API_007 | Validate_Page_Performance | BrowserAPI, Performance | performance.timing |
| TC_API_008 | Validate_Custom_JavaScript_Execution | BrowserAPI | Page.Evaluate |
| TC_API_009 | Validate_LocalStorage_Persistence | BrowserAPI, Integration | localStorage + navigation |

### Running Browser API Tests

```bash
# Run all Browser API tests
dotnet test --filter "Category=BrowserAPI"

# Run smoke Browser API tests
dotnet test --filter "Category=BrowserAPI&Category=Smoke"

# Run performance tests
dotnet test --filter "Category=Performance"

# Run specific test
dotnet test --filter "Name=TC_API_001_Validate_LocalStorage_SetAndGet"
```

---

## 6. Best Practices

### ✅ DO

1. **Use Assertions for Validation**
   ```csharp
   // ✅ Good: Use assertion method
   await browserAPI.AssertLocalStorageValueAsync("key", "value");

   // ❌ Bad: Manual check without clear error
   var value = await browserAPI.GetLocalStorageItemAsync("key");
   if (value != "expected") { /* unclear error */ }
   ```

2. **Clean Up After Tests**
   ```csharp
   try
   {
	   await browserAPI.SetLocalStorageItemAsync("test", "data");
	   // ... test logic
   }
   finally
   {
	   await browserAPI.ClearLocalStorageAsync(); // Clean up
   }
   ```

3. **Use Descriptive Context Messages**
   ```csharp
   await browserAPI.AssertCookieExistsAsync("sessionId", 
	   "Verify user session cookie exists after login");
   ```

4. **Log Browser API Calls**
   ```csharp
   // BrowserAPIHelper already logs automatically
   await browserAPI.GetUserAgentAsync(); 
   // Logs: "User Agent: Mozilla/5.0..."
   ```

5. **Combine with UI Interactions**
   ```csharp
   await homePage.SearchByTitleAsync("Inception");
   await browserAPI.SetLocalStorageItemAsync("lastSearch", "Inception");
   await homePage.ReloadPage();
   await browserAPI.AssertLocalStorageValueAsync("lastSearch", "Inception");
   ```

### ❌ DON'T

1. **Don't Skip Error Context**
   ```csharp
   // ❌ Bad: No context
   await browserAPI.AssertCookieExistsAsync("auth");

   // ✅ Good: Clear context
   await browserAPI.AssertCookieExistsAsync("auth", "After user login");
   ```

2. **Don't Hardcode Timeouts in JavaScript**
   ```csharp
   // ❌ Bad: setTimeout in JS
   await Page.EvaluateAsync("() => { setTimeout(..., 5000); }");

   // ✅ Good: Use Playwright's built-in waits
   await Page.WaitForTimeoutAsync(5000);
   ```

3. **Don't Forget to Handle Null Returns**
   ```csharp
   // ❌ Bad: Null reference risk
   var value = await browserAPI.GetLocalStorageItemAsync("key");
   var upper = value.ToUpper(); // May throw if null

   // ✅ Good: Null check
   var value = await browserAPI.GetLocalStorageItemAsync("key");
   var upper = value?.ToUpper() ?? "DEFAULT";
   ```

---

## 7. Examples

### Example 1: User Preference Persistence

```csharp
[Test]
public async Task TC_UserPreference_Theme_Persists()
{
	var browserAPI = new BrowserAPIHelper(Page);
	var homePage = new TMDBHomePage(Page);

	// User sets dark theme
	await homePage.SelectTheme("dark");

	// Verify theme saved to localStorage
	await browserAPI.AssertLocalStorageValueAsync("theme", "dark", 
		"Theme preference");

	// Reload page
	await Page.ReloadAsync();

	// Verify theme persisted
	await browserAPI.AssertLocalStorageValueAsync("theme", "dark", 
		"After page reload");

	// Verify UI reflects dark theme
	var bgColor = await homePage.GetBackgroundColor();
	Assert.That(bgColor, Does.Contain("dark"));
}
```

### Example 2: Session Management

```csharp
[Test]
public async Task TC_Session_Cookie_ExpiresAfterLogout()
{
	var browserAPI = new BrowserAPIHelper(Page);
	var loginPage = new LoginPage(Page);

	// Login
	await loginPage.LoginAsync("user", "pass");

	// Verify session cookie created
	await browserAPI.AssertCookieExistsAsync("sessionId", "After login");
	var cookie = await browserAPI.GetCookieAsync("sessionId");
	Logger.Info($"Session ID: {cookie.Value}");

	// Logout
	await loginPage.LogoutAsync();

	// Verify session cookie cleared
	var clearedCookie = await browserAPI.GetCookieAsync("sessionId");
	Assert.IsNull(clearedCookie, "Session cookie should be cleared after logout");
}
```

### Example 3: Performance Monitoring

```csharp
[Test]
public async Task TC_Performance_PageLoadUnder5Seconds()
{
	var browserAPI = new BrowserAPIHelper(Page);

	// Navigate to page (timed by Performance API)
	await Page.GotoAsync("https://tmdb-discover.surge.sh/");

	// Assert page load time
	await browserAPI.AssertPageLoadTimeAsync(5000, 
		"Home page should load within 5 seconds");

	// Get detailed timing
	var timing = await browserAPI.GetPerformanceTimingAsync();
	var domContentLoaded = timing["domContentLoadedEventEnd"] - timing["navigationStart"];
	var fullyLoaded = timing["loadEventEnd"] - timing["navigationStart"];

	Logger.Info($"DOM Content Loaded: {domContentLoaded}ms");
	Logger.Info($"Fully Loaded: {fullyLoaded}ms");

	// Performance assertions
	Assert.Less(domContentLoaded, 3000, "DOM should load within 3 seconds");
	Assert.Less(fullyLoaded, 5000, "Page should fully load within 5 seconds");
}
```

### Example 4: Responsive Design Validation

```csharp
[Test]
public async Task TC_Responsive_MobileViewport()
{
	var browserAPI = new BrowserAPIHelper(Page);

	// Set mobile viewport
	await Page.SetViewportSizeAsync(375, 667); // iPhone SE size
	await Page.ReloadAsync();

	// Verify window size
	await browserAPI.AssertWindowSizeAsync(375, 667, "Mobile viewport");

	// Verify responsive behavior
	var isMobileMenuVisible = await Page.Locator(".mobile-menu").IsVisibleAsync();
	Assert.IsTrue(isMobileMenuVisible, "Mobile menu should be visible");

	// Verify desktop menu hidden
	var isDesktopMenuVisible = await Page.Locator(".desktop-menu").IsVisibleAsync();
	Assert.IsFalse(isDesktopMenuVisible, "Desktop menu should be hidden on mobile");
}
```

### Example 5: Scroll Behavior Testing

```csharp
[Test]
public async Task TC_InfiniteScroll_LoadsMoreItems()
{
	var browserAPI = new BrowserAPIHelper(Page);
	var homePage = new TMDBHomePage(Page);

	// Get initial item count
	var initialCount = await homePage.GetResultsCountAsync();
	Logger.Info($"Initial items: {initialCount}");

	// Scroll to bottom
	await browserAPI.ScrollToBottomAsync();
	await Task.Delay(2000); // Wait for lazy load

	// Verify scroll position
	var (x, y) = await browserAPI.GetScrollPositionAsync();
	Assert.Greater(y, 500, "Should have scrolled down");

	// Verify more items loaded
	var newCount = await homePage.GetResultsCountAsync();
	Logger.Info($"Items after scroll: {newCount}");
	Assert.Greater(newCount, initialCount, "More items should load after scrolling");
}
```

---

## 8. Assertions Summary

### Available Assertion Methods

| Method | Throws Exception When |
|--------|----------------------|
| `AssertLocalStorageContainsKeyAsync()` | Key does not exist in localStorage |
| `AssertLocalStorageValueAsync()` | Value does not match expected |
| `AssertUserAgentContainsAsync()` | User agent does not contain text |
| `AssertWindowSizeAsync()` | Window size does not match |
| `AssertDocumentReadyAsync()` | Ready state is not "complete" |
| `AssertDocumentTitleContainsAsync()` | Title does not contain text |
| `AssertCookieExistsAsync()` | Cookie does not exist |
| `AssertCookieValueAsync()` | Cookie value does not match |
| `AssertPageLoadTimeAsync()` | Page load time exceeds threshold |
| `AssertJavaScriptExpressionAsync()` | JavaScript expression evaluates to false |

### Assertion Error Format

All assertions throw exceptions with context:
```
{API} assertion failed ({context}): {detailed message}
```

Example:
```
Cookie assertion failed (After login): Cookie 'sessionId' not found
```

---

## 9. Integration with Existing Framework

### Seamless Integration

```csharp
public class TMDBHomePage : TMDBBasePage
{
	private readonly BrowserAPIHelper _browserAPI;

	public TMDBHomePage(IPage page) : base(page)
	{
		_browserAPI = new BrowserAPIHelper(page);
	}

	// Combine UI + Browser API
	public async Task SearchWithPreferenceSaveAsync(string query)
	{
		// UI interaction
		await SearchByTitleAsync(query);

		// Browser API
		await _browserAPI.SetLocalStorageItemAsync("lastSearch", query);

		// Assert both UI and storage
		var results = await GetResultsCountAsync();
		await _browserAPI.AssertLocalStorageValueAsync("lastSearch", query);

		Logger.Info($"Search '{query}' returned {results} results and saved preference");
	}
}
```

---

## 10. Conclusion

The **BrowserAPIHelper** provides a comprehensive, assertion-rich interface for testing browser-level functionality in Playwright. By combining UI automation with Browser API testing, we achieve:

✅ **Complete Coverage**: Test both user interface AND browser state
✅ **Strong Assertions**: Clear pass/fail criteria with detailed error messages
✅ **Maintainability**: Reusable methods across all tests
✅ **Debugging**: Automatic logging of all Browser API calls
✅ **Flexibility**: Execute any custom JavaScript when needed

**Key Benefits**:
- Verify application correctly uses browser storage
- Validate performance characteristics
- Test responsive behavior across viewports
- Ensure session management works correctly
- Debug runtime issues with browser state inspection

**Next Steps**:
1. Run the Browser API test suite: `dotnet test --filter "Category=BrowserAPI"`
2. Review HTML reports for detailed execution logs
3. Integrate Browser API assertions into existing tests
4. Expand coverage for application-specific browser interactions

---

**Document Version**: 1.0
**Last Updated**: January 2025
**Related Files**:
- `PlaywrightTests/Utilities/BrowserAPIHelper.cs`
- `PlaywrightTests/Tests/TMDB/BrowserAPITests.cs`
