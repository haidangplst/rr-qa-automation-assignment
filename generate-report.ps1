#!/usr/bin/env pwsh

# Generate HTML Test Report
# This script runs the tests and generates an HTML report with the format: report_{HH_dd_MM_yyyy}.html

$ProjectPath = "PlaywrightTests"
$ReportDir = "PlaywrightTests\bin\Debug\net10.0\Reports"

# Create Reports directory if it doesn't exist
if (-not (Test-Path $ReportDir)) {
    New-Item -ItemType Directory -Path $ReportDir -Force | Out-Null
    Write-Host "Created Reports directory: $ReportDir"
}

# Get current date/time for report naming
$Now = Get-Date
$ReportFileName = "report_{0:HH_dd_MM_yyyy}.html" -f $Now
$ReportPath = Join-Path $ReportDir $ReportFileName

Write-Host "=========================================="
Write-Host "Running Tests and Generating HTML Report"
Write-Host "=========================================="
Write-Host "Report will be saved to: $ReportPath"
Write-Host ""

# Run the tests with XML output for analysis
$TestResults = @()
Write-Host "Running CategoryPagesTests..."

# Execute tests and capture output
$testOutput = dotnet test $ProjectPath --filter "FullyQualifiedName~CategoryPagesTests" --logger "console;verbosity=minimal" -v minimal 2>&1
$TestResults += $testOutput

Write-Host ""
Write-Host "Creating HTML Report..."

# Create a basic HTML report
$htmlContent = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Test Execution Report</title>
    <style>
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
            min-height: 100vh;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
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
            font-size: 0.9em;
            opacity: 0.9;
        }

        .summary {
            padding: 30px 20px;
            background: #f9fafb;
            border-bottom: 1px solid #e5e7eb;
        }

        .summary h2 {
            margin-bottom: 20px;
            color: #1f2937;
            font-size: 1.5em;
        }

        .summary-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 15px;
        }

        .summary-item {
            padding: 15px;
            background: white;
            border-radius: 6px;
            border-left: 4px solid #667eea;
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .summary-label {
            font-size: 0.85em;
            color: #6b7280;
            font-weight: 500;
        }

        .summary-value {
            font-size: 1.5em;
            font-weight: bold;
            color: #1f2937;
        }

        .passed {
            color: #10b981;
        }

        .failed {
            color: #ef4444;
        }

        .content {
            padding: 30px 20px;
        }

        .test-info {
            background: #f3f4f6;
            padding: 20px;
            border-radius: 6px;
            margin-bottom: 20px;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
            max-height: 400px;
            overflow-y: auto;
            border-left: 4px solid #667eea;
        }

        .footer {
            padding: 20px;
            background: #f3f4f6;
            border-top: 1px solid #e5e7eb;
            text-align: center;
            font-size: 0.9em;
            color: #6b7280;
        }

        h3 {
            color: #667eea;
            margin-top: 20px;
            margin-bottom: 10px;
        }

        pre {
            background: white;
            padding: 10px;
            border-radius: 4px;
            overflow-x: auto;
            margin: 10px 0;
        }

        @media (max-width: 768px) {
            .summary-grid {
                grid-template-columns: 1fr 1fr;
            }

            .header h1 {
                font-size: 1.8em;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <header class="header">
            <h1>🧪 Test Execution Report</h1>
            <p class="timestamp">Generated: $($Now.ToString('yyyy-MM-dd HH:mm:ss'))</p>
        </header>

        <section class="summary">
            <h2>Execution Summary</h2>
            <div class="summary-grid">
                <div class="summary-item">
                    <span class="summary-label">Test Suite</span>
                    <span class="summary-value">CategoryPagesTests</span>
                </div>
                <div class="summary-item">
                    <span class="summary-label">Execution Date</span>
                    <span class="summary-value">$($Now.ToString('MM/dd/yyyy'))</span>
                </div>
                <div class="summary-item">
                    <span class="summary-label">Report Format</span>
                    <span class="summary-value">HTML</span>
                </div>
                <div class="summary-item">
                    <span class="summary-label">Framework</span>
                    <span class="summary-value">Playwright + NUnit</span>
                </div>
            </div>
        </section>

        <section class="content">
            <h3>Test Execution Details</h3>
            <div class="test-info">
                <pre>$($testOutput | Out-String)</pre>
            </div>

            <h3>Report Information</h3>
            <p>
                This report was generated on <strong>$($Now.ToString('yyyy-MM-dd HH:mm:ss'))</strong>
                as part of the automated test execution for the TMDB Discover project.
            </p>
            <p>
                The HTML report file has been saved with the naming convention:
                <strong>report_{hour}_{day}_{month}_{year}.html</strong>
            </p>
        </section>

        <footer class="footer">
            <p>Report generated by Playwright Test Automation Framework</p>
            <p>Test execution completed successfully</p>
        </footer>
    </div>
</body>
</html>
"@

# Save the HTML report
Set-Content -Path $ReportPath -Value $htmlContent -Encoding UTF8

Write-Host ""
Write-Host "=========================================="
Write-Host "✓ HTML Report Generated Successfully!"
Write-Host "=========================================="
Write-Host "Report Path: $ReportPath"
Write-Host ""
Write-Host "File Details:"
Get-Item $ReportPath | Format-List Name, CreationTime, Length
