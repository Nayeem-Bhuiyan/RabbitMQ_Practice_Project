namespace SmartApp.Worker.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using SmartApp.Application.Contracts.Messages;
using SmartApp.Domain.Interfaces.Repositories;
using SmartApp.Domain.Interfaces.Services;

public sealed class GenerateEmployeeReportConsumer(
    IEmployeeRepository employeeRepository,
    IExcelReportService excelReportService,
    IPublishEndpoint publishEndpoint,
    ILogger<GenerateEmployeeReportConsumer> logger
) : IConsumer<GenerateEmployeeReportMessage>
{
    public async Task Consume(ConsumeContext<GenerateEmployeeReportMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Received report request. CorrelationId: {CorrelationId}, RequestedBy: {RequestedBy}",
            msg.CorrelationId, msg.RequestedBy);

        var employeesResult = await employeeRepository.GetAllActiveAsync(context.CancellationToken);

        if (!employeesResult.isSuccess)
        {
            logger.LogError(
                "Failed to fetch employees. CorrelationId: {CorrelationId}, Error: {Error}",
                msg.CorrelationId, employeesResult.message);

            await PublishCompletedAsync(msg.CorrelationId, false, string.Empty, 0, employeesResult.message, context.CancellationToken);
            return;
        }

        var reportResult = await excelReportService.GenerateEmployeeReportAsync(
            employeesResult.data!,
            msg.OutputDirectory,
            context.CancellationToken);

        if (reportResult.isSuccess)
        {
            logger.LogInformation(
                "Report generated. CorrelationId: {CorrelationId}, File: {FilePath}, Employees: {Count}",
                msg.CorrelationId, reportResult.data, employeesResult.data!.Count);
        }
        else
        {
            logger.LogError(
                "Report generation failed. CorrelationId: {CorrelationId}, Error: {Error}",
                msg.CorrelationId, reportResult.message);
        }

        await PublishCompletedAsync(
            msg.CorrelationId,
            reportResult.isSuccess,
            reportResult.data ?? string.Empty,
            employeesResult.data!.Count,
            reportResult.isSuccess ? null : reportResult.message,
            context.CancellationToken);
    }

    private async Task PublishCompletedAsync(
        Guid correlationId,
        bool isSuccess,
        string filePath,
        int employeeCount,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new EmployeeReportCompletedMessage
            {
                CorrelationId = correlationId,
                IsSuccess = isSuccess,
                FilePath = filePath,
                EmployeeCount = employeeCount,
                CompletedAt = DateTime.UtcNow,
                ErrorMessage = errorMessage
            },
            cancellationToken);
    }
}