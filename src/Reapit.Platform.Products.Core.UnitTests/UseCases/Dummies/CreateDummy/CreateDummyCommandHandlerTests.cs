using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.CreateDummy;

public class CreateDummyCommandHandlerTests
{
    private readonly IValidator<CreateDummyCommand> _validator = Substitute.For<IValidator<CreateDummyCommand>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDummyRepository _dummyRepository = Substitute.For<IDummyRepository>();

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFails()
    {
        var command = GetCommand();
        _validator.ValidateAsync(Arg.Any<CreateDummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") }));

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsDummy_WhenDummyCreated()
    {
        // Fix the guid and date time providers to control generated values
        using var guidProvider = new GuidProviderContext(Guid.NewGuid());
        using var dateTimeProvider = new DateTimeOffsetProviderContext(DateTimeOffset.Now);
        
        var command = GetCommand();
        var expected = new Dummy(command.Name);
        
        _validator.ValidateAsync(Arg.Any<CreateDummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Should().BeEquivalentTo(expected);
    }

    /*
     * Private methods
     */

    private CreateDummyCommandHandler CreateSut()
    {
        _unitOfWork.Dummies.Returns(_dummyRepository);
        return new CreateDummyCommandHandler(_validator, _unitOfWork);
    }

    private static CreateDummyCommand GetCommand(string name = "test-name")
        => new(name);
}