using FluentValidation;
using Reapit.Platform.Common.Interfaces;

namespace Reapit.Platform.Products.Api.Exceptions;

/// <summary>ProblemDetail factory registrar for application exceptions.</summary>
public static class ExceptionRegistrar
{
    /// <summary>Register factory methods for exceptions defined in this project with the <see cref="IProblemDetailsFactory"/>.</summary>
    /// <param name="app">The service collection</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder RegisterExceptions(this IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetService<IProblemDetailsFactory>();
        
        if (factory is null)
            return app;
        
        factory.RegisterFactoryMethod<ValidationException>(ProblemDetailFactoryImplementations.GetValidationExceptionProblemDetails);

        return app;
    }
    
    
}