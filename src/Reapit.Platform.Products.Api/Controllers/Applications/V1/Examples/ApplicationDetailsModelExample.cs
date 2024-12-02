using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Examples;

/// <summary>Examples provider for the <see cref="ApplicationDetailsModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ApplicationDetailsModelExample : IExamplesProvider<ApplicationDetailsModel>
{
    private static readonly DateTime BaseDate = new(2024, 12, 2, 14, 52, 12, DateTimeKind.Utc);
    
    /// <inheritdoc />
    public ApplicationDetailsModel GetExamples()
        => new ApplicationDetailsModel(
            Id: "b0fc2503daaf4b6ebd7bafc15d5822d8", 
            Name: "Example App", 
            Description: "An example application with two clients.", 
            IsFirstParty: true,
            DateCreated: BaseDate,
            DateModified: BaseDate.AddHours(3),
            Clients: [
                new ApplicationClientModel(Id: "cdbdfaa0f8644093bea5a490ce3c6b6a", Name: "Example M2M Client", ClientType.Machine.Name),
                new ApplicationClientModel(Id: "9403ac7c56944df3a93065749c48234b", Name: "Example AuthCode Client", ClientType.AuthCode.Name)
            ]);
}