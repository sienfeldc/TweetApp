using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Model.Model.ViewModels;

public class postTweetViewModel
{
    [MaxLength(200)] [Required] public string TweetData { get; set; }
}