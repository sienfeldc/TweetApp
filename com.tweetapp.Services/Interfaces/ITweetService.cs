using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;

namespace com.tweetapp.Services.Interfaces;

public interface ITweetService
{
    Task<TweetDetails> PostTweet(postTweetViewModel postTweet, string emailId);
    Task<List<TweetDetails>> GetAllTweets();

    Task<getOneTweet> GetATweet(int id);
    
    Task<List<TweetDetails>> GetTweetsByUser(string username);

    //bool DeleteTweet(TweetDetails tweet);
    bool DeleteTweetById(string tweetId, string emailId);
    TweetLikes LikeTweetById(string tweetId, string username);

    Task<TweetDetails> EditTweet(string emailId, string tweetId, UpdateTweet tweetDetails);
    Task<ReplyResponse<int>> ReplyTweet(string username, string id, TweetMessage message);

    Task<List<TweetReply>> GetTweetReply(int tweetId);
    int LikesCount(int Id);
}