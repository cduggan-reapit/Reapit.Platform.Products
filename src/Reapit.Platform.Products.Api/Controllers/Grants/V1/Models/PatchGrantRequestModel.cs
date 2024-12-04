namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;

/// <summary>Definition of the grant properties to update.</summary>
/// <param name="Scopes">The scopes allowed for this grant.</param>
public record PatchGrantRequestModel(ICollection<string> Scopes);