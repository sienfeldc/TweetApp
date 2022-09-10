using System.Text;
using System.Text.Json.Serialization;
using com.tweetapp.Controller.KafkaProducer;
using com.tweetapp.Model.Context;
using com.tweetapp.Model.Model;
using com.tweetapp.Repository.Interfaces;
using com.tweetapp.Repository.Repositories;
using com.tweetapp.Repository.Repository;
using com.tweetapp.Services.Interfaces;
using com.tweetapp.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace com.tweetapp.Controller;

/// <summary>
/// Startup class for configuration
/// </summary>
public class Startup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    private IConfiguration Configuration { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// 
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(c =>
            {
                c.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                c.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        services.AddDbContext<TweetAppDbContext>(o =>
            o.UseSqlServer(Configuration.GetConnectionString("TweetAppDbConn"))
        );
        
        services.AddSingleton<IConfiguration>(Configuration);
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITweetService, TweetServices>();
        services.AddScoped<ITweetRepository, TweetRepository<TweetDetails>>();
        services.AddScoped<IUserRepository, UserRepository<UserDetails>>(); ;
        services.AddScoped<IKafkaProducer, KafkaProducer.KafkaProducer>();
        services.AddEndpointsApiExplorer();
        services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            // builder.WithOrigins("http://localhost:3001")
            //     .AllowAnyMethod()
            //     .AllowAnyHeader();
        }));


        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TWEET APP",
                Description = "Tweet API"
            });
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            
            // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // options.IncludeXmlComments(xmlPath);
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    /// <param name="dbContext"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TweetAppDbContext dbContext)
    {
        //app.UseMiddleware<ELKMiddleware>();
        if (env.IsDevelopment())
        {
            
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // dbContext.Database.EnsureCreated();
        // dbContext.Database.Migrate();
       
        dbContext.Database.SetCommandTimeout(160);
        dbContext.Database.GetPendingMigrationsAsync();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        
        // app.UseSwaggerUI(c =>
        //  {
        //      c.SwaggerEndpoint("/swagger/v1/swagger.json", "com.tweetapp.api v1");
        //      c.RoutePrefix = string.Empty;
        //  });
        
        app.UseHttpsRedirection();
        app.UseCors(builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        
    }
    
    public void ApplyMigrations(TweetAppDbContext context) {
        if (context.Database.GetPendingMigrations().Any()) {
            context.Database.Migrate();
        }
    }
}