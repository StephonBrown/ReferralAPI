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
using NSwag;
using NSwag.Generation.Processors.Security;
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
        builder.Services.AddOpenApiDocument(options =>
        {
            options.Title = "Livefront Referrals API";
            options.Version = "v1";
            options.Description = "API for managing referrals and referral links.";
            options.DocumentName = "Livefront.Referrals.API";
            
            // Adds the ability to pass a bearer token in the header for authentication
            options.OperationProcessors.Add(new OperationSecurityScopeProcessor("Auth"));
            options.DocumentProcessors.Add(new SecurityDefinitionAppender("Auth", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                In = OpenApiSecurityApiKeyLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "jwt"
            }));
        });
        
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        
        //This sets up JWT configuration for our authentication for local development
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

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            // This is a hack to get the database seeded in development
            CreateNewDatabase(app.Services);
            
            app.UseOpenApi();
            app.UseSwaggerUi(); 
        }
        
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
        builder.Services.AddHttpClient<ExternalDeeplinkApiServiceWrapper>( client =>
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
    
    /// <summary>
    ///  This is a bit of a hack to get the database seeded
    /// It is not recommended to use this in production
    /// </summary>
    /// <param name="serviceProvider"></param>
    private static void CreateNewDatabase(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ReferralsContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            SeedTestUser(context);
        }
    }
    private static void SeedTestUser(DbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context
            .Set<User>()
            .AddRange(GetTestUser());
        context.SaveChanges();
    }


    private static IEnumerable<User> GetTestUser()
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
        };
    }


}