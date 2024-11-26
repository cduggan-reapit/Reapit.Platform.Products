﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Products.Core.Configuration;

namespace Reapit.Platform.Products.Core;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCoreServices(this WebApplicationBuilder builder)
    {
        // // These can't reference static classes (like Startup) so just needs to point at any class in this assembly
        builder.Services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblyContaining<UseCases.Products.CreateProduct.CreateProductCommandHandler>());
        
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(UseCases.Products.CreateProduct.CreateProductCommandValidator));
        
        builder.Services.Configure<NotificationTopicConfiguration>(builder.Configuration.GetSection("Service:NotificationTopic"));
        
        return builder;
    }
}
