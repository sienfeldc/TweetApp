using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Model.Model.ViewModels;

public class userDetailsViewModel
{
    [Required(ErrorMessage = "First Name is mandatory")]
    public string FirstName { get; set; }

    public int UserId { get; set; }

    public string LastName { get; set; }

    [Required(ErrorMessage = "Email Id is mandatory")]
    public string EmailId { get; set; }

    [Required(ErrorMessage = "Date of Birth is mandatory")]
    public DateTime DOB { get; set; }

    [Required(ErrorMessage = "Password is mandatory")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public ICollection<TweetDetails>? Tweets { get; set; }

    public bool IsLoggedIn { get; set; }

    public string Gender { get; set; }
}