using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Services.Notifications;

namespace Reapit.Platform.Products.Core;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCoreServices(this WebApplicationBuilder builder)
    {
        // // These can't reference static classes (like Startup) so just needs to point at any class in this assembly
        // builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UseCases.Products.CreateProduct.CreateProductCommandHandler>());
        
        // builder.Services.AddValidatorsFromAssemblyContaining(typeof(UseCases.Products.CreateProduct.CreateProductCommandValidator));

        builder.Services.AddScoped<INotificationsService, NotificationsService>();
        builder.Services.Configure<NotificationTopicConfiguration>(builder.Configuration.GetSection("Service:NotificationTopic"));
        
        builder.Services.Configure<IdentityProviderOptions>(builder.Configuration.GetSection("Service:Auth0"));
        
        return builder;
    }
}
