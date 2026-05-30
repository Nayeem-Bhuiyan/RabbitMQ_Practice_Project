namespace SmartApp.Application.DTOs;

public sealed record EmployeeDto(
    Guid Id,
    string FullName,
    string Email,
    string Department,
    string Position,
    decimal Salary,
    DateTime JoinedAt,
    bool IsActive
);