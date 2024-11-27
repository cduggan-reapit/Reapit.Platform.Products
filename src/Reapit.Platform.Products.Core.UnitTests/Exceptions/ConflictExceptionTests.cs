using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Core.Exceptions;

namespace Reapit.Platform.Products.Core.UnitTests.Exceptions;

public class ConflictExceptionTests
{
     /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesException_WithNoParameters()
    {
        var sut = new ConflictException();
        sut.Should().NotBeNull();
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage);
        var sut = new ConflictException(message);
        sut.Message.Should().Be(message);
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage_AndInnerException()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage_AndInnerException);
        var innerException = new ArgumentNullException(nameof(Ctor_InitializesException_WithMessage_AndInnerException), "message");
        var sut = new ConflictException(message, innerException);
        sut.Message.Should().Be(message);
        sut.InnerException.Should().BeEquivalentTo(innerException);
    }
    
    /*
     * Pre-defined exception: ResourceExistsConflict
     */

    [Fact]
    public void ResourceExistsConflict_ReturnsException_WithExpectedMessage()
    {
        const string resourceType = "cake";
        const string resourceIdentifier = "sponge";
        const string expected = $"{resourceType} already exists for '{resourceIdentifier}'.";
        var sut = ConflictException.ResourceExists(resourceType, resourceIdentifier);
        sut.Message.Should().Be(expected);
    }
    
    /*
     * CreateProblemDetails
     */
    
    [Fact]
    public void CreateProblemDetails_ShouldThrow_WhenExceptionTypeIncorrect()
    {
        var action = () => ConflictException.CreateProblemDetails(new Exception());
        action.Should().Throw<ProblemDetailsFactoryException>();
    }
    
    [Fact]
    public void CreateProblemDetails_WithExpectedProperty_ForConflictException()
    {
        const string exceptionMessage = "test-exception";
        var exception = new ConflictException(exceptionMessage);
        var actual = ConflictException.CreateProblemDetails(exception);
        
        actual.Type.Should().EndWith(ConflictException.ProblemDetailsType);
        actual.Title.Should().Be(ConflictException.ProblemDetailsTitle);
        actual.Status.Should().Be(ConflictException.ProblemDetailsStatusCode);
        actual.Detail.Should().BeEquivalentTo(exceptionMessage);
    }
}