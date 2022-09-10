using System.Reflection;
using com.tweetapp.Model.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace com.tweetapp.Controller;

/// <summary>
/// Entry point
/// </summary>
public class Program
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        string tweetApptemplate = "myTweetApp";
        Console.WriteLine("Applying migrations");
        var app = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHostBuilder
                => webHostBuilder.ConfigureAppConfiguration((context, configuration) =>
                        _ = configuration.SetBasePath(context.HostingEnvironment.ContentRootPath)
                            .AddJsonFile("appsettings.json", false, true))
                    .UseStartup<Startup>())
            .UseSerilog((context, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(context.Configuration["ELKConfiguration:docker"]))
                        {
                            IndexFormat = $"app-logs-{context.Configuration["ELKConfiguration:index"]}-" +
                                          $"{context.Configuration["ASPNETCORE_ENVIRONMENT"].ToLower()}_{DateTime.UtcNow:yyyy-MM-dd}",
                            //IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name}-" +
                                         // $"{context.Configuration["ASPNETCORE_ENVIRONMENT"].ToLower()}_{DateTime.UtcNow:yyyy-MM-dd}",
                            AutoRegisterTemplate = true,
                            OverwriteTemplate = true,
                            TemplateName = tweetApptemplate,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                            TypeName = null,
                            BatchAction = ElasticOpType.Create
                        })
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .ReadFrom.Configuration(context.Configuration);
            })
            .Build();
        //CreateHostBuilder(args).Build().Run();
;
        app.Run();
    }

    // private static IHostBuilder CreateHostBuilder(string[] args)
    // {
    //     var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    //     Console.WriteLine(env);
    //     return Host.CreateDefaultBuilder(args)
    //             .ConfigureWebHostDefaults(webHostBuilder
    //             => webHostBuilder.ConfigureAppConfiguration((context, configuration) =>
    //                     _ = configuration.SetBasePath(context.HostingEnvironment.ContentRootPath)
    //                         .AddJsonFile("appsettings.json", false, true))
    //                 .UseStartup<Startup>())
    //             // .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
    //             .UseSerilog((context, configuration) =>
    //         {
    //             configuration
    //                 .Enrich.FromLogContext()
    //                 .Enrich.WithMachineName()
    //                 .WriteTo.Console()
    //                 .WriteTo.Elasticsearch(
    //                     new ElasticsearchSinkOptions(new Uri(context.Configuration["ELKConfiguration:docker"]))
    //                     {
    //                         IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name}-" +
    //                                       $"{context.Configuration["ASPNETCORE_ENVIRONMENT"].ToLower()}_{DateTime.UtcNow:yyyy-MM-dd}",
    //                         AutoRegisterTemplate = true
    //                     })
    //                 .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
    //                 .ReadFrom.Configuration(context.Configuration);
    //         })
    //         ;
    // }
}