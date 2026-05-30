namespace SmartApp.Domain.Interfaces.Services;

using SmartApp.Domain.Entities;
using SmartApp.Shared.Common;

public interface IExcelReportService
{
    Task<Response<string>> GenerateEmployeeReportAsync(
        IReadOnlyList<Employee> employees,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}