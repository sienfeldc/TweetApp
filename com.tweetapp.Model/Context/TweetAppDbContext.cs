using com.tweetapp.Model.Model;
using Microsoft.EntityFrameworkCore;

namespace com.tweetapp.Model.Context;

public class TweetAppDbContext : DbContext

{
    public TweetAppDbContext()
    {
    }

    public TweetAppDbContext(DbContextOptions<TweetAppDbContext> options) : base(options)
    {
    }

    public DbSet<UserDetails> UserDetails { get; set; }

    public DbSet<TweetDetails> Tweets { get; set; }

    public DbSet<TweetReplies> TweetReplies { get; set; }
    public DbSet<TweetLikes> TweetLikes { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //         optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TweetAppC2");
    // }
}