using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;

namespace com.tweetapp.Services.Interfaces;

public interface IUserService //<T> where T: class
{
    Task<IList<registeredUsers>> GetAllUsers();

    Task<int> Register(UserRegistrationViewModel userRegistrationViewModel);

    //Task<bool> DuplicateUserCheck(string username);
    Task<UserDetails> LoginAuthentication(string username, string password);
    Task<IList<registeredUsers>> GetByUserName(string username);

    Task<bool> ResetPassword(string username, string oldPassword, string newPassword);
    //Task<bool> ForgotPassword(string username, DateTime Dob);
    //void LogOut(string username);
}