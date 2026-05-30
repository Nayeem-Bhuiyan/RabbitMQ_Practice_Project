namespace SmartApp.Domain.Interfaces.Repositories;

using SmartApp.Shared.Common;

public interface IGenericRepository<T> where T : class
{
    Task<Response<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<Response<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<int>> CountAsync(CancellationToken cancellationToken = default);
    Task<Response<IReadOnlyList<T>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}