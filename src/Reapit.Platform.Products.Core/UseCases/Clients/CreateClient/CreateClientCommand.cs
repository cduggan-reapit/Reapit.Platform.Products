using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;

/// <summary>Request to create a new client.</summary>
/// <param name="AppId">The unique identifier of the application with which the client is associated.</param>
/// <param name="Type">The type of client to create.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Description">A description of the client.</param>
/// <param name="LoginUrl">The login uri of your application (optional, not supported by machine to machine clients).</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback after authentication (optional, not supported by machine to machine clients).</param>
/// <param name="SignOutUrls">Collection of URLs which are valid for redirect after logout (optional, not supported by machine to machine clients).</param>
public record CreateClientCommand(
    string AppId,
    string Type,
    string Name,
    string? Description,
    string? LoginUrl,
    ICollection<string>? CallbackUrls,
    ICollection<string>? SignOutUrls)
    : IRequest<Entities.Client>;