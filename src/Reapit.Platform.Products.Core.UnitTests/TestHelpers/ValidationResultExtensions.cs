using FluentValidation.Results;

namespace Reapit.Platform.Products.Core.UnitTests.TestHelpers;

public static class ValidationResultExtensions
{
    public static ValidationResultAssertions Should(this ValidationResult instance)
        => new(instance); 
}