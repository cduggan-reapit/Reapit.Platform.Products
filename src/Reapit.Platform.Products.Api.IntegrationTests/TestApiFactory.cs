using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Api.IntegrationTests;

public class TestApiFactory : WebApplicationFactory<Program> 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Replace services
        builder.ConfigureServices(services =>
        {
            RemoveServiceForType(services, typeof(DbContextOptions<DemoDbContext>));
            
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });

            services.AddDbContext<DemoDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        builder.UseEnvironment("Development");
    }

    private static void RemoveServiceForType(IServiceCollection services, Type type)
    {
        var service = services.SingleOrDefault(s => s.ServiceType == type);
        
        if(service is not null)
            services.Remove(service);
    }
}