using com.tweetapp.Repository.Interfaces;

namespace com.tweetapp.Repository.Repository;

public interface IUnitOfWork
{
    IUserRepository User { get; }
    ITweetRepository Tweet { get; }

    Task Save();
}