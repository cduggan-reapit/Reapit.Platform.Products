namespace Reapit.Platform.Products.Core.UseCases.Applications.DeleteApplication;

/// <summary>Request to delete an application.</summary>
/// <param name="Id">The unique identifier of the application.</param>
public record DeleteApplicationCommand(string Id) : IRequest<Entities.App>;