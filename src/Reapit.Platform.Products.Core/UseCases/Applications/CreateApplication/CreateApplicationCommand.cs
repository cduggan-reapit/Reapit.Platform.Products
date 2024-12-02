namespace Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;

/// <summary>Request to create a new application.</summary>
/// <param name="Name">The name of the application.</param>
/// <param name="Description">An optional description of the application.</param>
/// <param name="IsFirstParty">
/// Flag indicating whether clients for this application should be marked as first party clients. This cannot be changed
/// once the application has been created.
/// </param>
public record CreateApplicationCommand(string Name, string? Description, bool IsFirstParty) : IRequest<Entities.App>;