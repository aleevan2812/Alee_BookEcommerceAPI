using System.Linq.Expressions;
using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Alee_BookEcommerceAPI.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }

    public async Task CreateAsync(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true,
        string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (!tracked) query = query.AsNoTracking();

        if (filter != null) query = query.Where(filter);

        if (includeProperties != null)
            foreach (var includeProp in includeProperties.Split(new[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProp);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
        int pageSize = 0, int pageNumber = 1)
    {
        IQueryable<T> query = dbSet;

        if (filter != null) query = query.Where(filter);

        if (pageSize > 0)
        {
            if (pageSize > 100) pageSize = 100;

            //skip0.take(5)
            //page number- 2     || page size -5
            //skip(5*(1)) take(5)
            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }

        if (includeProperties != null)
            foreach (var includeProp in includeProperties.Split(new[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProp);

        return await query.ToListAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        dbSet.Remove(entity);
    }

    public async Task RemoveRangeAsync(IEnumerable<T> entity)
    {
        dbSet.RemoveRange();
    }

    
}