namespace SmartApp.Application.Commands.GenerateEmployeeReport;

using MediatR;
using Microsoft.Extensions.Logging;
using SmartApp.Application.Abstractions;
using SmartApp.Application.Contracts.Messages;
using SmartApp.Shared.Common;

public sealed class GenerateEmployeeReportCommandHandler(
    IMessagePublisher messagePublisher,
    ILogger<GenerateEmployeeReportCommandHandler> logger
) : IRequestHandler<GenerateEmployeeReportCommand, Response<Guid>>
{
    public async Task<Response<Guid>> Handle(
        GenerateEmployeeReportCommand request,
        CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();

        var message = new GenerateEmployeeReportMessage
        {
            CorrelationId = correlationId,
            RequestedBy = request.RequestedBy,
            RequestedAt = DateTime.UtcNow,
            OutputDirectory = request.OutputDirectory
        };

        await messagePublisher.PublishAsync(message, cancellationToken);

        logger.LogInformation(
            "Employee report request published. CorrelationId: {CorrelationId}, RequestedBy: {RequestedBy}",
            correlationId, request.RequestedBy);

        return Response<Guid>.SuccessResponse(correlationId, "Report generation queued successfully.");
    }
}