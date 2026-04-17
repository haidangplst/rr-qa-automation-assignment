using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Utilities;

namespace PlaywrightTests;

/// <summary>
/// Assembly-level fixture that generates reports after all tests complete
/// </summary>
[SetUpFixture]
public class AssemblySetupTearDown
{
    private static bool _reportGenerated = false;

    [OneTimeTearDown]
    public void GenerateReportOnAssemblyComplete()
    {
        if (!_reportGenerated)
        {
            try
            {
                var browserConfig = BrowserFactory.GetBrowserConfigurationName();

                System.Console.WriteLine("\n");
                System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
                System.Console.WriteLine("📊 GENERATING TEST EXECUTION REPORT");
                System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
                System.Console.WriteLine($"Browser: {browserConfig}");

                var reportGenerator = new EnhancedHTMLReportGenerator();
                var reportPath = reportGenerator.GenerateReport();

                System.Console.WriteLine($"✅ Report generated successfully!");
                System.Console.WriteLine($"📁 Location: {reportPath}");
                System.Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

                _reportGenerated = true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Error generating report: {ex.Message}");
                System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}

