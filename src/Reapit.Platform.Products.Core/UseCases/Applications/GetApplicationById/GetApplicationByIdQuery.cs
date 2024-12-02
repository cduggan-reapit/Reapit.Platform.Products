namespace Reapit.Platform.Products.Core.UseCases.Applications.GetApplicationById;

/// <summary>Request to retrieve an application.</summary>
/// <param name="Id">The unique identifier of the application.</param>
public record GetApplicationByIdQuery(string Id) : IRequest<Entities.App>;