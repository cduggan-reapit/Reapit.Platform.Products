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
        builder.Services.AddDbContext<DemoDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return builder; 
    }
}
