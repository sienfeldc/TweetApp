using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Repository;

namespace com.tweetapp.Repository.Interfaces;

/// <summary>
///     User repo interface
/// </summary>
public interface IUserRepository : IRepository<UserDetails>
{
    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<UserDetails> UserLogin(string emailId, string password);

    /// <summary>
    /// </summary>
    /// <param name="userDetails"></param>
    /// <returns></returns>
    Task<int> UserRegistration(UserRegistrationViewModel userDetails);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    Task<bool> UpdatePassword(string emailId, string newPassword);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="dob"></param>
    /// <returns></returns>
    Task<bool> ForgotPassword(string emailId, DateTime dob);

    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    Task<UserDetails> ValidateEmailId(string emailId);

    /// <summary>
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<UserDetails> GetUserProfile(string username);


    // public bool UserRegistration(UserDetails userDetails);
    // public void SetLoginStatus(string emailId, bool loggedIn);
    // public bool ForgetPassword(string user, DateTime dob, string pass);
    // public bool ResetPassword(string user, string oldpass, string pass);
    //
    // public bool DuplicateCheck(string email);
    //
    // public IEnumerable<UserDetails> GetAllUsers();

    void Update(UserDetails userDetails);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<IList<registeredUsers>> GetAllUsers();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userSearch"></param>
    /// <returns></returns>
    Task<IList<registeredUsers>> GetByUserName(string userSearch);
}