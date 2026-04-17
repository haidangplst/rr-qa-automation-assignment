using System.Text;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Enhanced HTML Report Generator with step-by-step execution details
/// </summary>
public class EnhancedHTMLReportGenerator
{
    private readonly string _reportDirectory;

    public EnhancedHTMLReportGenerator(string? reportDirectory = null)
    {
        _reportDirectory = reportDirectory ?? Path.Combine(
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..")),
            "Reports"
        );
        Directory.CreateDirectory(_reportDirectory);
    }

    public string ReportDirectory => _reportDirectory;

    /// <summary>
    /// Generate enhanced HTML report with step-by-step details
    /// </summary>
    public string GenerateReport()
    {
        var records = TestExecutionLogger.GetAllRecords();
        var now = DateTime.Now;
        var reportFileName = $"report_{now:HH_dd_MM_yyyy}.html";
        var reportPath = Path.Combine(_reportDirectory, reportFileName);

        var passedCount = records.Count(r => r.Passed);
        var failedCount = records.Count(r => !r.Passed);
        var passPercentage = records.Count > 0 ? (passedCount * 100) / records.Count : 0;
        var totalDuration = records.Sum(r => r.Duration.TotalSeconds);

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("    <title>Enhanced Test Execution Report</title>");
        html.AppendLine("    <style>");
        html.AppendLine(GetStyles());
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        // Header
        html.AppendLine("    <div class=\"container\">");
        html.AppendLine("        <header class=\"header\">");
        html.AppendLine("            <h1>🧪 Enhanced Test Execution Report</h1>");
        html.AppendLine($"            <p class=\"timestamp\">Generated: {now:yyyy-MM-dd HH:mm:ss}</p>");
        html.AppendLine("        </header>");

        // Summary
        html.AppendLine("        <section class=\"summary\">");
        html.AppendLine("            <h2>Execution Summary</h2>");
        html.AppendLine("            <div class=\"summary-grid\">");
        html.AppendLine("                <div class=\"summary-item\">");
        html.AppendLine("                    <span class=\"label\">Total Tests</span>");
        html.AppendLine($"                    <span class=\"value\">{records.Count}</span>");
        html.AppendLine("                </div>");
        html.AppendLine("                <div class=\"summary-item\">");
        html.AppendLine("                    <span class=\"label\">✓ Passed</span>");
        html.AppendLine($"                    <span class=\"value passed\">{passedCount}</span>");
        html.AppendLine("                </div>");
        html.AppendLine("                <div class=\"summary-item\">");
        html.AppendLine("                    <span class=\"label\">✗ Failed</span>");
        html.AppendLine($"                    <span class=\"value failed\">{failedCount}</span>");
        html.AppendLine("                </div>");
        html.AppendLine("                <div class=\"summary-item\">");
        html.AppendLine("                    <span class=\"label\">Pass Rate</span>");
        html.AppendLine($"                    <span class=\"value\">{passPercentage}%</span>");
        html.AppendLine("                </div>");
        html.AppendLine("                <div class=\"summary-item\">");
        html.AppendLine("                    <span class=\"label\">Duration</span>");
        html.AppendLine($"                    <span class=\"value\">{totalDuration:F2}s</span>");
        html.AppendLine("                </div>");
        html.AppendLine("            </div>");
        html.AppendLine("        </section>");

        // Test Details
        html.AppendLine("        <section class=\"test-details\">");
        html.AppendLine("            <h2>Test Execution Details</h2>");

        foreach (var record in records.OrderByDescending(r => r.Passed))
        {
            html.AppendLine("            <div class=\"test-card\">");
            html.AppendLine($"                <div class=\"test-header {(record.Passed ? "passed" : "failed")}\">");
            html.AppendLine($"                    <h3>{(record.Passed ? "✓" : "✗")} {HtmlEncode(record.TestName)}</h3>");
            html.AppendLine($"                    <span class=\"category\">{HtmlEncode(record.Category ?? "General")}</span>");
            html.AppendLine($"                    <span class=\"duration\">{record.Duration.TotalSeconds:F2}s</span>");
            html.AppendLine("                </div>");

            // Test Information
            html.AppendLine("                <div class=\"test-info\">");
            html.AppendLine($"                    <p><strong>Status:</strong> <span class=\"{(record.Passed ? "status-passed" : "status-failed")}\">{(record.Passed ? "PASSED" : "FAILED")}</span></p>");
            html.AppendLine($"                    <p><strong>Start Time:</strong> {record.StartTime:HH:mm:ss.fff}</p>");
            html.AppendLine($"                    <p><strong>End Time:</strong> {record.EndTime:HH:mm:ss.fff}</p>");
            html.AppendLine($"                    <p><strong>Duration:</strong> {record.Duration.TotalSeconds:F2} seconds</p>");

            if (!record.Passed && record.FailureMessage != null)
            {
                html.AppendLine($"                    <p><strong>Error:</strong> <span class=\"error-message\">{HtmlEncode(record.FailureMessage)}</span></p>");
            }

            if (!record.Passed && record.FailureStep != null)
            {
                html.AppendLine($"                    <p><strong>Failed At:</strong> <span class=\"failed-step\">{HtmlEncode(record.FailureStep)}</span></p>");
            }

            html.AppendLine("                </div>");

            // Steps
            if (record.Steps.Count > 0)
            {
                html.AppendLine("                <div class=\"steps-section\">");
                html.AppendLine("                    <h4>Execution Steps</h4>");
                html.AppendLine("                    <div class=\"steps-list\">");

                foreach (var step in record.Steps)
                {
                    html.AppendLine($"                        <div class=\"step {(step.Passed ? "step-passed" : "step-failed")}\">");
                    html.AppendLine($"                            <div class=\"step-header\">");
                    html.AppendLine($"                                <span class=\"step-number\">Step {step.Number}</span>");
                    html.AppendLine($"                                <span class=\"step-description\">{HtmlEncode(step.Description)}</span>");
                    html.AppendLine($"                                <span class=\"step-duration\">{step.Duration.TotalSeconds:F2}s</span>");
                    html.AppendLine($"                            </div>");

                    if (step.StepLogs.Count > 0)
                    {
                        html.AppendLine("                            <div class=\"step-logs\">");
                        html.AppendLine("                                <div class=\"logs-header\">Step Logs:</div>");
                        html.AppendLine("                                <ul>");
                        foreach (var log in step.StepLogs)
                        {
                            html.AppendLine($"                                    <li>{HtmlEncode(log)}</li>");
                        }
                        html.AppendLine("                                </ul>");
                        html.AppendLine("                            </div>");
                    }

                    if (!step.Passed && step.ErrorMessage != null)
                    {
                        html.AppendLine($"                            <div class=\"step-error\">");
                        html.AppendLine($"                                <div class=\"error-label\">Error:</div>");
                        html.AppendLine($"                                <div class=\"error-details\">{HtmlEncode(step.ErrorMessage)}</div>");
                        html.AppendLine($"                            </div>");
                    }

                    html.AppendLine("                        </div>");
                }

                html.AppendLine("                    </div>");
                html.AppendLine("                </div>");
            }

            // Full Logs
            if (record.Logs.Count > 0)
            {
                html.AppendLine("                <div class=\"logs-section\">");
                html.AppendLine("                    <h4>Full Test Logs</h4>");
                html.AppendLine("                    <div class=\"logs-container\">");
                foreach (var log in record.Logs)
                {
                    html.AppendLine($"                        <div class=\"log-line\">{HtmlEncode(log)}</div>");
                }
                html.AppendLine("                    </div>");
                html.AppendLine("                </div>");
            }

            // Stack Trace
            if (!record.Passed && record.StackTrace != null)
            {
                html.AppendLine("                <div class=\"stacktrace-section\">");
                html.AppendLine("                    <h4>Stack Trace</h4>");
                html.AppendLine("                    <div class=\"stacktrace-container\">");
                html.AppendLine($"                        <pre>{HtmlEncode(record.StackTrace)}</pre>");
                html.AppendLine("                    </div>");
                html.AppendLine("                </div>");
            }

            html.AppendLine("            </div>");
        }

        html.AppendLine("        </section>");

        html.AppendLine("        <footer class=\"footer\">");
        html.AppendLine($"            <p>Report generated: {now:yyyy-MM-dd HH:mm:ss}</p>");
        html.AppendLine("            <p>Enhanced Test Execution Report with Step-by-Step Details</p>");
        html.AppendLine("        </footer>");
        html.AppendLine("    </div>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        File.WriteAllText(reportPath, html.ToString());
        Logger.Info($"Enhanced HTML Report generated: {reportPath}");
        return reportPath;
    }

    private string GetStyles()
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

.container {
    max-width: 1400px;
    margin: 0 auto;
    background: white;
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    overflow: hidden;
}

.header {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 40px 20px;
    text-align: center;
}

.header h1 {
    font-size: 2.5em;
    margin-bottom: 10px;
}

.timestamp {
    font-size: 0.95em;
    opacity: 0.95;
}

.summary {
    padding: 40px 20px;
    background: #f9fafb;
    border-bottom: 2px solid #e5e7eb;
}

.summary h2 {
    margin-bottom: 25px;
    color: #1f2937;
    font-size: 1.8em;
}

.summary-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
    gap: 20px;
}

.summary-item {
    padding: 20px;
    background: white;
    border-radius: 8px;
    border-left: 4px solid #667eea;
    text-align: center;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.summary-item .label {
    display: block;
    font-size: 0.85em;
    color: #6b7280;
    font-weight: 600;
    margin-bottom: 8px;
    text-transform: uppercase;
}

.summary-item .value {
    display: block;
    font-size: 2em;
    font-weight: 700;
    color: #1f2937;
}

.summary-item .value.passed {
    color: #10b981;
}

.summary-item .value.failed {
    color: #ef4444;
}

.test-details {
    padding: 40px 20px;
}

.test-details h2 {
    margin-bottom: 30px;
    color: #1f2937;
    font-size: 1.8em;
    padding-bottom: 15px;
    border-bottom: 2px solid #e5e7eb;
}

.test-card {
    background: white;
    border: 1px solid #e5e7eb;
    border-radius: 8px;
    margin-bottom: 25px;
    overflow: hidden;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    transition: box-shadow 0.2s;
}

.test-card:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
}

.test-header {
    padding: 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-bottom: 2px solid #e5e7eb;
    background: #f9fafb;
}

.test-header.passed {
    border-left: 5px solid #10b981;
}

.test-header.failed {
    border-left: 5px solid #ef4444;
}

.test-header h3 {
    margin: 0;
    color: #1f2937;
    font-size: 1.3em;
}

.test-header .category {
    background: #667eea;
    color: white;
    padding: 4px 12px;
    border-radius: 20px;
    font-size: 0.85em;
    margin-left: 15px;
}

.test-header .duration {
    color: #6b7280;
    font-weight: 600;
    margin-left: auto;
}

.test-info {
    padding: 20px;
    background: #f9fafb;
}

.test-info p {
    margin-bottom: 10px;
    font-size: 0.95em;
}

.status-passed {
    color: #10b981;
    font-weight: 600;
}

.status-failed {
    color: #ef4444;
    font-weight: 600;
}

.error-message {
    color: #ef4444;
    background: #fff5f5;
    padding: 4px 8px;
    border-radius: 4px;
}

.failed-step {
    color: #d97706;
    background: #fffbeb;
    padding: 4px 8px;
    border-radius: 4px;
}

.steps-section {
    padding: 20px;
}

.steps-section h4 {
    color: #667eea;
    margin-bottom: 15px;
    font-size: 1.1em;
}

.steps-list {
    display: flex;
    flex-direction: column;
    gap: 15px;
}

.step {
    border: 1px solid #e5e7eb;
    border-radius: 6px;
    padding: 15px;
    background: white;
}

.step.step-passed {
    border-left: 4px solid #10b981;
    background: #f0fdf4;
}

.step.step-failed {
    border-left: 4px solid #ef4444;
    background: #fef2f2;
}

.step-header {
    display: flex;
    align-items: center;
    gap: 15px;
    margin-bottom: 10px;
}

.step-number {
    background: #667eea;
    color: white;
    padding: 4px 12px;
    border-radius: 4px;
    font-weight: 600;
    min-width: 60px;
    text-align: center;
}

.step-description {
    flex: 1;
    color: #1f2937;
    font-weight: 500;
}

.step-duration {
    color: #6b7280;
    font-weight: 600;
}

.step-logs {
    margin: 10px 0;
    padding: 10px;
    background: white;
    border-radius: 4px;
    border-left: 3px solid #667eea;
}

.logs-header {
    font-weight: 600;
    color: #667eea;
    margin-bottom: 8px;
    font-size: 0.9em;
}

.step-logs ul {
    list-style: none;
    margin: 0;
}

.step-logs li {
    padding: 4px 0;
    color: #6b7280;
    font-size: 0.9em;
    font-family: 'Courier New', monospace;
}

.step-error {
    margin: 10px 0;
    padding: 10px;
    background: #fff5f5;
    border-radius: 4px;
    border-left: 3px solid #ef4444;
}

.error-label {
    font-weight: 600;
    color: #ef4444;
    margin-bottom: 8px;
    font-size: 0.9em;
}

.error-details {
    color: #991b1b;
    font-size: 0.9em;
    font-family: 'Courier New', monospace;
    white-space: pre-wrap;
    word-break: break-word;
}

.logs-section {
    padding: 20px;
    border-top: 1px solid #e5e7eb;
}

.logs-section h4 {
    color: #667eea;
    margin-bottom: 15px;
    font-size: 1.1em;
}

.logs-container {
    background: #1f2937;
    color: #10b981;
    padding: 15px;
    border-radius: 6px;
    font-family: 'Courier New', monospace;
    font-size: 0.9em;
    max-height: 400px;
    overflow-y: auto;
}

.log-line {
    padding: 2px 0;
    line-height: 1.4;
}

.stacktrace-section {
    padding: 20px;
    border-top: 1px solid #e5e7eb;
    background: #f9fafb;
}

.stacktrace-section h4 {
    color: #ef4444;
    margin-bottom: 15px;
    font-size: 1.1em;
}

.stacktrace-container {
    background: #1f2937;
    color: #ef4444;
    padding: 15px;
    border-radius: 6px;
    overflow-x: auto;
}

.stacktrace-container pre {
    margin: 0;
    font-family: 'Courier New', monospace;
    font-size: 0.85em;
    line-height: 1.5;
    white-space: pre-wrap;
    word-break: break-word;
}

.footer {
    padding: 20px;
    background: #f3f4f6;
    border-top: 2px solid #e5e7eb;
    text-align: center;
    color: #6b7280;
}

@media (max-width: 768px) {
    .summary-grid {
        grid-template-columns: 1fr 1fr;
    }

    .test-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 10px;
    }

    .test-header .category {
        margin-left: 0;
    }

    .test-header .duration {
        margin-left: 0;
    }

    .step-header {
        flex-direction: column;
        align-items: flex-start;
    }
}
";
    }

    private string HtmlEncode(string text)
    {
        return System.Net.WebUtility.HtmlEncode(text);
    }
}
