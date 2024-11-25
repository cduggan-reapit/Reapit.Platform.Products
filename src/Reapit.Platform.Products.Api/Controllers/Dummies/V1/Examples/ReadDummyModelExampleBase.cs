using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;

namespace Reapit.Platform.Products.Api.Controllers.Dummies.V1.Examples;

/// <summary>Static provider of an example <see cref="ReadDummyResponseModel"/> object.</summary>
[ExcludeFromCodeCoverage]
public static class ReadDummyModelExampleBase
{
    /// <summary>Creates an example <see cref="ReadDummyResponseModel"/> object.</summary>

    public static ReadDummyResponseModel GetExample()
        => new(
            Id: "851f3e46cc664149a066fc062dc0ed8c",
            Name: "Example Dummy",
            DateCreated: new DateTime(2020, 1, 12, 15, 47, 32),
            DateModified: new DateTime(2020, 1, 13));
}