﻿using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Examples;

/// <summary>
/// Example provider for the <see cref="CreateProductRequestModel"/> type.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateProductRequestModelExampleProvider : IExamplesProvider<CreateProductRequestModel>
{
    /// <inheritdoc />
    public CreateProductRequestModel GetExamples()
        => new("My Product", "An example product with a bit of a silly name.");
}