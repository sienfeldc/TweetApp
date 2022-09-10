using com.tweetapp.Model.Model;
using com.tweetapp.Repository.Repository;
using com.tweetapp.Services.Interfaces;
using com.tweetapp.Services.Services;

namespace com.tweetapp.Controller.Extensions;

public static class Dependency
{
    public static IServiceCollection AddMyDependencyGroup(this IServiceCollection services)
    {
        //services.AddScoped<IServices, UserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITweetService, TweetServices>();
        services.AddScoped<ITweetRepository, TweetRepository<TweetDetails>>();
        services.AddScoped<IUserRepository, UserRepository<UserDetails>>();


        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<, UserRepo>();
        // services.AddScoped<ITweetRepo, TweetRepo>();
        // services.AddScoped<ITweetService, TweetService>();
        // //services.AddTransient<ExceptionHandlerMiddleware>();

        return services;
    }
}