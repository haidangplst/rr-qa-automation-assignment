using System.Reflection;
using System.Text;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Generates HTML test reports from test results
/// </summary>
public class HTMLReportGenerator
{
    private readonly List<TestResult> _results;
    private readonly DateTime _executionStartTime;
    private readonly string _reportDirectory;

    public class TestResult
    {
        public string TestName { get; set; } = "";
        public bool Passed { get; set; }
        public string? Message { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> Screenshots { get; set; } = new();
        public string? Category { get; set; }
        public string? LogFile { get; set; }
    }

    public HTMLReportGenerator()
    {
        _results = new List<TestResult>();
        _executionStartTime = DateTime.Now;

        // Set report directory to project root /Reports folder
        // Navigate from bin/Debug/net10.0 up to project root
        var binDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(binDirectory, "..", "..", ".."));
        _reportDirectory = Path.Combine(projectRoot, "Reports");

        Directory.CreateDirectory(_reportDirectory);
    }

    /// <summary>
    /// Add test result
    /// </summary>
    public void AddResult(TestResult result)
    {
        _results.Add(result);
    }

    /// <summary>
    /// Generate HTML report
    /// </summary>
    public string GenerateHTMLReport()
    {
        var sb = new StringBuilder();
        var totalTests = _results.Count;
        var passedTests = _results.Count(r => r.Passed);
        var failedTests = _results.Count(r => !r.Passed);
        var passPercentage = totalTests > 0 ? (passedTests * 100) / totalTests : 0;
        var totalDuration = _results.Sum(r => (r.EndTime - r.StartTime).TotalSeconds);

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Test Automation Report - TMDB Discover</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine(GetCSSStyles());
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        // Header
        sb.AppendLine("    <div class=\"header\">");
        sb.AppendLine("        <h1>🧪 Test Automation Report</h1>");
        sb.AppendLine("        <p class=\"subtitle\">TMDB Discover Platform - Functional Testing</p>");
        sb.AppendLine("    </div>");

        // Summary
        sb.AppendLine("    <div class=\"summary\">");
        sb.AppendLine("        <div class=\"summary-item\">");
        sb.AppendLine($"            <div class=\"summary-label\">Total Tests</div>");
        sb.AppendLine($"            <div class=\"summary-value\">{totalTests}</div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"summary-item success\">");
        sb.AppendLine($"            <div class=\"summary-label\">✓ Passed</div>");
        sb.AppendLine($"            <div class=\"summary-value\">{passedTests}</div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"summary-item failure\">");
        sb.AppendLine($"            <div class=\"summary-label\">✗ Failed</div>");
        sb.AppendLine($"            <div class=\"summary-value\">{failedTests}</div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"summary-item\">");
        sb.AppendLine($"            <div class=\"summary-label\">Pass Rate</div>");
        sb.AppendLine($"            <div class=\"summary-value\">{passPercentage}%</div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"summary-item\">");
        sb.AppendLine($"            <div class=\"summary-label\">Duration</div>");
        sb.AppendLine($"            <div class=\"summary-value\">{totalDuration:F1}s</div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");

        // Progress Bar
        sb.AppendLine("    <div class=\"progress-bar\">");
        sb.AppendLine($"        <div class=\"progress-fill\" style=\"width: {passPercentage}%\"></div>");
        sb.AppendLine("    </div>");

        // Results Table
        sb.AppendLine("    <div class=\"results-section\">");
        sb.AppendLine("        <h2>Test Results</h2>");
        sb.AppendLine("        <table class=\"results-table\">");
        sb.AppendLine("            <thead>");
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Test Name</th>");
        sb.AppendLine("                    <th>Category</th>");
        sb.AppendLine("                    <th>Status</th>");
        sb.AppendLine("                    <th>Duration</th>");
        sb.AppendLine("                    <th>Details</th>");
        sb.AppendLine("                </tr>");
        sb.AppendLine("            </thead>");
        sb.AppendLine("            <tbody>");

        foreach (var result in _results.OrderByDescending(r => r.Passed))
        {
            var status = result.Passed ? "✓ PASSED" : "✗ FAILED";
            var statusClass = result.Passed ? "passed" : "failed";
            var duration = (result.EndTime - result.StartTime).TotalSeconds;

            sb.AppendLine($"                <tr class=\"{statusClass}\">");
            sb.AppendLine($"                    <td class=\"test-name\">{result.TestName}</td>");
            sb.AppendLine($"                    <td>{result.Category ?? "N/A"}</td>");
            sb.AppendLine($"                    <td class=\"status\">{status}</td>");
            sb.AppendLine($"                    <td>{duration:F2}s</td>");
            sb.AppendLine($"                    <td>{(string.IsNullOrEmpty(result.Message) ? "-" : result.Message)}</td>");
            sb.AppendLine("                </tr>");
        }

        sb.AppendLine("            </tbody>");
        sb.AppendLine("        </table>");
        sb.AppendLine("    </div>");

        // Category Summary
        var categories = _results.GroupBy(r => r.Category ?? "Uncategorized");
        sb.AppendLine("    <div class=\"category-section\">");
        sb.AppendLine("        <h2>Results by Category</h2>");
        sb.AppendLine("        <div class=\"category-grid\">");

        foreach (var category in categories)
        {
            var catPassed = category.Count(r => r.Passed);
            var catTotal = category.Count();

            sb.AppendLine("            <div class=\"category-card\">");
            sb.AppendLine($"                <h3>{category.Key}</h3>");
            sb.AppendLine($"                <p>{catPassed}/{catTotal} Passed</p>");
            sb.AppendLine($"                <div class=\"category-progress\">");
            sb.AppendLine($"                    <div class=\"category-fill\" style=\"width: {(catTotal > 0 ? (catPassed * 100) / catTotal : 0)}%\"></div>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
        }

        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");

        // Failure Details
        var failedResults = _results.Where(r => !r.Passed).ToList();
        if (failedResults.Count > 0)
        {
            sb.AppendLine("    <div class=\"failures-section\">");
            sb.AppendLine("        <h2>❌ Failures</h2>");

            foreach (var failure in failedResults)
            {
                sb.AppendLine("        <div class=\"failure-card\">");
                sb.AppendLine($"            <h3>{failure.TestName}</h3>");
                sb.AppendLine($"            <p><strong>Message:</strong> {failure.Message}</p>");

                if (failure.Screenshots.Count > 0)
                {
                    sb.AppendLine("            <div class=\"screenshots\">");
                    foreach (var screenshot in failure.Screenshots)
                    {
                        sb.AppendLine($"                <img src=\"{screenshot}\" alt=\"Screenshot\" />");
                    }
                    sb.AppendLine("            </div>");
                }

                sb.AppendLine("        </div>");
            }

            sb.AppendLine("    </div>");
        }

        // Environment Info
        sb.AppendLine("    <div class=\"environment-section\">");
        sb.AppendLine("        <h3>Environment</h3>");
        sb.AppendLine($"        <p><strong>Execution Time:</strong> {_executionStartTime:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine($"        <p><strong>OS:</strong> {Environment.OSVersion}</p>");
        sb.AppendLine($"        <p><strong>.NET Version:</strong> {Environment.Version}</p>");
        sb.AppendLine($"        <p><strong>Machine:</strong> {Environment.MachineName}</p>");
        sb.AppendLine("    </div>");

        // Footer
        sb.AppendLine("    <div class=\"footer\">");
        sb.AppendLine($"        <p>Report generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine("        <p>Automation Framework: Playwright + NUnit</p>");
        sb.AppendLine("    </div>");

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    /// <summary>
    /// Save HTML report to file with naming format: report_{hour_day_month_year}.html
    /// </summary>
    public string SaveHTMLReport()
    {
        var now = DateTime.Now;
        var reportFileName = $"report_{now:HH_dd_MM_yyyy}.html";
        var reportPath = Path.Combine(_reportDirectory, reportFileName);

        var html = GenerateHTMLReport();
        File.WriteAllText(reportPath, html);

        Logger.Info($"HTML Report saved: {reportPath}");
        return reportPath;
    }

    /// <summary>
    /// Get CSS styles for HTML report
    /// </summary>
    private static string GetCSSStyles()
    {
        return @"
            * {
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }

            body {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: #333;
                padding: 20px;
                line-height: 1.6;
            }

            .header {
                background: white;
                padding: 30px;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                margin-bottom: 20px;
                text-align: center;
            }

            .header h1 {
                color: #667eea;
                font-size: 2.5em;
                margin-bottom: 10px;
            }

            .subtitle {
                color: #666;
                font-size: 1.1em;
            }

            .summary {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
                gap: 15px;
                margin-bottom: 20px;
            }

            .summary-item {
                background: white;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                text-align: center;
                border-left: 4px solid #667eea;
            }

            .summary-item.success {
                border-left-color: #28a745;
            }

            .summary-item.failure {
                border-left-color: #dc3545;
            }

            .summary-label {
                color: #666;
                font-size: 0.9em;
                text-transform: uppercase;
                margin-bottom: 10px;
            }

            .summary-value {
                font-size: 2em;
                font-weight: bold;
                color: #667eea;
            }

            .summary-item.success .summary-value {
                color: #28a745;
            }

            .summary-item.failure .summary-value {
                color: #dc3545;
            }

            .progress-bar {
                width: 100%;
                height: 40px;
                background: white;
                border-radius: 20px;
                overflow: hidden;
                margin-bottom: 30px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            }

            .progress-fill {
                height: 100%;
                background: linear-gradient(90deg, #28a745 0%, #20c997 100%);
                display: flex;
                align-items: center;
                justify-content: center;
                color: white;
                font-weight: bold;
            }

            .results-section, .category-section, .failures-section, .environment-section {
                background: white;
                padding: 30px;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                margin-bottom: 20px;
            }

            .results-section h2, .category-section h2, .failures-section h2 {
                color: #667eea;
                margin-bottom: 20px;
                font-size: 1.8em;
                border-bottom: 2px solid #667eea;
                padding-bottom: 10px;
            }

            .results-table {
                width: 100%;
                border-collapse: collapse;
            }

            .results-table thead {
                background: #f8f9fa;
            }

            .results-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #667eea;
                border-bottom: 2px solid #dee2e6;
            }

            .results-table td {
                padding: 12px 15px;
                border-bottom: 1px solid #dee2e6;
            }

            .results-table tr.passed {
                background: #f0fff4;
            }

            .results-table tr.failed {
                background: #fff5f5;
            }

            .results-table tr:hover {
                background: #f1f3ff;
            }

            .status {
                font-weight: bold;
            }

            .results-table tr.passed .status {
                color: #28a745;
            }

            .results-table tr.failed .status {
                color: #dc3545;
            }

            .category-grid {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                gap: 20px;
            }

            .category-card {
                background: #f8f9fa;
                padding: 20px;
                border-radius: 8px;
                border: 2px solid #dee2e6;
            }

            .category-card h3 {
                color: #667eea;
                margin-bottom: 10px;
            }

            .category-progress {
                width: 100%;
                height: 20px;
                background: #dee2e6;
                border-radius: 10px;
                overflow: hidden;
                margin-top: 10px;
            }

            .category-fill {
                height: 100%;
                background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
            }

            .failure-card {
                background: #fff5f5;
                padding: 20px;
                border-left: 4px solid #dc3545;
                margin-bottom: 15px;
                border-radius: 4px;
            }

            .failure-card h3 {
                color: #dc3545;
                margin-bottom: 10px;
            }

            .screenshots {
                margin-top: 15px;
            }

            .screenshots img {
                max-width: 100%;
                height: auto;
                border-radius: 4px;
                margin-top: 10px;
            }

            .environment-section {
                background: #f8f9fa;
            }

            .environment-section h3 {
                color: #667eea;
                margin-bottom: 15px;
            }

            .environment-section p {
                margin: 8px 0;
            }

            .footer {
                text-align: center;
                padding: 20px;
                color: white;
                font-size: 0.9em;
            }

            @media (max-width: 768px) {
                .summary {
                    grid-template-columns: repeat(2, 1fr);
                }

                .header h1 {
                    font-size: 1.8em;
                }

                .results-table {
                    font-size: 0.9em;
                }

                .results-table th, .results-table td {
                    padding: 8px;
                }
            }
        ";
    }

    /// <summary>
    /// Generate console summary
    /// </summary>
    public void PrintConsoleSummary()
    {
        var totalTests = _results.Count;
        var passedTests = _results.Count(r => r.Passed);
        var failedTests = _results.Count(r => !r.Passed);
        var passPercentage = totalTests > 0 ? (passedTests * 100) / totalTests : 0;

        Console.WriteLine("\n" + new string('=', 80));
        Console.WriteLine("TEST EXECUTION SUMMARY");
        Console.WriteLine(new string('=', 80));
        Console.WriteLine($"Total Tests:  {totalTests}");
        Console.WriteLine($"✓ Passed:     {passedTests}");
        Console.WriteLine($"✗ Failed:     {failedTests}");
        Console.WriteLine($"Pass Rate:    {passPercentage}%");
        Console.WriteLine(new string('=', 80) + "\n");

        if (failedTests > 0)
        {
            Console.WriteLine("Failed Tests:");
            foreach (var failure in _results.Where(r => !r.Passed))
            {
                Console.WriteLine($"  ✗ {failure.TestName}");
                if (!string.IsNullOrEmpty(failure.Message))
                {
                    Console.WriteLine($"    {failure.Message}");
                }
            }
            Console.WriteLine();
        }
    }
}
