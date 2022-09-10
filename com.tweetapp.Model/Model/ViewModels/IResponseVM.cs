namespace com.tweetapp.Model.Model.ViewModels;

public interface IResponseVM
{
    Exception Ex { get; set; }
    string Message { get; set; }
    bool Success { get; set; }
}