using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;
using Reapit.Platform.Products.Core.Services.Notifications;

namespace Reapit.Platform.Products.Core;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCoreServices(this WebApplicationBuilder builder)
    {
        // // These can't reference static classes (like Startup) so just needs to point at any class in this assembly
        builder.Services.AddMediatR(cfg 
            => cfg.RegisterServicesFromAssemblyContaining<UseCases.ResourceServers.CreateResourceServer.CreateResourceServerCommandHandler>());
        
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(UseCases.ResourceServers.CreateResourceServer.CreateResourceServerCommandValidator));

        builder.Services.AddScoped<INotificationsService, NotificationsService>();
        builder.Services.Configure<NotificationTopicConfiguration>(builder.Configuration.GetSection("Service:NotificationTopic"));

        builder.Services.AddSingleton<ITokenCache, TokenCache>();
        builder.Services.AddSingleton<IIdentityProviderClientFactory, IdentityProviderClientFactory>();
        builder.Services.AddScoped<IIdentityProviderService, IdentityProviderService>();
        
        builder.Services.Configure<IdentityProviderOptions>(builder.Configuration.GetSection("IdP"));
        
        return builder;
    }
}
