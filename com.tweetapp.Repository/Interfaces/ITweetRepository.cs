using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Repository;
#pragma warning disable CS1591

namespace com.tweetapp.Repository.Interfaces;

/// <summary>
/// </summary>
public interface ITweetRepository : IRepository<TweetDetails>
{
    /// <summary>
    ///     GetAllTweet
    /// </summary>
    /// <returns></returns>
    Task<List<TweetDetails>> GetAllTweets();


    /// <summary>
    /// </summary>
    /// <param name="tweet"></param>
    /// <param name="emailId"></param>
    /// <returns></returns>
    TweetDetails PostTweet(TweetDetails tweet, string emailId);


    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    Task<List<TweetDetails>> GetTweetByUserId(string emailId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userDetails"></param>
    /// <param name="tweetDetails"></param>
    /// <returns></returns>
    Task<int> UpdateTweet(UserDetails userDetails, UpdateTweet tweetDetails);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="tweetDetails"></param>
    /// <returns></returns>
    Task<bool> DeleteTweet(string emailId, TweetDetails tweetDetails);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="tweetID"></param>
    /// <returns></returns>
    Task<bool> DeleteTweetById(string emailId, string tweetID);

    /// <summary>
    /// </summary>
    /// <param name="tweetDetails"></param>
    void Update(TweetDetails tweetDetails);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="tweetDetails"></param>
    /// <returns></returns>
    Task<bool> TweetLike(string emailId, TweetDetails tweetDetails);
#pragma warning disable CS1591
    Task<TweetLikes> TweetLikeById(string emailId, string tweetId);
#pragma warning restore CS1591
    Task<int> TweetReply(string emailId, int Id, TweetMessage message);
    Task<TweetDetails> UpdateTweetById(string tweetId, UpdateTweet tweetDetails, string emailId);

    Task<getOneTweet> GetATweet(int id);
    public int GetLikesCount(int tweetId);

    Task<List<TweetReply>> GetReplies(int tweetId);
}