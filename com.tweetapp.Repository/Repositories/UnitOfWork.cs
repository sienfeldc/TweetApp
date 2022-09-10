using com.tweetapp.Model.Context;
using com.tweetapp.Model.Model;
using com.tweetapp.Repository.Interfaces;
using com.tweetapp.Repository.Repository;

namespace com.tweetapp.Repository.Repositories;

/// <summary>
/// 
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly TweetAppDbContext _dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public UnitOfWork(TweetAppDbContext dbContext)
    {
        _dbContext = dbContext;

        User = new UserRepository<UserDetails>(_dbContext);
        Tweet = new TweetRepository<TweetDetails>(_dbContext);
    }

    /// <summary>
    /// User Repo
    /// </summary>
    public IUserRepository User { get; }
    /// <summary>
    /// Tweet Repo
    /// </summary>
    public ITweetRepository Tweet { get; }

    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }
}