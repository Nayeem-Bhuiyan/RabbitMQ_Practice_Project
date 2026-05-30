namespace SmartApp.Infrastructure.Services;

using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SmartApp.Domain.Entities;
using SmartApp.Domain.Interfaces.Services;
using SmartApp.Shared.Common;

public sealed class ExcelReportService(ILogger<ExcelReportService> logger) : IExcelReportService
{
    private static readonly string[] Headers =
        ["#", "Full Name", "Email", "Department", "Position", "Salary (USD)", "Join Date", "Status"];

    private static readonly XLColor HeaderBg = XLColor.FromHtml("#1F3864");
    private static readonly XLColor SubHeaderBg = XLColor.FromHtml("#2E74B5");
    private static readonly XLColor RowEvenBg = XLColor.FromHtml("#DEEAF1");
    private static readonly XLColor SummaryBg = XLColor.FromHtml("#1F3864");
    private static readonly XLColor ActiveColor = XLColor.FromHtml("#1E8449");
    private static readonly XLColor InactiveColor = XLColor.FromHtml("#C0392B");

    public async Task<Response<string>> GenerateEmployeeReportAsync(
        IReadOnlyList<Employee> employees,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Directory.CreateDirectory(outputDirectory);
            var fileName = $"Employee_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(outputDirectory, fileName);

            await Task.Run(() => BuildWorkbook(employees, filePath), cancellationToken);

            logger.LogInformation(
                "Excel report generated successfully. Path: {FilePath}, EmployeeCount: {Count}",
                filePath, employees.Count);

            return Response<string>.SuccessResponse(filePath, "Excel report generated successfully.");
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Excel report generation was cancelled.");
            return Response<string>.Failure("Report generation was cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excel report generation failed.");
            return Response<string>.Failure($"Report generation failed: {ex.Message}");
        }
    }

    private static void BuildWorkbook(IReadOnlyList<Employee> employees, string filePath)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Employee Report");

        WriteTitle(ws);
        WriteHeaders(ws);
        WriteRows(ws, employees);
        WriteSummary(ws, employees.Count);
        ApplySheetFormatting(ws, employees.Count);

        workbook.SaveAs(filePath);
    }

    private static void WriteTitle(IXLWorksheet ws)
    {
        var titleRange = ws.Range("A1:H1");
        titleRange.Merge();
        titleRange.Value = "Employee Report";
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;
        titleRange.Style.Font.FontColor = XLColor.White;
        titleRange.Style.Fill.BackgroundColor = HeaderBg;
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Row(1).Height = 30;

        var subRange = ws.Range("A2:H2");
        subRange.Merge();
        subRange.Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
        subRange.Style.Font.Italic = true;
        subRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        subRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#D6DCE4");
    }

    private static void WriteHeaders(IXLWorksheet ws)
    {
        for (var i = 0; i < Headers.Length; i++)
        {
            var cell = ws.Cell(3, i + 1);
            cell.Value = Headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = SubHeaderBg;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
        ws.Row(3).Height = 20;
    }

    private static void WriteRows(IXLWorksheet ws, IReadOnlyList<Employee> employees)
    {
        for (var i = 0; i < employees.Count; i++)
        {
            var rowNum = i + 4;
            var emp = employees[i];
            var rowBg = i % 2 == 0 ? RowEvenBg : XLColor.White;

            ws.Cell(rowNum, 1).Value = i + 1;
            ws.Cell(rowNum, 2).Value = $"{emp.FirstName} {emp.LastName}";
            ws.Cell(rowNum, 3).Value = emp.Email;
            ws.Cell(rowNum, 4).Value = emp.Department;
            ws.Cell(rowNum, 5).Value = emp.Position;
            ws.Cell(rowNum, 6).Value = emp.Salary;
            ws.Cell(rowNum, 7).Value = emp.JoinedAt.ToString("yyyy-MM-dd");
            ws.Cell(rowNum, 8).Value = emp.IsActive ? "Active" : "Inactive";

            ws.Cell(rowNum, 6).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(rowNum, 8).Style.Font.FontColor = emp.IsActive ? ActiveColor : InactiveColor;
            ws.Cell(rowNum, 8).Style.Font.Bold = true;

            var rowRange = ws.Range(rowNum, 1, rowNum, Headers.Length);
            rowRange.Style.Fill.BackgroundColor = rowBg;
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
        }
    }

    private static void WriteSummary(IXLWorksheet ws, int employeeCount)
    {
        var summaryRow = employeeCount + 5;
        var labelCell = ws.Cell(summaryRow, 1);
        var valueCell = ws.Cell(summaryRow, 2);

        labelCell.Value = "Total Active Employees:";
        valueCell.Value = employeeCount;

        foreach (var cell in new[] { labelCell, valueCell })
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = SummaryBg;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        }
    }

    private static void ApplySheetFormatting(IXLWorksheet ws, int employeeCount)
    {
        ws.Column(1).Width = 5;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 32;
        ws.Column(4).Width = 18;
        ws.Column(5).Width = 22;
        ws.Column(6).Width = 16;
        ws.Column(7).Width = 14;
        ws.Column(8).Width = 12;

        ws.SheetView.FreezeRows(3);
        ws.Range(3, 1, employeeCount + 3, Headers.Length).SetAutoFilter();
    }
}