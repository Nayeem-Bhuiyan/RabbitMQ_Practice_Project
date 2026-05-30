namespace SmartApp.Application.Contracts.Messages;

public sealed record GenerateEmployeeReportMessage
{
    public Guid CorrelationId { get; init; }
    public string RequestedBy { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
    public string OutputDirectory { get; init; } = string.Empty;
}