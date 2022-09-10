namespace com.tweetapp.Repository.ExceptionModels;

public class Error
{
    public Error(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}