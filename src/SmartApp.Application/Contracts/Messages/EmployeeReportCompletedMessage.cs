namespace SmartApp.Application.Contracts.Messages;

public sealed record EmployeeReportCompletedMessage
{
    public Guid CorrelationId { get; init; }
    public bool IsSuccess { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public int EmployeeCount { get; init; }
    public DateTime CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
}