using System.Text;
using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Interfaces;
using com.tweetapp.Repository.Repository;
using com.tweetapp.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace com.tweetapp.Services.Services;

public class UserService : IUserService //<UserDetails>
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<IList<registeredUsers>> GetAllUsers()
    {
        try
        {
            var usrList = await _userRepository.GetAllUsers();
            return usrList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IList<registeredUsers>> GetByUserName(string search)
    {
        try
        {
            var userList = await _userRepository.GetByUserName(search);

            return userList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e + " User not found");
            throw;
        }
    }

    public async Task<UserDetails> LoginAuthentication(string emailId, string password)
    {
        try
        {
            if (password != null && emailId != null) password = EncryptPassword(password);

            var result = await _userRepository.UserLogin(emailId, password);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int> Register(UserRegistrationViewModel userDetails)
    {
        try
        {
            if (userDetails != null)
            {
                var returnM = -1;
                var validateEmail = await _userRepository.ValidateEmailId(userDetails.EmailId);

                if (validateEmail == null)
                {
                    userDetails.Password = EncryptPassword(userDetails.Password);
                    var result = await _userRepository.UserRegistration(userDetails);

                    if (result > 0)
                        return 1;
                    return 0;
                }

                if (validateEmail != null) return -10;
                return returnM;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e + "  Something went wrong");
            throw;
        }

        return -1;
    }

    public async Task<bool> ResetPassword(string username, string oldPassword, string newPassword)
    {
        var result = false;
        try
        {
            if (oldPassword != null)
            {
                oldPassword = EncryptPassword(oldPassword);
                var resp = await _userRepository.UserLogin(username, oldPassword);

                if (resp != null)
                {
                    newPassword = EncryptPassword(newPassword);
                    result = await _userRepository.UpdatePassword(username, newPassword);
                }
            }

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private string EncryptPassword(string password)
    {
        var message = string.Empty;
        var encode = new byte[password.Length];
        encode = Encoding.UTF8.GetBytes(password);
        message = Convert.ToBase64String(encode);
        return message;
    }
}