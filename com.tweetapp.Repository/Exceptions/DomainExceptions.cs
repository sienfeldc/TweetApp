using System.Net;
using com.tweetapp.Repository.ExceptionModels;

namespace com.tweetapp.Repository.Exceptions;

public class DomainExceptions : Exception
{
    public DomainExceptions(string message, HttpStatusCode httpStatusCode, List<Error> errors = null) : base(message)
    {
        ErrorMessage = message;
        HttpStatusCode = httpStatusCode;
        if (errors != null) Errors.AddRange(errors);
    }

    public string ErrorMessage { get; }
    public HttpStatusCode HttpStatusCode { get; }

    public List<Error> Errors { get; } = new();
}