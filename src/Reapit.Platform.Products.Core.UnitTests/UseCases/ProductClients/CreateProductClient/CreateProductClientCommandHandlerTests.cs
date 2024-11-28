using FluentValidation.Results;
using Reapit.Platform.Products.Core.UseCases.ProductClients.CreateProductClient;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.CreateProductClient;

public class CreateProductClientCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductClientRepository _productClientRepository = Substitute.For<IProductClientRepository>();
    private readonly IValidator<CreateProductClientCommand> _validator = Substitute.For<IValidator<CreateProductClientCommand>>();
    private readonly FakeLogger<CreateProductClientCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFails()
    {
        _validator.ValidateAsync(Arg.Any<CreateProductClientCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("property", "error")]));

        var command = GetCommand();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsProductClient_WhenRequestSuccessful()
    {
        _validator.ValidateAsync(Arg.Any<CreateProductClientCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        // TODO: factor in the clientId and grantId once that's built 
        
        // This will be an invalid command, but it doesn't matter because we mock the validator
        var command = GetCommand(
            callbackUrls: ["https://example.net/callback"], 
            signOutUrls:["https://example.net/sign-out"]);
        
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        
        actual.ProductId.Should().Be(command.ProductId);
        actual.Name.Should().Be(command.Name);
        actual.Description.Should().Be(command.Description);
        actual.Type.Should().Be((ClientType)command.Type);
        actual.Audience.Should().Be(command.Audience);
        actual.CallbackUrls.Should().BeEquivalentTo(command.CallbackUrls);
        actual.SignOutUrls.Should().BeEquivalentTo(command.SignOutUrls);

        await _productClientRepository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private CreateProductClientCommandHandler CreateSut()
    {
        _unitOfWork.ProductClients.Returns(_productClientRepository);
        return new CreateProductClientCommandHandler(_unitOfWork, _validator, _logger);
    }
    
    private static CreateProductClientCommand GetCommand(
        string productId = "product-id",
        string name = "name", 
        string? description = "description", 
        string type = "client_credentials",
        string? audience = "https://example.audience",
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(productId, name, description, type, audience, callbackUrls, signOutUrls);
    
    private static ProductClient GetProductClient(
        string productId = "productId",
        string clientId = "clientId",
        string grantId = "grantId",
        string name = "name",
        string description = "description",
        ClientType? type = null,
        string? audience = null, 
        ICollection<string>? callbackUrls = null, 
        ICollection<string>? signOutUrls = null)
        => new(productId, clientId, grantId, name, description, type ?? ClientType.ClientCredentials, audience, callbackUrls, signOutUrls);
}