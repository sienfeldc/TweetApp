using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.tweetapp.Model.Model;

public class TweetLikes
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public TweetDetails TweetDetails { get; set; }

    public UserDetails UserDetails { get; set; }

    public bool HasLiked { get; set; }
}