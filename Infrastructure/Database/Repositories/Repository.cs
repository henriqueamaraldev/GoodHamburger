using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace Infrastructure.Database.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> FirstAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? query = null, CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbSet.Where(predicate);
        if (query != null) baseQuery = query(baseQuery);
        return await baseQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? query = null, CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbSet.Where(predicate);
        if (query != null) baseQuery = query(baseQuery);
        return await baseQuery.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        => _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
}
