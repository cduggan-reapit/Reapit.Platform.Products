using Reapit.Platform.Products.Core.Exceptions;

namespace Reapit.Platform.Products.Core.UnitTests.Exceptions;

public class IdentityProviderExceptionTests
{
     /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesException_WithNoParameters()
    {
        var sut = new IdentityProviderException();
        sut.Should().NotBeNull();
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage);
        var sut = new IdentityProviderException(message);
        sut.Message.Should().Be(message);
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage_AndInnerException()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage_AndInnerException);
        var innerException = new ArgumentNullException(nameof(Ctor_InitializesException_WithMessage_AndInnerException), "message");
        var sut = new IdentityProviderException(message, innerException);
        sut.Message.Should().Be(message);
        sut.InnerException.Should().BeEquivalentTo(innerException);
    }
    
    /*
     * Pre-defined exception: ResourceExistsConflict
     */

    [Fact]
    public void NullResponse_ReturnsException_WithExpectedMessage()
    {
        const string expected = "Null response.";
        var sut = IdentityProviderException.NullResponse;
        sut.Message.Should().Be(expected);
    }
    
    /*
     * CreateProblemDetails
     */
    
    [Fact]
    public void CreateProblemDetails_ShouldThrow_WhenExceptionTypeIncorrect()
    {
        var action = () => IdentityProviderException.CreateProblemDetails(new Exception());
        action.Should().Throw<ProblemDetailsFactoryException>();
    }
    
    [Fact]
    public void CreateProblemDetails_WithExpectedProperty_ForIdentityProviderException()
    {
        const string exceptionMessage = "test-exception";
        var exception = new IdentityProviderException(exceptionMessage);
        var actual = IdentityProviderException.CreateProblemDetails(exception);
        
        actual.Type.Should().EndWith(IdentityProviderException.ProblemDetailsType);
        actual.Title.Should().Be(IdentityProviderException.ProblemDetailsTitle);
        actual.Status.Should().Be(IdentityProviderException.ProblemDetailsStatusCode);
        actual.Detail.Should().BeEquivalentTo(exceptionMessage);
    }
}