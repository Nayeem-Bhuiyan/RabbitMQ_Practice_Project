namespace SmartApp.Application.Settings;

public sealed class ReportSettings
{
    public const string SectionName = "ReportSettings";
    public string OutputDirectory { get; init; } = Path.Combine(Path.GetTempPath(), "SmartApp", "Reports");
}