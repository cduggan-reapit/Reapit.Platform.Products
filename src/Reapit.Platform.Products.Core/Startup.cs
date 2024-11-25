using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Common;

namespace Reapit.Platform.Products.Core;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCoreServices(this WebApplicationBuilder builder)
    {
        // // These can't reference static classes (like Startup) so just needs to point at any class in this assembly
        // builder.Services.AddMediatR(cfg => 
        //     cfg.RegisterServicesFromAssemblyContaining<UseCases.Dummies.CreateDummy.CreateDummyCommandHandler>());
        //
        // builder.Services.AddValidatorsFromAssemblyContaining(typeof(UseCases.Dummies.CreateDummy.CreateDummyCommandValidator));
        
        return builder;
    }
}
