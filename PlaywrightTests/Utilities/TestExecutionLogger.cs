namespace PlaywrightTests.Utilities;

/// <summary>
/// Enhanced test execution logger that captures step-by-step details
/// </summary>
public static class TestExecutionLogger
{
    private static readonly Dictionary<string, TestExecutionRecord> _testRecords = new();
    private static string? _currentTestName;
    private static readonly object _lockObject = new object();

    public class TestExecutionRecord
    {
        public string TestName { get; set; } = string.Empty;
        public List<TestStep> Steps { get; set; } = new();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Passed { get; set; }
        public string? FailureMessage { get; set; }
        public string? FailureStep { get; set; }
        public string? StackTrace { get; set; }
        public List<string> Logs { get; set; } = new();
        public string? Category { get; set; }

        public TimeSpan Duration => EndTime - StartTime;
    }

    public class TestStep
    {
        public int Number { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Passed { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public List<string> StepLogs { get; set; } = new();

        public TimeSpan Duration => EndTime - StartTime;
    }

    /// <summary>
    /// Start recording a test
    /// </summary>
    public static void StartTest(string testName, string? category = null)
    {
        lock (_lockObject)
        {
            _currentTestName = testName;
            _testRecords[testName] = new TestExecutionRecord
            {
                TestName = testName,
                StartTime = DateTime.Now,
                Category = category ?? "General"
            };
        }
    }

    /// <summary>
    /// Record a test step
    /// </summary>
    public static void RecordStep(int stepNumber, string description)
    {
        lock (_lockObject)
        {
            if (_currentTestName != null && _testRecords.TryGetValue(_currentTestName, out var record))
            {
                var step = new TestStep
                {
                    Number = stepNumber,
                    Description = description,
                    StartTime = DateTime.Now
                };
                record.Steps.Add(step);
            }
        }
    }

    /// <summary>
    /// Add log entry to current step
    /// </summary>
    public static void LogStepInfo(string message)
    {
        lock (_lockObject)
        {
            if (_currentTestName != null && _testRecords.TryGetValue(_currentTestName, out var record))
            {
                var lastStep = record.Steps.LastOrDefault();
                if (lastStep != null)
                {
                    lastStep.StepLogs.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
                }
                record.Logs.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    /// <summary>
    /// Mark current step as complete
    /// </summary>
    public static void CompleteStep()
    {
        lock (_lockObject)
        {
            if (_currentTestName != null && _testRecords.TryGetValue(_currentTestName, out var record))
            {
                var lastStep = record.Steps.LastOrDefault();
                if (lastStep != null)
                {
                    lastStep.EndTime = DateTime.Now;
                }
            }
        }
    }

    /// <summary>
    /// Mark step as failed
    /// </summary>
    public static void FailStep(string errorMessage)
    {
        lock (_lockObject)
        {
            if (_currentTestName != null && _testRecords.TryGetValue(_currentTestName, out var record))
            {
                var lastStep = record.Steps.LastOrDefault();
                if (lastStep != null)
                {
                    lastStep.Passed = false;
                    lastStep.ErrorMessage = errorMessage;
                    lastStep.EndTime = DateTime.Now;
                    record.FailureStep = $"Step {lastStep.Number}: {lastStep.Description}";
                }
            }
        }
    }

    /// <summary>
    /// Complete test execution
    /// </summary>
    public static void CompleteTest(bool passed, string? failureMessage = null, string? stackTrace = null)
    {
        lock (_lockObject)
        {
            if (_currentTestName != null && _testRecords.TryGetValue(_currentTestName, out var record))
            {
                record.EndTime = DateTime.Now;
                record.Passed = passed;
                record.FailureMessage = failureMessage;
                record.StackTrace = stackTrace;

                // Close any open steps
                var lastStep = record.Steps.LastOrDefault();
                if (lastStep != null && lastStep.EndTime == DateTime.MinValue)
                {
                    lastStep.EndTime = DateTime.Now;
                }
            }
            _currentTestName = null;
        }
    }

    /// <summary>
    /// Get all test records
    /// </summary>
    public static List<TestExecutionRecord> GetAllRecords()
    {
        lock (_lockObject)
        {
            return _testRecords.Values.ToList();
        }
    }

    /// <summary>
    /// Get specific test record
    /// </summary>
    public static TestExecutionRecord? GetTestRecord(string testName)
    {
        lock (_lockObject)
        {
            return _testRecords.TryGetValue(testName, out var record) ? record : null;
        }
    }

    /// <summary>
    /// Clear all records
    /// </summary>
    public static void Clear()
    {
        lock (_lockObject)
        {
            _testRecords.Clear();
            _currentTestName = null;
        }
    }
}
