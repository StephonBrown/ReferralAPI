using Livefront.Referrals.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Livefront.Referrals.FunctionalTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Make sure the application runs in development mode
        builder.UseEnvironment("Development");
        base.ConfigureWebHost(builder);
    }
}
