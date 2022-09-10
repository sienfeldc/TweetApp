namespace com.tweetapp.Repository.ExceptionModels;

public class ErrorInfo
{
    public ErrorInfo(string message, List<Error> info = null)
    {
        Message = message;
        if (info != null) Info = info;
    }

    public string Message { get; set; }

    /// <summary>
    ///     Errors list
    /// </summary>
    public List<Error> Info { get; set; }
}