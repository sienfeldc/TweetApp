namespace com.tweetapp.Model.Model.ViewModels;

public class tweetLikes
{
    public TweetDetails TweetDetails { get; set; }

    public int Likes { get; set; }

    public List<TweetReply> Replies { get; set; }
}