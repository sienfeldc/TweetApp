using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.tweetapp.Model.Model;

public class TweetReplies
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TweetId { get; set; }

    public virtual TweetDetails Tweet { get; set; }

    public virtual UserDetails UserDetails { get; set; }

    public string Comment { get; set; }

    public DateTime DateTime { get; set; }
}