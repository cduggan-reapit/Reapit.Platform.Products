using FluentValidation.Results;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Core.Exceptions;

namespace Reapit.Platform.Products.Core.UnitTests.Exceptions;

public class QueryValidationExceptionTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesException_WithNoParameters()
    {
        var sut = new QueryValidationException();
        sut.Should().NotBeNull();
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage);
        var sut = new QueryValidationException(message);
        sut.Message.Should().Be(message);
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage_AndInnerException()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage_AndInnerException);
        var innerException = new ArgumentNullException(nameof(Ctor_InitializesException_WithMessage_AndInnerException), "message");
        var sut = new QueryValidationException(message, innerException);
        sut.Message.Should().Be(message);
        sut.InnerException.Should().BeEquivalentTo(innerException);
    }
    
    /*
     * Pre-defined exception: ValidationFailed
     */

    [Fact]
    public void ResourceExistsQueryString_ReturnsException_WithExpectedMessage()
    {
        const string expectedMessage = "One or more validation errors occurred.";
        var validation = new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") });
        
        var sut = QueryValidationException.ValidationFailed(validation);
        sut.Message.Should().Be(expectedMessage);
        sut.Errors.Should().BeEquivalentTo(validation.Errors);
    }
    
    /*
     * CreateProblemDetails
     */
    
    [Fact]
    public void CreateProblemDetails_ShouldThrow_WhenExceptionTypeIncorrect()
    {
        var action = () => QueryValidationException.CreateProblemDetails(new Exception());
        action.Should().Throw<ProblemDetailsFactoryException>();
    }
    
    [Fact]
    public void CreateProblemDetails_WithExpectedProperties_ForQueryStringException()
    {
        const string exceptionMessage = "test-exception";
        var exception = new QueryValidationException(exceptionMessage);
        var actual = QueryValidationException.CreateProblemDetails(exception);
        
        actual.Type.Should().EndWith(QueryValidationException.ProblemDetailsType);
        actual.Title.Should().Be(QueryValidationException.ProblemDetailsTitle);
        actual.Status.Should().Be(QueryValidationException.ProblemDetailsStatusCode);
        actual.Detail.Should().BeEquivalentTo(exceptionMessage);
        
        // No errors?  No extensions!
        actual.Extensions.Should().BeEmpty();
    }
    
    [Fact]
    public void CreateProblemDetails_WithExpectedExtensions_ForQueryStringException()
    {
        var validation = new ValidationResult(
            new[]
            {
                new ValidationFailure("property-1", "error-1"), 
                new ValidationFailure("property-1", "error-2"),
                new ValidationFailure("property-1", "error-3"),
                new ValidationFailure("property-2", "error-1"),
                new ValidationFailure("property-2", "error-2"),
                new ValidationFailure("property-3", "error-1") 
            });

        var expectedDictionary = new Dictionary<string, IEnumerable<string>>
        {
            { "property-1", ["error-1", "error-2", "error-3"] },
            { "property-2", ["error-1", "error-2"] },
            { "property-3", ["error-1"] }
        };
        
        var exception = QueryValidationException.ValidationFailed(validation);
        var actual = QueryValidationException.CreateProblemDetails(exception);
        
        actual.Type.Should().EndWith(QueryValidationException.ProblemDetailsType);
        actual.Title.Should().Be(QueryValidationException.ProblemDetailsTitle);
        actual.Status.Should().Be(QueryValidationException.ProblemDetailsStatusCode);
        
        // Errors?  Have some extensions!
        actual.Extensions.Should().HaveCount(1)
            .And.AllSatisfy(item => item.Key.Should().Be("errors"));

        actual.Extensions.TryGetValue("errors", out var actualDictionaryObj);
        var actualDictionary = actualDictionaryObj as Dictionary<string, IEnumerable<string>>;
        actualDictionary.Should().BeEquivalentTo(expectedDictionary);
    }
}