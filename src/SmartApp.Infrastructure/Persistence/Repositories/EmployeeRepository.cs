namespace SmartApp.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SmartApp.Domain.Entities;
using SmartApp.Domain.Interfaces.Repositories;
using SmartApp.Infrastructure.Persistence;
using SmartApp.Shared.Common;

public sealed class EmployeeRepository(AppDbContext context)
    : GenericRepository<Employee>(context), IEmployeeRepository
{
    public async Task<Response<IReadOnlyList<Employee>>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var employees = await DbSet
            .AsNoTracking()
            .Where(e => e.IsActive)
            .OrderBy(e => e.Department)
            .ThenBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

        return Response<IReadOnlyList<Employee>>.SuccessResponse(employees.AsReadOnly());
    }
}