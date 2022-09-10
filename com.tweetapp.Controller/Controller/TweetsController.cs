using System.Net;
using System.Text.Json;
using com.tweetapp.Controller.KafkaProducer;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Exceptions;
using com.tweetapp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;


namespace com.tweetapp.Controller.Controller;

/// <summary>
/// Controller for Tweet related functions
/// </summary>
[Route("api/v1.0/tweets")]
[Authorize]
[ApiController]

public class TweetsController : ControllerBase
{
    private readonly ILogger<TweetsController> _logger;
    private readonly ITweetService _tweetService;
    private readonly IConfiguration _configuration;
    // private string bootstrap = "localhost:9092";
    // private string topic = "TweetAppTopic";

    private string bootstrap;
    private string topic;
    
    private readonly IKafkaProducer _kafkaProducer;

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tweetService"></param>
    /// <param name="logger"></param>
    public TweetsController(ITweetService tweetService, ILogger<TweetsController> logger, IKafkaProducer kafkaProducer, IConfiguration configuration)
    {
        _tweetService = tweetService;
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        var kafka = _configuration.GetSection("Kafka");
        bootstrap = kafka["bootstrap"];
    }
    
    /// <summary>
    /// Get Tweets by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [Route("{username}")]
    [HttpGet]
    public ActionResult GetUserTweets(string username)
    {
        var result = _tweetService.GetTweetsByUser(username).Result;
        result.Reverse();
        _logger.LogInformation("GetTweetsByUsers", result);
        if (result != null)
        {
            var tweetJ = JsonSerializer.Serialize(result);
            //var tweets = JsonSerializer.Deserialize<List<TweetDetails>>(result);
            return Ok(tweetJ);
        }

        return NotFound();
    }


    /// <summary>
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [Route("all")]
    [HttpGet]
    public ActionResult GetAllTweets()
    {
        var result = _tweetService.GetAllTweets().Result;
        _logger.LogInformation("GetAllTweets", result);
        var listTweets = new List<tweetLikes>();

        foreach (var tweet in result)
        {
            var count = _tweetService.LikesCount(tweet.TweetID);
            var tweetReply = _tweetService.GetTweetReply(tweet.TweetID).Result;
            var tweetsLike = new tweetLikes
            {
                TweetDetails = tweet,
                Likes = count,
                Replies = tweetReply
            };
            listTweets.Add(tweetsLike);
            _logger.LogInformation("GetLikesCount", count);
            _logger.LogInformation("GetLikedTweet", tweetsLike);
        }
        //var tweetJ = JsonSerializer.Serialize(result);

        // var response = new ResponseVM<TweetDetails>
        // {
        //     Data = result,
        //     
        //         
        // }
        listTweets.Reverse();
        return Ok(listTweets);
    }

    /// <summary>
    /// Post a new Tweet
    /// </summary>
    /// <param name="username"></param>
    /// <param name="tweet"></param>
    /// <returns></returns>
    /// <exception cref="DomainExceptions"></exception>
    [Route("{username}/add")]
    [HttpPost]
    public ActionResult PostTweet(string username, [FromBody] postTweetViewModel tweet)
    {
        if (tweet == null) throw new DomainExceptions("Invalid Request", HttpStatusCode.BadRequest);
        var result = _tweetService.PostTweet(tweet, username).Result;
        _logger.LogInformation("PostTweet", result);
        
        _kafkaProducer.Publish(bootstrap, "\t\t"+result?.User?.FirstName+" posted a tweet. with message:"+result?.TweetData);
        return Ok(result);
    }


    /// <summary>
    /// Update/Edit an existing tweet
    /// </summary>
    /// <param name="username"></param>
    /// <param name="id"></param>
    /// <param name="tweet"></param>
    /// <returns></returns>
    /// <exception cref="DomainExceptions"></exception>
    [Route("{username}/update/{id}")]
    [HttpPut]
    public ActionResult UpdateTweet(string username, string id, [FromBody] UpdateTweet tweet)
    {
        if (tweet == null) throw new DomainExceptions("Invalid Request", HttpStatusCode.BadRequest);
        var result = _tweetService.EditTweet(username, id, tweet).Result;
        _logger.LogInformation("Update a Tweet", result);
        if (result != null) return Ok();

        _logger.LogInformation("Update a Tweet - Tweet not found Cannot update", result);
        return NotFound("Cannot update");
    }

    /// <summary>
    /// Like a tweet
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    [Route("{username}/like/{id}")]
    [HttpPut]
    public ActionResult LikeTweet(string id, string username)
    {
        var result = _tweetService.LikeTweetById(id, username);
        _logger.LogInformation("LikeTweet", result);
        if (result != null)
            return Ok(result);

        return Ok("cannot like a tweet twice");
    }

    /// <summary>
    /// Reply to a tweet
    /// </summary>
    /// <param name="username"></param>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="DomainExceptions"></exception>
    [Route("{username}/reply/{id}")]
    [HttpPost]
    public ActionResult ReplyTweet(string username, string id, [FromBody] TweetMessage message)
    {
        if (message == null) throw new DomainExceptions("Invalid Request", HttpStatusCode.BadRequest);
        var result = _tweetService.ReplyTweet(username, id, message).Result;
        _logger.LogInformation("ReplyTweet", result);
        return Ok(result);
    }

    /// <summary>
    /// Delete tweet
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    [Route("{username}/delete/{id}")]
    [HttpDelete]
    public ActionResult DeleteTweet(string id, string username)
    {
        _logger.LogInformation("Delete Tweet:", id, username);
        
        _kafkaProducer.Publish(bootstrap,"\t\t"+username+ " deleted the tweet with ID."+id);
        
        return Ok(_tweetService.DeleteTweetById(id, username));
    }


    [Route("details/{id}")]
    [HttpGet]
    public ActionResult GetATweeet(string id)
    {
        int tweetId = Int32.Parse(id);

        var res = _tweetService.GetATweet(tweetId);

        return Ok(res);
    }
}