using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Examples;

/// <summary>Examples provider for the <see cref="ApplicationModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ApplicationModelExample : IExamplesProvider<ApplicationModel>
{
    private static readonly DateTime BaseDate = new(2024, 12, 2, 14, 33, 11, DateTimeKind.Utc);
    
    /// <inheritdoc />
    public ApplicationModel GetExamples()
        => new(
            Id: "07e6a3251da04abd8a8090055d6b37f7", 
            Name: "Custom Application", 
            IsFirstParty: true, 
            DateCreated: BaseDate, 
            DateModified: BaseDate.AddDays(3));
}