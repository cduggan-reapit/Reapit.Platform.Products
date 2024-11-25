using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Api.Exceptions;

namespace Reapit.Platform.Products.Api.UnitTests.Exceptions;

public class ProblemDetailFactoryImplementationsTests
{
    [Fact]
    public void GetValidationExceptionProblemDetails_ThrowsException_WhenNotValidationException()
    {
        var exception = new Exception();
        var action = () => ProblemDetailFactoryImplementations.GetValidationExceptionProblemDetails(exception);
        action.Should().Throw<ProblemDetailsFactoryException>();
    }
    
    [Fact]
    public void GetValidationExceptionProblemDetails_ReturnsProblemDetails_WithMappedErrorCollection()
    {
        var failures = new[]
        {
            new ValidationFailure("PropertyOne", "error-one"),
            new ValidationFailure("PropertyOne", "error-two"),
            new ValidationFailure("PropertyTwo", "error-one"),
            new ValidationFailure("PropertyThree", "error-one"),
            new ValidationFailure("PropertyThree", "error-two"),
            new ValidationFailure("PropertyThree", "error-three")
        };

        var expected = new Dictionary<string, IEnumerable<string>>
        {
            { "propertyOne", new[] { "error-one", "error-two" } },
            { "propertyTwo", new[] { "error-one" } },
            { "propertyThree", new[] { "error-one", "error-two", "error-three" } }
        };
        
        var exception = new ValidationException(failures);
        var actual = ProblemDetailFactoryImplementations.GetValidationExceptionProblemDetails(exception);

        actual.Status.Should().Be(422);
        actual.Extensions["errors"].Should().BeEquivalentTo(expected);
    }
}