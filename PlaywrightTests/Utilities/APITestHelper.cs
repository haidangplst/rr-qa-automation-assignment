using Microsoft.Playwright;

namespace PlaywrightTests.Utilities;

/// <summary>
/// Utility for capturing and analyzing API calls and responses
/// </summary>
public class APITestHelper
{
    private readonly IPage _page;
    private readonly List<NetworkCall> _networkCalls;

    public class NetworkCall
    {
        public string Method { get; set; } = "";
        public string URL { get; set; } = "";
        public int StatusCode { get; set; }
        public long Duration { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public APITestHelper(IPage page)
    {
        _page = page;
        _networkCalls = new List<NetworkCall>();
    }

    /// <summary>
    /// Start intercepting network calls
    /// </summary>
    public async Task StartNetworkCapturingAsync()
    {
        if (_page == null)
        {
            Logger.Warning("Cannot start network capturing - page is null");
            return;
        }

        Logger.Info("Starting network call capture");

        // Capture all network responses
        _page.Response += async (_, response) =>
        {
            try
            {
                var request = response.Request;
                var call = new NetworkCall
                {
                    Method = request.Method,
                    URL = request.Url,
                    StatusCode = response.Status,
                    Timestamp = DateTime.Now
                };

                // Try to capture response body for JSON APIs
                if (response.Url.Contains("api") || response.Url.Contains("discover"))
                {
                    try
                    {
                        call.ResponseBody = await response.TextAsync();
                    }
                    catch
                    {
                        call.ResponseBody = null;
                    }
                }

                _networkCalls.Add(call);

                // Log API calls
                if (response.Url.Contains("api") || response.Url.Contains("discover"))
                {
                    Logger.ApiCall(request.Method, request.Url, response.Status, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error capturing network response: {ex.Message}", ex);
            }
        };
    }

    /// <summary>
    /// Get all captured API calls
    /// </summary>
    public List<NetworkCall> GetAllNetworkCalls() => _networkCalls;

    /// <summary>
    /// Get API calls for a specific endpoint
    /// </summary>
    public List<NetworkCall> GetAPICallsForEndpoint(string endpoint)
    {
        return _networkCalls
            .Where(c => c.URL.Contains(endpoint) && (c.URL.Contains("api") || c.URL.Contains("discover")))
            .ToList();
    }

    /// <summary>
    /// Verify API response contains expected data
    /// </summary>
    public bool VerifyAPIResponse(string endpoint, string expectedContent)
    {
        var call = GetAPICallsForEndpoint(endpoint).LastOrDefault();
        if (call?.ResponseBody == null)
        {
            Logger.Warning($"No API response captured for endpoint: {endpoint}");
            return false;
        }

        var contains = call.ResponseBody.Contains(expectedContent);
        Logger.Assert(contains, $"API response contains '{expectedContent}'",
            $"Response: {call.ResponseBody.Substring(0, Math.Min(200, call.ResponseBody.Length))}...");
        return contains;
    }

    /// <summary>
    /// Get all 4xx/5xx responses (errors)
    /// </summary>
    public List<NetworkCall> GetErrorResponses()
    {
        return _networkCalls
            .Where(c => c.StatusCode >= 400)
            .ToList();
    }

    /// <summary>
    /// Generate API call report
    /// </summary>
    public string GenerateAPIReport()
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("\n=== API CALLS REPORT ===\n");
        report.AppendLine($"Total API Calls: {_networkCalls.Count}");
        report.AppendLine($"Successful Calls (2xx): {_networkCalls.Count(c => c.StatusCode >= 200 && c.StatusCode < 300)}");
        report.AppendLine($"Failed Calls (4xx/5xx): {_networkCalls.Count(c => c.StatusCode >= 400)}");
        report.AppendLine("\nDetailed Calls:");
        report.AppendLine(new string('-', 100));

        foreach (var call in _networkCalls.Where(c => c.URL.Contains("api") || c.URL.Contains("discover")))
        {
            report.AppendLine($"[{call.Timestamp:HH:mm:ss}] {call.Method} {call.URL}");
            report.AppendLine($"  Status: {call.StatusCode} | Duration: {call.Duration}ms");
            if (!string.IsNullOrEmpty(call.ResponseBody))
            {
                var preview = call.ResponseBody.Length > 150 ?
                    call.ResponseBody.Substring(0, 150) + "..." : call.ResponseBody;
                report.AppendLine($"  Response: {preview}");
            }
            report.AppendLine();
        }

        return report.ToString();
    }

    /// <summary>
    /// Clear captured calls
    /// </summary>
    public void ClearCalls()
    {
        _networkCalls.Clear();
    }
}
