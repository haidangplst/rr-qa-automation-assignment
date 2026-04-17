using PlaywrightTests.Configuration;

namespace PlaywrightTests.Configuration;

/// <summary>
/// Represents a specific browser configuration (type + headless mode)
/// </summary>
public record BrowserConfiguration(
    BrowserType Type,
    bool Headless
)
{
    /// <summary>
    /// Get configuration name
    /// </summary>
    public override string ToString() =>
        $"{Type}_{(Headless ? "Headless" : "Headed")}";

    /// <summary>
    /// Get display name
    /// </summary>
    public string DisplayName =>
        $"{Type} ({(Headless ? "Headless" : "Headed")})";
}
