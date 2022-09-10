using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Model.Model.ViewModels;

public class UpdateTweet
{
    public int TweetID { get; set; }

    [MaxLength(200)] [Required] public string TweetData { get; set; }
}