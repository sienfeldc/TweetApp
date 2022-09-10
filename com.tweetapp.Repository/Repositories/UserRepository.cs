using com.tweetapp.Model.Context;
using com.tweetapp.Model.Model;
using com.tweetapp.Model.Model.ViewModels;
using com.tweetapp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8603

namespace com.tweetapp.Repository.Repository;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public class UserRepository<T> : Repository<UserDetails>, IUserRepository
{
    private readonly TweetAppDbContext _dbContext;
    private readonly Repository<UserDetails> _userRepo;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public UserRepository(TweetAppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        //this.dbContext = new TweetAppContext(new DbContextOptions<TweetAppContext>());
    }

    /// <summary>
    /// </summary>
    /// <param name="userDetails"></param>
    public void Update(UserDetails userDetails)
    {
        _dbContext.UserDetails.Update(userDetails);
    }

    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<UserDetails> UserLogin(string emailId, string password)
    {
        var userDetails =
            await _dbContext.UserDetails.Where(e => e.EmailId == emailId && e.Password == password)
                .FirstOrDefaultAsync();
        if (userDetails != null)
            // UserDetails logUser = new UserDetails
            // {
            //     FirstName = userDetails.FirstName,
            //     LastName = userDetails.LastName,
            //     DOB = userDetails.DOB,
            //     EmailId = userDetails.EmailId,
            //     Gender = userDetails.Gender,
            //     IsLoggedIn = true,
            //     profileString = userDetails.profileString,
            // };
            return userDetails;
#pragma warning disable CS8603
        return null;
#pragma warning restore CS8603
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userRegDetails"></param>
    /// <returns></returns>
    public async Task<int> UserRegistration(UserRegistrationViewModel userRegDetails)
    {
        var userDetails = new UserDetails
        {
            FirstName = userRegDetails.FirstName,
            LastName = userRegDetails.LastName,
            Password = userRegDetails.Password,
            EmailId = userRegDetails.EmailId,
            Gender = userRegDetails.Gender,
            profileString = userRegDetails.profileString,
            DOB = userRegDetails.DOB
        };

        _dbContext.UserDetails.Add(userDetails);
        var result = await _dbContext.SaveChangesAsync();
        return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<bool> UpdatePassword(string emailId, string newPassword)
    {
        var update = await _dbContext.UserDetails.Where(x => x.EmailId == emailId).FirstOrDefaultAsync();
        if (update != null)
        {
            update.Password = newPassword;
            _dbContext.UserDetails.Update(update);
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0) return true;
        }

        return false;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailId"></param>
    /// <param name="dob"></param>
    /// <returns></returns>
    public async Task<bool> ForgotPassword(string emailId, DateTime dob)
    {
        var result = await _dbContext.UserDetails.Where(s => s.EmailId == emailId).FirstOrDefaultAsync();
        if (result == null) return false;
        if (result.DOB == dob) result.Password = "12345";
        _dbContext.Update(result);
        var response = await _dbContext.SaveChangesAsync();
        if (response > 0) return true;

        return false;
    }

    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public async Task<UserDetails> GetUserProfile(string emailId)
    {
        var result = await _dbContext.UserDetails.Where(u => u.EmailId == emailId).FirstOrDefaultAsync();
        return result;
    }


    /// <summary>
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public async Task<UserDetails> ValidateEmailId(string emailId)
    {
        var user = await _dbContext.UserDetails.FirstOrDefaultAsync(e => e.EmailId == emailId);
        return user;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public async Task<IList<registeredUsers>> GetAllUsers()
    {
        var res = await _dbContext.UserDetails.Select(p => new registeredUsers
        {
            FirstName = p.FirstName,
            LastName = p.LastName,
            Username = p.EmailId,
            profileString = p.profileString
        }).ToListAsync();
        return res;
    }


    /// <summary>
    /// Search user by name/email
    /// </summary>
    /// <param name="userSearch"></param>
    /// <returns></returns>
    public async Task<IList<registeredUsers>> GetByUserName(string userSearch)
    {
        var result =
            await (from u in _dbContext.UserDetails where u.EmailId.Contains(userSearch) select u).ToListAsync();

        var retList = new List<registeredUsers>();

        foreach (var res in result)
        {
            var retRes = new registeredUsers
            {
                FirstName = res.FirstName,
                LastName = res.LastName,
                Username = res.EmailId,
                profileString = res.profileString
            };

            retList.Add(retRes);
        }

        return retList;
    }


    // public bool UserLogin(string emailId, string password)
    // {
    //     var userRegistrationDetails = _dbContext.UserDetails.FirstOrDefault(s => s.EmailId == emailId && s.Password == password);
    //     if (userRegistrationDetails != null)
    //     {
    //         SetLoginStatus(emailId,true);
    //         return true;
    //     }
    //     return false;
    // }
    //
    // public void SetLoginStatus(string emailId, bool loggedIn)
    // {
    //     var user = _dbContext.UserDetails.FirstOrDefault(u => u.EmailId == emailId);
    //     user.IsLoggedIn = loggedIn;
    //     _dbContext.Update(user);
    //     _dbContext.SaveChanges();
    // }
    //
    // public bool UserRegistration(UserDetails userDetails)
    // {
    //     try
    //     {
    //         _dbContext.UserDetails.Add(userDetails);
    //         _dbContext.SaveChanges();
    //         SetLoginStatus(userDetails.EmailId,true);
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         return false;
    //     }
    // }
    //
    // public bool ForgetPassword(string user, DateTime dob, string pass)
    // {
    //     var result = _dbContext.UserDetails.Where(x => x.EmailId == user && x.DOB == dob).FirstOrDefault();
    //     if (result != null)
    //     {
    //         result.Password = pass;
    //         _dbContext.SaveChanges();
    //         return true;
    //     }
    //
    //     return false;
    // }
    //
    // public bool ResetPassword(string user, string oldpass, string pass)
    // {
    //     var result = _dbContext.UserDetails.Where(x => x.EmailId == user && x.Password == oldpass).FirstOrDefault();
    //     if (result != null)
    //     {
    //         result.Password = pass;
    //         _dbContext.SaveChanges();
    //         return true;
    //     }
    //
    //     return false;
    // }
    //
    // public bool DuplicateCheck(string emailId)
    // {
    //     var res = _dbContext.UserDetails.FirstOrDefault(x => x.EmailId == emailId);
    //
    //     if (res != null)
    //         return true;
    //
    //     return false;
    //
    // }
    //
    // public async Task<IEnumerable<T>> GetAllUsersAsync(Expression<Func<T,bool>>? filter = null, string? inProps = null)
    // {
    //     IQueryable<T> queryable = _dbSet;
    //     if (filter != null)
    //     {
    //         queryable = queryable.Where(filter);
    //     }
    //
    //     if (inProps != null)
    //     {
    //         foreach (var inProp in inProps.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
    //         {
    //             queryable = queryable.Include(inProp);
    //         }
    //     } 
    //     
    //     return await queryable.ToListAsync();
    // }
    //
}