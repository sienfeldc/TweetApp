using com.tweetapp.Model.Model.ViewModels;

public class ResponseVM<T> : IResponseVM
{
    public T Data { get; set; }
    public string? Token { get; set; }
    public int status { get; set; }
    public Exception Ex { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}