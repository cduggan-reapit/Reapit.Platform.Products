using FluentValidation;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Common.Interfaces;
using Reapit.Platform.Products.Core.Exceptions;
using ConflictException = Reapit.Platform.Products.Core.Exceptions.ConflictException;

namespace Reapit.Platform.Products.Api.Infrastructure.Exceptions;

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
        
        // Third-party exceptions
        factory.RegisterFactoryMethod<ValidationException>(ProblemDetailFactoryImplementations.GetValidationExceptionProblemDetails);
        
        // App exceptions
        factory.RegisterFactoryMethod<NotFoundException>(NotFoundException.CreateProblemDetails);
        factory.RegisterFactoryMethod<ConflictException>(ConflictException.CreateProblemDetails);
        factory.RegisterFactoryMethod<QueryValidationException>(QueryValidationException.CreateProblemDetails);

        return app;
    }
    
    
}