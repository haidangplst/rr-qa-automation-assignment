using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.BaseTests;

/// <summary>
/// Custom PageTest base class that automatically generates reports after all tests complete
/// </summary>
public class PlaywrightPageTest : PageTest
{
    private static bool _reportGenerated = false;
    private static readonly object _reportLock = new object();

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
