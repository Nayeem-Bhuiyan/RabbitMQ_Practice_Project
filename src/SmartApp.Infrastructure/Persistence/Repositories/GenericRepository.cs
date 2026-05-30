namespace SmartApp.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SmartApp.Domain.Interfaces.Repositories;
using SmartApp.Infrastructure.Persistence;
using SmartApp.Shared.Common;

public abstract class GenericRepository<T>(AppDbContext context) : IGenericRepository<T>
    where T : class
{
    protected readonly AppDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<Response<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        return entity is null
            ? Response<T>.Failure($"{typeof(T).Name} '{id}' not found.")
            : Response<T>.SuccessResponse(entity);
    }

    public async Task<Response<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return Response<T>.SuccessResponse(entity);
    }

    public async Task<Response<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return Response<T>.SuccessResponse(entity);
    }

    public async Task<Response<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        if (entity is null)
            return Response<bool>.Failure($"{typeof(T).Name} '{id}' not found.");

        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return Response<bool>.SuccessResponse(true);
    }

    public async Task<Response<int>> CountAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.CountAsync(cancellationToken);
        return Response<int>.SuccessResponse(count);
    }

    public async Task<Response<IReadOnlyList<T>>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var items = await DbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Response<IReadOnlyList<T>>.SuccessResponse(items.AsReadOnly());
    }
}