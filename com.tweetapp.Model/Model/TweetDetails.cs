using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.tweetapp.Model.Model;

public class TweetDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TweetID { get; set; }

    [MaxLength(200)] [Required] public string TweetData { get; set; }

    public virtual UserDetails? User { get; set; }

    public DateTime TweetTime { get; set; }
}