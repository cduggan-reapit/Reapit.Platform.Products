using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Data;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddDataServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProductDbContext>(options =>
            options.UseMySql(
                connectionString: builder.Configuration.GetConnectionString("Writer"),
                serverVersion: new MySqlServerVersion(new Version(5, 31, 7)),
                mySqlOptionsAction: action =>
                {
                    action.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                }));
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return builder; 
    }
}
