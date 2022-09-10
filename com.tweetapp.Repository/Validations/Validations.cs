using System.Net;
using com.tweetapp.Repository.ExceptionModels;
using com.tweetapp.Repository.Exceptions;
using FluentValidation;

namespace com.tweetapp.Repository.Validations;

public class Validations
{
    public static void EnsureValidTweet<TRequest>(TRequest request, IValidator<TRequest> validator)
    {
        var validationError = new DomainExceptions("Invalid Request", HttpStatusCode.BadRequest);
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            validationError.Errors.AddRange(
                validationResult.Errors.Select(
                    validationFailure => new Error(validationFailure.ErrorMessage)
                ));
            throw validationError;
        }
    }
}