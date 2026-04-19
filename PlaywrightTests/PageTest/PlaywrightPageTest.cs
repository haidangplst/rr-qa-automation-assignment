using System.Buffers.Text;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.BaseTests;

/// <summary>
/// Custom PageTest base class that automatically generates reports after all tests complete
/// and navigates to base URL before each test
/// </summary>
public class PlaywrightPageTest : PageTest
{
    private static bool _reportGenerated = false;
    private static readonly object _reportLock = new object();

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true
        };

    }

    [SetUp]
    public async Task SetUp()
    {
        // Navigate to base URL before each test
        await Page.GotoAsync(TestConfig.BaseUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfig.NavigationTimeout
        });
        Logger.Info($"Navigated to base URL: {TestConfig.BaseUrl}");
    }

    [OneTimeTearDown]
    public new void OneTimeTearDown()
    {
        // Generate report only once after all tests complete
        lock (_reportLock)
        {
            if (!_reportGenerated)
            {
                try
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine("📊 GENERATING TEST EXECUTION REPORT...");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");

                    var reportGenerator = new EnhancedHTMLReportGenerator();
                    var reportPath = reportGenerator.GenerateReport();

                    Console.WriteLine($"✅ Report generated successfully!");
                    Console.WriteLine($"📁 Location: {reportPath}");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

                    _reportGenerated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error generating report: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }
    }
}
