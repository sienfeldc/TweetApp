namespace com.tweetapp.Model.Model.ViewModels;

public class TweetReply
{
    public int Id { get; set; }

    public int TweetId { get; set; }

    public string Comment { get; set; }

    public DateTime DateTime { get; set; }

    public registeredUsers Users { get; set; }
}