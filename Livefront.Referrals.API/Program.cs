using Livefront.Referrals.API.Configuration;
using Livefront.Referrals.DataAccess.Services;
using Serilog;

namespace Livefront.Referrals.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        AddNameHttpClients(builder);
        
        // Configure logging using Serilog Sinks for both the console and a local file
        // Note: We can explore other sinks in the future for visualization
        // This gets the configuration from app-settings.json
        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration)
        );
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        var app = builder.Build();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseSerilogRequestLogging();

        app.MapControllers();

        app.Run();
    }
    
    /// <summary>
    /// This function adds configuration for named HttpClients based on specified types.
    /// It pulls from the configuration and retrieves service needs
    /// Note: Secret properties will be pulled from other services and not from local configuration
    /// </summary>
    /// <param name="builder"> The host builder object to add the required clients</param>
    private static void AddNameHttpClients(WebApplicationBuilder builder)
    {
        var deeplinkApiConfig = builder.Configuration.GetSection("DeeplinkApi").Get<DeepLinkApiConfig>();
        builder.Services.AddHttpClient<ExternalDeeplinkApiService>( client =>
        {
            client.BaseAddress = new Uri(deeplinkApiConfig!.BaseAddress);
            client.DefaultRequestHeaders.Add("ApiToken", deeplinkApiConfig.ApiToken);
            client.DefaultRequestHeaders.Add("ApiTokenSecret", deeplinkApiConfig.ApiToken);
        });
    }
}