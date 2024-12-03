namespace Reapit.Platform.Products.Core.UseCases.Clients.DeleteClient;

/// <summary>Request to delete a client.</summary>
/// <param name="Id">The unique identifier of the client.</param>
public record DeleteClientCommand(string Id) : IRequest<Entities.Client>;