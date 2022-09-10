using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.tweetapp.Model.Model;

public class UserDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    /// <summary>
    ///     User First name
    /// </summary>
    /// <param name=""></param>
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailId { get; set; }

    public DateTime DOB { get; set; }

    public string Password { get; set; }


    public bool IsLoggedIn { get; set; }

    public string Gender { get; set; }

    /// <summary>
    /// List of Tweets liked by user
    /// </summary>

    //public ICollection<TweetDetails>? Tweets { get; set; }

    //public ICollection<TweetLikes> TweetsLikedCollection { get; set; }
    /// <summary>
    ///     List of Replies by user
    /// </summary>
    //public ICollection<TweetReplies> RepliesCollection { get; set; }

    public string profileString { get; set; }
}