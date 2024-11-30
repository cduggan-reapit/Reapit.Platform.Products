using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Api.IntegrationTests.TestServices;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;
using Reapit.Platform.Products.Core.Services.Notifications;
using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Api.IntegrationTests;

public class TestApiFactory : WebApplicationFactory<Program> 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Replace services
        builder.ConfigureServices(services =>
        {
            // Swap configured database out for an in-memory database
            RemoveServiceForType(services, typeof(DbContextOptions<ProductDbContext>));
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });
            services.AddDbContext<ProductDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
            
            // Swap live notifications service for the test service
            RemoveServiceForType(services, typeof(INotificationsService));
            services.AddSingleton<INotificationsService, MockNotificationsService>();
            
            // Swap live IdP services for the test service
            RemoveServiceForType(services, typeof(ITokenCache));
            RemoveServiceForType(services, typeof(IIdentityProviderClientFactory));
            RemoveServiceForType(services, typeof(IIdentityProviderService));
        });

        // Configuration isn't injected from SSM in development. We could mock that stuff if we wanted, but it's a bit
        // overkill imo.
        builder.UseEnvironment("Development");
    }

    private static void RemoveServiceForType(IServiceCollection services, Type type)
    {
        var service = services.SingleOrDefault(s => s.ServiceType == type);
        
        if(service is not null)
            services.Remove(service);
    }
}