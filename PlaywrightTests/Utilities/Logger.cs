using System.Diagnostics;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Logging utility for test automation
/// Provides console and file logging with severity levels
/// </summary>
public static class Logger
{
    private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
    private static readonly string LogFile = Path.Combine(LogDirectory, $"test_log_{DateTime.Now:yyyyMMdd_HHmmss}.log");
    private static readonly object _lockObject = new object();

    static Logger()
    {
        Directory.CreateDirectory(LogDirectory);
        lock (_lockObject)
        {
            File.WriteAllText(LogFile, $"Test Execution Log - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
            File.AppendAllText(LogFile, new string('-', 80) + "\n\n");
        }
    }

    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }

    /// <summary>
    /// Log debug message
    /// </summary>
    public static void Debug(string message, string? context = null)
    {
        Log(LogLevel.Debug, message, context);
    }

    /// <summary>
    /// Log info message
    /// </summary>
    public static void Info(string message, string? context = null)
    {
        Log(LogLevel.Info, message, context);
    }

    /// <summary>
    /// Log warning message
    /// </summary>
    public static void Warning(string message, string? context = null)
    {
        Log(LogLevel.Warning, message, context);
    }

    /// <summary>
    /// Log error message
    /// </summary>
    public static void Error(string message, Exception? ex = null, string? context = null)
    {
        var fullMessage = ex != null ? $"{message}\nException: {ex.Message}\n{ex.StackTrace}" : message;
        Log(LogLevel.Error, fullMessage, context);
    }

    /// <summary>
    /// Log critical message
    /// </summary>
    public static void Critical(string message, string? context = null)
    {
        Log(LogLevel.Critical, message, context);
    }

    /// <summary>
    /// Log step (test step)
    /// </summary>
    public static void Step(string stepNumber, string description)
    {
        var message = $"[STEP {stepNumber}] {description}";
        Console.WriteLine($"  ℹ️  {message}");
        lock (_lockObject)
        {
            File.AppendAllText(LogFile, $"{DateTime.Now:HH:mm:ss.fff} | STEP | {message}\n");
        }
    }

    /// <summary>
    /// Log assertion result
    /// </summary>
    public static void Assert(bool condition, string assertion, string failureMessage = "")
    {
        var status = condition ? "✓ PASS" : "✗ FAIL";
        var message = $"Assertion: {assertion}";
        if (!condition && !string.IsNullOrEmpty(failureMessage))
        {
            message += $" | {failureMessage}";
        }

        Log(condition ? LogLevel.Info : LogLevel.Error, message, "ASSERTION");
        Console.WriteLine($"  {(condition ? "✓" : "✗")} {message}");
    }

    /// <summary>
    /// Log API call
    /// </summary>
    public static void ApiCall(string method, string url, int statusCode, long durationMs)
    {
        var message = $"{method} {url} -> {statusCode} ({durationMs}ms)";
        var logLevel = statusCode >= 200 && statusCode < 300 ? LogLevel.Info : LogLevel.Warning;
        Log(logLevel, message, "API");
        Console.WriteLine($"  🔗 API: {message}");
    }

    /// <summary>
    /// Log network error
    /// </summary>
    public static void NetworkError(string url, string error)
    {
        var message = $"Network error on {url}: {error}";
        Log(LogLevel.Error, message, "NETWORK");
        Console.WriteLine($"  ✗ {message}");
    }

    /// <summary>
    /// Log test start
    /// </summary>
    public static void TestStart(string testName)
    {
        var message = $"TEST START: {testName}";
        Console.WriteLine($"\n{'='*80}");
        Console.WriteLine($"🧪 {message}");
        Console.WriteLine($"{'='*80}");
        lock (_lockObject)
        {
            File.AppendAllText(LogFile, $"\n{DateTime.Now:HH:mm:ss.fff} | TEST START | {testName}\n");
        }
    }

    /// <summary>
    /// Log test end
    /// </summary>
    public static void TestEnd(string testName, bool passed, string? message = null)
    {
        var status = passed ? "✓ PASSED" : "✗ FAILED";
        Console.WriteLine($"{status}: {testName}");
        if (!string.IsNullOrEmpty(message))
        {
            Console.WriteLine($"  Message: {message}");
        }
        Console.WriteLine();

        lock (_lockObject)
        {
            File.AppendAllText(LogFile, $"{DateTime.Now:HH:mm:ss.fff} | TEST {(passed ? "PASSED" : "FAILED")} | {testName}\n");
            if (!string.IsNullOrEmpty(message))
            {
                File.AppendAllText(LogFile, $"  {message}\n");
            }
        }
    }

    /// <summary>
    /// Log test screenshot
    /// </summary>
    public static void Screenshot(string fileName)
    {
        var message = $"Screenshot captured: {fileName}";
        Log(LogLevel.Info, message, "SCREENSHOT");
        Console.WriteLine($"  📸 {message}");
    }

    /// <summary>
    /// Core logging method
    /// </summary>
    private static void Log(LogLevel level, string message, string? context = null)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var levelString = level.ToString().ToUpper();
        var contextString = !string.IsNullOrEmpty(context) ? $" [{context}]" : "";
        var logLine = $"{timestamp} | {levelString}{contextString} | {message}";

        // Write to file with lock
        lock (_lockObject)
        {
            File.AppendAllText(LogFile, logLine + "\n");
        }

        // Write to console with color
        var originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
            Console.WriteLine(logLine);
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    /// <summary>
    /// Get log file path
    /// </summary>
    public static string GetLogFilePath() => LogFile;

    /// <summary>
    /// Get all log content
    /// </summary>
    public static string GetAllLogs()
    {
        try
        {
            return File.ReadAllText(LogFile);
        }
        catch
        {
            return "Log file not available";
        }
    }
}
