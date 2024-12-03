namespace Reapit.Platform.Products.Core.UseCases.Clients.GetClientById;

/// <summary>Request for a client.</summary>
/// <param name="Id">The unique identifier of the client.</param>
public record GetClientByIdQuery(string Id) : IRequest<Entities.Client>;