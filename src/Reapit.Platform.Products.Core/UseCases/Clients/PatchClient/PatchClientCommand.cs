namespace Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;

/// <summary>Request to update a client.</summary>
/// <param name="Id">The unique identifier of the client.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Description">A description of the client.</param>
/// <param name="LoginUrl">The login uri of your application (optional, not supported by machine to machine clients).</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback after authentication (optional, not supported by machine to machine clients).</param>
/// <param name="SignOutUrls">Collection of URLs which are valid for redirect after logout (optional, not supported by machine to machine clients).</param>
public record PatchClientCommand(
    string Id,
    string? Name,
    string? Description,
    string? LoginUrl,
    ICollection<string>? CallbackUrls,
    ICollection<string>? SignOutUrls)
    : IRequest<Entities.Client>;
