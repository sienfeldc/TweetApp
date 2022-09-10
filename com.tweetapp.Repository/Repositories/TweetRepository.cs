using System.Net;
using com.tweetapp.Model.Context;
using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Exceptions;
using com.tweetapp.Repository.Interfaces;
using com.tweetapp.Repository.Repository;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8603

#pragma warning disable CS8602

namespace com.tweetapp.Repository.Repositories;

/// <summary>
/// </summary>
public class TweetRepository<T> : Repository<TweetDetails>, ITweetRepository
{
    private readonly TweetAppDbContext _dbContext;

    /// <summary>
    /// </summary>
    /// <param name="dbContext"></param>
    public TweetRepository(TweetAppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// </summary>
    /// <param name="tweetDetails"></param>
    public void Update(TweetDetails tweetDetails)
    {
        _dbContext.Tweets.Update(tweetDetails);
    }

    /// <summary>
    /// </summary>
    /// <param name="tweetDetails"></param>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public TweetDetails PostTweet(TweetDetails tweetDetails, string emailId)
    {
        try
        {
            var userDetails = _dbContext.UserDetails.FirstOrDefault(u => u.EmailId == emailId);
            tweetDetails.User = userDetails;
            _dbContext.Tweets.Add(tweetDetails);
            var res = _dbContext.SaveChangesAsync();
            if (res.Result > 0) return tweetDetails;
        }
        catch (Exception er)
        {
            return null;
        }

        return null;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public async Task<List<TweetDetails>> GetAllTweets()
    {
        var tweetAll = await _dbContext.Tweets.Include(x => x.User).ToListAsync();

        var tweetList = new List<UserTweets>();
        foreach (var tweet in tweetAll)
        {
            var usr = tweet.User;
            var count = _dbContext.TweetLikes.Count(t =>
                t.TweetDetails.TweetID == tweet.TweetID && t.UserDetails.UserId == usr.UserId && t.HasLiked == true);
            var tL = new UserTweets
            {
                EmailID = tweet.User.EmailId,
                RegisteredUsers = new registeredUsers
                {
                    profileString = usr.profileString,
                    FirstName = usr.FirstName,
                    LastName = usr.LastName
                },
                Likes = count
            };
        }
        // var result = await (from tweet in _dbContext.Tweets join user in _dbContext.UserDetails on tweet.User equals user.UserId select new TweetDetails { User = user, Tweets = tweet.Tweets, Imagename = user.ImageName, TweetDate = tweet.TweetDate, FirstName = user.FirstName, LastName = user.LastName,Likes=tweet.Likes }).ToListAsync();

        // var tweetAll = await (this._dbContext.Tweets).Include(x=> new UserDetails
        // {
        //     FirstName = x.User.FirstName,
        //     EmailId = x.User.EmailId,
        //     IsLoggedIn = x.User.IsLoggedIn,
        //     profileString = x.User.profileString
        // }).ToListAsync();
        return tweetAll;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userDetails"></param>
    /// <param name="updateTweet"></param>
    /// <returns></returns>
    public async Task<int> UpdateTweet(UserDetails userDetails, UpdateTweet updateTweet)
    {
        var tweet = await _dbContext.Tweets.FirstOrDefaultAsync(t =>
            t.TweetID == updateTweet.TweetID && t.User == userDetails);

        if (tweet == null) return -1;
        tweet.TweetData = updateTweet.TweetData;

        _dbContext.Tweets.Update(tweet);

        return 1;

    }

    public async Task<TweetDetails> UpdateTweetById(string tweetId, UpdateTweet tweetUpdate, string emailId)
    {
        var tweet = _dbContext.Tweets.Where(x => x.TweetID == int.Parse(tweetId)).FirstOrDefaultAsync().Result;
        if (tweet == null) throw new DomainExceptions("Id is invalid", HttpStatusCode.BadRequest);

        tweet.TweetData = tweetUpdate.TweetData;

        //_dbContext.Tweets.Update(tweet);
        var res = _dbContext.SaveChangesAsync();

        if (res.Id > 1) return tweet;

        return null;
    }

    public async Task<bool> DeleteTweet(string emailId, TweetDetails tweetDetails)
    {
        var res = await _dbContext.Tweets.Where(u => u.User.EmailId == emailId && u.TweetID == tweetDetails.TweetID)
            .FirstOrDefaultAsync();
        if (res != null)
        {
            _dbContext.Tweets.Remove(res);
            var response = await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteTweetById(string emailId, string id)
    {
        var res = await GetTweetById(id);
        if (res != null)
        {
            _dbContext.Tweets.Remove(res);
            var response = await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tweetId"></param>
    /// <returns></returns>
    public async Task<List<TweetReply>> GetReplies(int tweetId)
    {
        var replies = await _dbContext.TweetReplies.Where(r => r.TweetId == tweetId).ToListAsync();

        var replyList = new List<TweetReply>();
        foreach (var reply in replies)
        {
            var user = new registeredUsers
            {
                FirstName = reply.UserDetails.FirstName,
                LastName = reply.UserDetails.LastName,
                profileString = null,
                Username = reply.UserDetails.EmailId
            };


            var replyNew = new TweetReply
            {
                Comment = reply.Comment,
                DateTime = reply.DateTime,
                Id = reply.Id,
                TweetId = reply.TweetId,
                Users = user
            };

            replyList.Add(replyNew);
        }

        return replyList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="Id"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<int> TweetReply(string emailId, int Id, TweetMessage message)
    {
        var reply = new TweetReplies();
        var results = 0;


        var user = await _dbContext.UserDetails.Where(u => u.EmailId == emailId).FirstOrDefaultAsync();
        // var result = await _dbContext.Tweets.Where(t => t.TweetID == tweetDetails.TweetID).FirstOrDefaultAsync();
        var result = await _dbContext.Tweets.Where(t => t.TweetID == Id).FirstOrDefaultAsync();


        if (result != null && user != null)
        {
            reply.Comment = message.TweetData;
            reply.TweetId = result.TweetID;
            reply.Tweet = result;
            reply.UserDetails = user;
            reply.DateTime = message.TweetTime;

            _dbContext.TweetReplies.Add(reply);

            results = await _dbContext.SaveChangesAsync();
        }

        return results;
    }

    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public async Task<List<TweetDetails>> GetTweetByUserId(string emailId)
    {
        var user = await _dbContext.UserDetails.FirstOrDefaultAsync(e => e.EmailId == emailId);
        var userTweets = await _dbContext.Tweets.Include(u => u.User).Where(t => t.User == user).ToListAsync();
        return userTweets;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="tweetDetails"></param>
    /// <returns></returns>
    public async Task<bool> TweetLike(string emailId, TweetDetails tweetDetails)
    {
        var like = new TweetLikes();
        var user = await _dbContext.UserDetails.FirstOrDefaultAsync(u => u.EmailId == emailId);
        like.TweetDetails = tweetDetails;
        like.UserDetails = user;
        like.HasLiked = true;
        _dbContext.TweetLikes.Add(like);
        var res = await _dbContext.SaveChangesAsync();

        if (res > 0) return true;

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="tweetID"></param>
    /// <returns></returns>
    /// <exception cref="DomainExceptions"></exception>
    public async Task<TweetLikes> TweetLikeById(string emailId, string tweetID)
    {
        var tweetDetails = GetTweetById(tweetID).Result;
        var like = new TweetLikes();
        var res = 0;
        var user = await _dbContext.UserDetails.FirstOrDefaultAsync(u => u.EmailId == emailId);

        if (tweetDetails == null || user == null)
            throw new DomainExceptions("Id is invalid", HttpStatusCode.BadRequest);

        var alreadyLiked = await _dbContext.TweetLikes.Where(t =>
            t.HasLiked == true && t.TweetDetails == tweetDetails && t.UserDetails == user).FirstOrDefaultAsync();

        if (alreadyLiked == null)
        {
            like.TweetDetails = tweetDetails;
            like.UserDetails = user;
            like.HasLiked = true;
            _dbContext.TweetLikes.Add(like);
            res = await _dbContext.SaveChangesAsync();
        }

        return res > 0 ? like : null;
    }

    /// <summary>
    /// Get count of likes
    /// </summary>
    /// <param name="tweetId"></param>
    /// <returns></returns>
    public int GetLikesCount(int tweetId)
    {
        var count = _dbContext.TweetLikes.Count(t => t.HasLiked == true && t.TweetDetails.TweetID == tweetId);
        return count;
    }

    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public UserDetails GetUser(string emailId)
    {
        var userDetails = new UserDetails();
        try
        {
            userDetails = _dbContext.UserDetails.FirstOrDefault(u => u.EmailId == emailId);
            return userDetails;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public int GetUserId(string emailId)
    {
        try
        {
            var userDetails = new UserDetails();
            userDetails = _dbContext.UserDetails.FirstOrDefault(u => u.EmailId == emailId);
            return userDetails.UserId;
        }

        catch
        {
            return -1;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<TweetDetails> GetTweetById(string id)
    {
        var tweetId = int.Parse(id);
        var tweet = await _dbContext.Tweets.Where(t => t.TweetID == tweetId).FirstOrDefaultAsync();

        return tweet;
    }

    public async Task<getOneTweet> GetATweet(int id)
    {
        var tweet = await _dbContext.Tweets.FirstOrDefaultAsync(x => x.TweetID == id);
        var likes =  _dbContext.TweetLikes.Where(x => x.TweetDetails.TweetID == id).Include(u => u.UserDetails).ToList();
        var repliesOnTweet = _dbContext.TweetReplies.Where(x => x.TweetId == id).Include(u => u.UserDetails).ToList();

        var likedByUsers = new List<UserDetails>();
        
        foreach (var like in likes)
        {
            likedByUsers.Add(like.UserDetails);
        }
        
        
        
        var tweetDetail = new getOneTweet()
        {
            LikedBy = likedByUsers,
            Replies = repliesOnTweet,
            TweetDetails = tweet,

        };

        return tweetDetail;
    }
}