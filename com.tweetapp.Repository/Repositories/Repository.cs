using System.Linq.Expressions;
using com.tweetapp.Model.Context;
using com.tweetapp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.tweetapp.Repository.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly TweetAppDbContext _dbContext;
    internal DbSet<T> _dbSet;

    public Repository(TweetAppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null) query = query.Where(filter);
        if (includeProperties != null)
            foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProp);
        return await query.ToListAsync();
    }
#nullable disable
    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string includeProperties = null)
    {
        IQueryable<T> query = _dbSet;

        query = query.Where(filter);
        if (includeProperties != null)
            foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProp);
        return await query.FirstOrDefaultAsync();
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}