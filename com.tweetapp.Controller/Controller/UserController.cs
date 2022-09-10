using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using com.tweetapp.Controller.KafkaProducer;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Exceptions;
using com.tweetapp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static com.tweetapp.Controller.KafkaProducer.KafkaProducer;


namespace com.tweetapp.Controller.Controller;

/// <summary>
/// Controller for all auth and user apis
/// </summary>
[Route("api/v1.0/tweets")]
[Authorize]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly string _bootstrap;
    private string topic;

    private readonly IKafkaProducer _kafkaProducer;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="logger"></param>
    /// <param name="config"></param>
    public UserController(IUserService userService, ILogger<UserController> logger, IConfiguration config, IKafkaProducer kafkaProducer)
    {
        _userService = userService;
        _logger = logger;
        _configuration = config;
        _kafkaProducer = kafkaProducer;
        var kafka = _configuration.GetSection("Kafka");
        _bootstrap = kafka["bootstrap"];
    }

    /// <summary>
    /// Get all users list
    /// </summary>
    /// <returns></returns>
    [Route("users/all")]
    [HttpGet]
    public ActionResult GetAllUser()
    {
        var result = _userService.GetAllUsers().Result;
        _logger.LogInformation("GetAll User Details", result);
        return Ok(result);
    }

    /// <summary>
    /// Search by partial username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [Route("user/search/{username}")]
    [HttpGet]
    public ActionResult SearchByUser(string username)
    {
        var result = _userService.GetByUserName(username).Result;
        return Ok(result);
    }


    /// <summary>
    /// Registration of new user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="DomainExceptions"></exception>
    [Route("register")]
    [AllowAnonymous]
    [HttpPost]
    public ActionResult Register([FromBody] UserRegistrationViewModel user)
    {
        _logger.LogInformation("Staring user Registration");
        if (user == null)
        {
            _logger.LogError("Error! Cannot register");
            throw new DomainExceptions("Invalid Request", HttpStatusCode.BadRequest);
        }
        var result = _userService.Register(user).Result;
        _kafkaProducer.Publish(_bootstrap, "Registration Successful. User Details: \n First name:"+user.FirstName+"\n Email Id:"+user.EmailId);
        return Ok(result);
    }
    
    /// <summary>
    /// Update password
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    [Route("{userName}/forgot")]
    [HttpPut]
    public ActionResult Forgot([FromRoute] string userName, [FromBody] UserPasswordUpdate update)
    {
        // UserLogin userLogin = new UserLogin
        // {
        //     EmailId =  userName,
        //     Password = oldPassword
        // };
        // string userName = update.EmailId;
        var oldPassword = update.OldPassword;
        var newPassword = update.NewPassword;

        var res = _userService.ResetPassword(userName, oldPassword, newPassword).Result;
        if (res) return Ok("Password Updated Successfully");
        return NotFound("Username/password doesn't match");
    }

    /// <summary>
    /// Login function that returns a token
    /// </summary>
    /// <param name="userData"></param>
    /// <returns></returns>
    [Route("login")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Token(UserLogin userData)
    {
        
        _kafkaProducer.Publish(_bootstrap,userData.EmailId +" is trying to login" );

        if (userData != null && userData.EmailId != null && userData.Password != null)
        {
            var user = _userService.LoginAuthentication(userData.EmailId, userData.Password).Result;

            if (user != null)
            {
                //create claims details based on the user information
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("DisplayName", user.FirstName + user.LastName),
                    new Claim("UserName", user.EmailId)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signIn);

                var ntoken = new JwtSecurityTokenHandler().WriteToken(token);
                var registeredUsers = new registeredUsers
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    profileString = user.profileString,
                    Username = user.EmailId
                };

                var res = new ResponseVM<registeredUsers>
                {
                    Token = ntoken,
                    Data = registeredUsers,
                    Success = true,
                    Message = "Successfully login"
                };

                _kafkaProducer.Publish(_bootstrap, res.Data.Username +" logged in Successfully. Name: "+res.Data.FirstName);
                
                return Ok(res);
            }

            return BadRequest("Invalid credentials");
        }

        return BadRequest();
    }
}