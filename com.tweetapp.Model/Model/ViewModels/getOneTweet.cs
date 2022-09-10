namespace com.tweetapp.Model.Model.ViewModels;

public class getOneTweet
{

    public TweetDetails? TweetDetails { get; set; }

    public ICollection<TweetReplies> Replies { get; set; }
    
    public ICollection<UserDetails> LikedBy { get; set; }
}