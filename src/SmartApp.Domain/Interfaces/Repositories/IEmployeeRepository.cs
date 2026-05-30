namespace SmartApp.Domain.Interfaces.Repositories;

using SmartApp.Domain.Entities;
using SmartApp.Shared.Common;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Response<IReadOnlyList<Employee>>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}