// using com.tweetapp.Model.Model;
//
// namespace com.tweetapp.Repository.Validations
// {
//     using FluentValidation;
//
//     /// <summary>
//     /// LoginValidator class
//     /// </summary>
//     public class LoginValidator : AbstractValidator<UserDetails>
//     {
//         public LoginValidator(UserDetails userLogin)
//         {
//             RuleFor(x => x.EmailId)
//                 .Cascade(CascadeMode.Stop)
//                 .NotEmpty()
//                 .WithMessage("UserName cannot be blank.")
//                 .Length(0, 10)
//                 .WithMessage("User Name cannot be more than 10 characters.")
//                 .Must(val => FieldValidator.isValidAlphaNumeric(val))
//                 .WithMessage("User Name is alphanumeric");
//
//             RuleFor(x => x.Password)
//                 .Cascade(CascadeMode.Stop)
//                 .NotEmpty()
//                 .WithMessage("Password cannot be blank.")
//                 .Length(6, 100)
//                 .WithMessage("Password cannot be less then 6 characters.");
//         }
//     }
// }

