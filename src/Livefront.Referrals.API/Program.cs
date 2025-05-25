using System.Text;
using Livefront.BusinessLogic.Services;
using Livefront.Referrals.API.Configuration;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        ConfigureServices(builder.Services);
        // Configure logging using Serilog Sinks for both the console and a local file
        // Note: We can explore other sinks in the future for visualization
        // This gets the configuration from app-settings.json
        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration)
        );
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        
        //This sets up JWT configuration for our authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
                };
            });
        
        var app = builder.Build();
        CreateNewDatase(app.Services);
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSerilogRequestLogging();
        app.UseCors();
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
    
    private static void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddDbContext<ReferralsContext>(options =>
        {
            // Seeding for both sync and async is recommended since some EF tooling uses sync still
            options.UseSqlite("Data Source=Referrals.db");
        });
        services.AddScoped<IReferralRepository, ReferralRepository>();
        services.AddScoped<IReferralLinkRepository, ReferralLinkRepository>();
        services.AddScoped<IUserRepository, TestUserRepository>();
        services.AddScoped<IReferralService, ReferralService>();
        services.AddScoped<IReferralLinkService, ReferralLinkService>();
        services.AddScoped<IExternalDeeplinkApiService, MockDeeplinkApiService>();
    }
    
    private static void Seed(DbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context
            .Set<User>()
            .AddRange(GetUsers());
        context.SaveChanges();
    }


    private static IEnumerable<User> GetUsers()
    {
        return new[]
        {
            new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@email.com",
                ReferralCode = "TESTCODE"
            },
            new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "John@email.com",
                ReferralCode = "JOHNCODE"
            },
            new User
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "Jane@email.com",
                ReferralCode = "JANECODE"
            }
        };
    }

    private static void CreateNewDatase(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ReferralsContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Seed(context);
        }
    }
}