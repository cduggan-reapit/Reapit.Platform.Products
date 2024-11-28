using FluentValidation.Results;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Core.UnitTests.TestServices;
using Reapit.Platform.Products.Core.UseCases.Products.CreateProduct;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.CreateProducts;

public class CreateProductCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly MockNotificationsService _notifications = new();
    private readonly IValidator<CreateProductCommand> _validator = Substitute.For<IValidator<CreateProductCommand>>();
    private readonly FakeLogger<CreateProductCommandHandler> _logger = new ();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFails()
    {
        var command = GetCommand();
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("property", "error")]));

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsProduct_WhenCreationSuccessful()
    {
        // Fix identity and times
        using var guidProvider = new GuidProviderContext(Guid.NewGuid());
        using var timeProvider = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        
        var command = GetCommand();
        var product = new Product(command.Name, command.Description);
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
        
        _productRepository.CreateAsync(Arg.Is<Product>(p => p.Name == command.Name && p.Description == command.Name), Arg.Any<CancellationToken>())
            .Returns(product);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Should().BeEquivalentTo(product);
        
        var expectedNotification = MessageEnvelope.ProductCreated(actual);
        _notifications.LastMessage.Should().BeEquivalentTo(expectedNotification); 
    }
    
    /*
     * Private methods
     */

    private CreateProductCommandHandler CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        return new CreateProductCommandHandler(_unitOfWork, _notifications, _validator, _logger);
    }
    
    private static CreateProductCommand GetCommand(string name = "name", string? description = "description")
        => new(name, description);
}