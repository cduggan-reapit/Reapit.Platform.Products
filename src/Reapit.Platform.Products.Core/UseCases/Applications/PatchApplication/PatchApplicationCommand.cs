namespace Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;

/// <summary>Request to patch an application.</summary>
/// <param name="Id">The unique identifier of the application.</param>
/// <param name="Name">The name of the application.</param>
/// <param name="Description">A description of the application.</param>
public record PatchApplicationCommand(string Id, string? Name, string? Description) : IRequest<Entities.App>;