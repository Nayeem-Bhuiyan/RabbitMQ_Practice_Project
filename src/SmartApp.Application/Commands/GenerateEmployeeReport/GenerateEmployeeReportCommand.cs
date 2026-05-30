namespace SmartApp.Application.Commands.GenerateEmployeeReport;

using MediatR;
using SmartApp.Shared.Common;

public sealed record GenerateEmployeeReportCommand(
    string RequestedBy,
    string OutputDirectory
) : IRequest<Response<Guid>>;