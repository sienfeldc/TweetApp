namespace com.tweetapp.Model.Model.ViewModels;

public class ReplyResponse<T> : IResponseVM
{
    public int Status { get; set; }
    public T Data { get; set; }
    public Exception Ex { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}