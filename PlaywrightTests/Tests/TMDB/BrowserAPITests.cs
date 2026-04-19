using PlaywrightTests.BaseTests;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BrowserAPITests : PlaywrightPageTest
{
    private BrowserAPIHelper? _browserAPI;

    [SetUp]
    public new async Task SetUp()
    {
        await base.SetUp();
        _browserAPI = new BrowserAPIHelper(Page);
    }

    [Test]
    [Category("BrowserAPI")]
    [Category("Smoke")]
    public async Task TC_API_001_Validate_LocalStorage_SetAndGet()
    {
        var testName = "TC_API_001_Validate_LocalStorage_SetAndGet";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-001: Validate LocalStorage Set and Get");

        try
        {
            TestExecutionLogger.RecordStep(1, "Set item in localStorage");
            await _browserAPI!.SetLocalStorageItemAsync("testKey", "testValue");
            TestExecutionLogger.LogStepInfo("Set localStorage: testKey = testValue");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Get item from localStorage and verify");
            var value = await _browserAPI.GetLocalStorageItemAsync("testKey");
            TestExecutionLogger.LogStepInfo($"Retrieved value: {value}");

            if (value != "testValue")
            {
                throw new Exception($"LocalStorage value mismatch: Expected 'testValue' but got '{value}'");
            }
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Assert localStorage contains key");
            await _browserAPI.AssertLocalStorageContainsKeyAsync("testKey", "Verify testKey exists");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Assert localStorage value matches");
            await _browserAPI.AssertLocalStorageValueAsync("testKey", "testValue", "Verify testKey value");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(5, "Clean up - remove localStorage item");
            await _browserAPI.RemoveLocalStorageItemAsync("testKey");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-001", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-001", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    [Category("Smoke")]
    public async Task TC_API_002_Validate_SessionStorage_Operations()
    {
        var testName = "TC_API_002_Validate_SessionStorage_Operations";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-002: Validate SessionStorage Operations");

        try
        {
            TestExecutionLogger.RecordStep(1, "Set item in sessionStorage");
            await _browserAPI!.SetSessionStorageItemAsync("sessionKey", "sessionValue");
            TestExecutionLogger.LogStepInfo("Set sessionStorage: sessionKey = sessionValue");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Get item from sessionStorage and verify");
            var value = await _browserAPI.GetSessionStorageItemAsync("sessionKey");
            TestExecutionLogger.LogStepInfo($"Retrieved value: {value}");

            if (value != "sessionValue")
            {
                throw new Exception($"SessionStorage value mismatch: Expected 'sessionValue' but got '{value}'");
            }
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Clear sessionStorage");
            await _browserAPI.ClearSessionStorageAsync();
            TestExecutionLogger.LogStepInfo("Cleared all sessionStorage");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Verify item was removed");
            var clearedValue = await _browserAPI.GetSessionStorageItemAsync("sessionKey");
            if (clearedValue != null)
            {
                throw new Exception($"SessionStorage clear failed: Expected null but got '{clearedValue}'");
            }
            TestExecutionLogger.LogStepInfo("✓ SessionStorage cleared successfully");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-002", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-002", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    [Category("Smoke")]
    public async Task TC_API_003_Validate_Navigator_Properties()
    {
        var testName = "TC_API_003_Validate_Navigator_Properties";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-003: Validate Navigator Properties");

        try
        {
            TestExecutionLogger.RecordStep(1, "Get user agent");
            var userAgent = await _browserAPI!.GetUserAgentAsync();
            TestExecutionLogger.LogStepInfo($"User Agent: {userAgent}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Assert user agent is not empty");
            if (string.IsNullOrEmpty(userAgent))
            {
                throw new Exception("User agent is empty");
            }
            TestExecutionLogger.LogStepInfo("✓ User agent is not empty");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Get browser language");
            var language = await _browserAPI.GetBrowserLanguageAsync();
            TestExecutionLogger.LogStepInfo($"Language: {language}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Check if cookies are enabled");
            var cookiesEnabled = await _browserAPI.AreCookiesEnabledAsync();
            TestExecutionLogger.LogStepInfo($"Cookies Enabled: {cookiesEnabled}");

            if (!cookiesEnabled)
            {
                Logger.Warning("Cookies are disabled");
            }
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(5, "Check online status");
            var isOnline = await _browserAPI.IsOnlineAsync();
            TestExecutionLogger.LogStepInfo($"Online: {isOnline}");

            if (!isOnline)
            {
                throw new Exception("Browser reports offline status");
            }
            TestExecutionLogger.LogStepInfo("✓ Browser is online");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-003", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-003", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    public async Task TC_API_004_Validate_Window_APIs()
    {
        var testName = "TC_API_004_Validate_Window_APIs";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-004: Validate Window APIs");

        try
        {
            TestExecutionLogger.RecordStep(1, "Get window size");
            var (width, height) = await _browserAPI!.GetWindowSizeAsync();
            TestExecutionLogger.LogStepInfo($"Window Size: {width}x{height}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Assert window size matches viewport");
            // Expected size from ContextOptions: 1920x1080
            await _browserAPI.AssertWindowSizeAsync(1920, 1080, "Default viewport size");
            TestExecutionLogger.LogStepInfo("✓ Window size matches expected 1920x1080");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Get scroll position before scrolling");
            var (x1, y1) = await _browserAPI.GetScrollPositionAsync();
            TestExecutionLogger.LogStepInfo($"Initial scroll position: X={x1}, Y={y1}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Scroll to bottom of page");
            await _browserAPI.ScrollToBottomAsync();
            await Task.Delay(500); // Wait for scroll animation
            TestExecutionLogger.LogStepInfo("Scrolled to bottom");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(5, "Verify scroll position changed");
            var (x2, y2) = await _browserAPI.GetScrollPositionAsync();
            TestExecutionLogger.LogStepInfo($"New scroll position: X={x2}, Y={y2}");

            if (y2 == y1)
            {
                Logger.Warning("Scroll position did not change (page might be short)");
            }
            else
            {
                TestExecutionLogger.LogStepInfo($"✓ Scroll Y changed from {y1} to {y2}");
            }
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(6, "Scroll back to top");
            await _browserAPI.ScrollToAsync(0, 0);
            await Task.Delay(500);
            TestExecutionLogger.LogStepInfo("Scrolled back to top");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-004", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-004", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    [Category("Smoke")]
    public async Task TC_API_005_Validate_Document_APIs()
    {
        var testName = "TC_API_005_Validate_Document_APIs";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-005: Validate Document APIs");

        try
        {
            TestExecutionLogger.RecordStep(1, "Assert document ready state is complete");
            await _browserAPI!.AssertDocumentReadyAsync("Page should be fully loaded");
            TestExecutionLogger.LogStepInfo("✓ Document ready state is complete");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Get document title");
            var title = await _browserAPI.GetDocumentTitleAsync();
            TestExecutionLogger.LogStepInfo($"Document Title: {title}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Assert document title contains 'TMDB'");
            await _browserAPI.AssertDocumentTitleContainsAsync("TMDB", "Verify TMDB in title");
            TestExecutionLogger.LogStepInfo("✓ Title contains 'TMDB'");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Get document URL");
            var url = await _browserAPI.GetDocumentURLAsync();
            TestExecutionLogger.LogStepInfo($"Document URL: {url}");

            if (!url.Contains("tmdb-discover.surge.sh"))
            {
                throw new Exception($"Unexpected URL: {url}");
            }
            TestExecutionLogger.LogStepInfo("✓ URL is correct");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(5, "Get all links on page");
            var links = await _browserAPI.GetAllLinksAsync();
            TestExecutionLogger.LogStepInfo($"Found {links.Count} links on page");

            if (links.Count == 0)
            {
                Logger.Warning("No links found on page");
            }
            else
            {
                TestExecutionLogger.LogStepInfo($"Sample links: {string.Join(", ", links.Take(5))}");
            }
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-005", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-005", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    public async Task TC_API_006_Validate_Cookie_Operations()
    {
        var testName = "TC_API_006_Validate_Cookie_Operations";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-006: Validate Cookie Operations");

        try
        {
            TestExecutionLogger.RecordStep(1, "Get all cookies");
            var initialCookies = await _browserAPI!.GetAllCookiesAsync();
            TestExecutionLogger.LogStepInfo($"Found {initialCookies.Count} cookies");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Set a test cookie");
            await _browserAPI.SetCookieAsync("testCookie", "testValue123");
            TestExecutionLogger.LogStepInfo("Set cookie: testCookie = testValue123");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Assert cookie exists");
            await _browserAPI.AssertCookieExistsAsync("testCookie", "Verify testCookie was created");
            TestExecutionLogger.LogStepInfo("✓ Cookie exists");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Assert cookie has correct value");
            await _browserAPI.AssertCookieValueAsync("testCookie", "testValue123", "Verify cookie value");
            TestExecutionLogger.LogStepInfo("✓ Cookie value is correct");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(5, "Clean up - clear all cookies");
            await _browserAPI.ClearAllCookiesAsync();
            TestExecutionLogger.LogStepInfo("Cleared all cookies");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-006", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-006", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    [Category("Performance")]
    public async Task TC_API_007_Validate_Page_Performance()
    {
        var testName = "TC_API_007_Validate_Page_Performance";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-007: Validate Page Performance");

        try
        {
            TestExecutionLogger.RecordStep(1, "Get performance timing data");
            var timing = await _browserAPI!.GetPerformanceTimingAsync();
            TestExecutionLogger.LogStepInfo($"Performance data collected: {timing.Count} metrics");

            foreach (var metric in timing)
            {
                TestExecutionLogger.LogStepInfo($"  {metric.Key}: {metric.Value}ms");
            }
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Calculate page load time");
            var pageLoadTime = timing["loadEventEnd"] - timing["navigationStart"];
            TestExecutionLogger.LogStepInfo($"Page Load Time: {pageLoadTime}ms");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Assert page load time is acceptable");
            // Page should load within 10 seconds (10000ms)
            await _browserAPI.AssertPageLoadTimeAsync(10000, "Page load performance check");
            TestExecutionLogger.LogStepInfo($"✓ Page load time ({pageLoadTime}ms) is within acceptable range");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-007", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-007", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    public async Task TC_API_008_Validate_Custom_JavaScript_Execution()
    {
        var testName = "TC_API_008_Validate_Custom_JavaScript_Execution";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-008: Validate Custom JavaScript Execution");

        try
        {
            TestExecutionLogger.RecordStep(1, "Execute custom JavaScript to get page height");
            var pageHeight = await _browserAPI!.ExecuteJavaScriptAsync<int>(
                "() => { return document.body.scrollHeight; }",
                "Get page scroll height"
            );
            TestExecutionLogger.LogStepInfo($"Page height: {pageHeight}px");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Assert page height is greater than 0");
            if (pageHeight <= 0)
            {
                throw new Exception($"Invalid page height: {pageHeight}");
            }
            TestExecutionLogger.LogStepInfo($"✓ Page height is valid: {pageHeight}px");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Execute JavaScript expression assertion");
            await _browserAPI.AssertJavaScriptExpressionAsync(
                "document.body !== null",
                "Verify document.body exists"
            );
            TestExecutionLogger.LogStepInfo("✓ document.body exists");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Assert DOM elements exist");
            await _browserAPI.AssertJavaScriptExpressionAsync(
                "document.querySelectorAll('*').length > 0",
                "Verify page has DOM elements"
            );
            TestExecutionLogger.LogStepInfo("✓ Page has DOM elements");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-008", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-008", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("BrowserAPI")]
    [Category("Integration")]
    public async Task TC_API_009_Validate_LocalStorage_Persistence_Across_Navigation()
    {
        var testName = "TC_API_009_Validate_LocalStorage_Persistence";
        TestExecutionLogger.StartTest(testName, "BrowserAPI");
        Logger.TestStart("TC-API-009: Validate LocalStorage Persistence Across Navigation");

        try
        {
            var homePage = new TMDBHomePage(Page);

            TestExecutionLogger.RecordStep(1, "Set localStorage on home page");
            await _browserAPI!.SetLocalStorageItemAsync("persistentKey", "persistentValue");
            TestExecutionLogger.LogStepInfo("Set localStorage: persistentKey = persistentValue");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Navigate to Popular page");
            var popularPage = new PopularPage(Page);
            await popularPage.NavigateToPopularAsync();
            TestExecutionLogger.LogStepInfo("Navigated to Popular page");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Verify localStorage persisted after navigation");
            await _browserAPI.AssertLocalStorageValueAsync("persistentKey", "persistentValue", "After navigation");
            TestExecutionLogger.LogStepInfo("✓ LocalStorage persisted across navigation");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Clean up localStorage");
            await _browserAPI.ClearLocalStorageAsync();
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-API-009", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-API-009", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }
}
