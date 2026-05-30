namespace SmartApp.API.Controllers.v1;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartApp.Application.Commands.GenerateEmployeeReport;
using SmartApp.Application.Settings;
using SmartApp.Shared.Common;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class EmployeesController(
    ISender sender,
    IOptions<ReportSettings> reportSettings
) : ControllerBase
{
    [HttpPost("reports")]
    [ProducesResponseType(typeof(Response<Guid>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestEmployeeReport(
        CancellationToken cancellationToken)
    {
        var command = new GenerateEmployeeReportCommand(
            RequestedBy: User.Identity?.Name ?? "anonymous",
            OutputDirectory: reportSettings.Value.OutputDirectory);

        var result = await sender.Send(command, cancellationToken);

        return result.isSuccess
            ? Accepted(result)
            : BadRequest(result);
    }
}