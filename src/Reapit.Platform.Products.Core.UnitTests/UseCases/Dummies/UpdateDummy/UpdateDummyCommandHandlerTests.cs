using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.UpdateDummy;

public class UpdateDummyCommandHandlerTests
{
    private readonly IValidator<UpdateDummyCommand> _validator = Substitute.For<IValidator<UpdateDummyCommand>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDummyRepository _dummyRepository = Substitute.For<IDummyRepository>();
    
    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFails()
    {
        var command = GetCommand();
        _validator.ValidateAsync(Arg.Any<UpdateDummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") }));

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenGetByIdReturnsNull()
    {
        _validator.ValidateAsync(Arg.Any<UpdateDummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _dummyRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(null as Dummy);

        var command = GetCommand();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsDummy_WhenDummyUpdated()
    {
        var id = Guid.NewGuid();
        const string initialName = "dummy-name";
        const string newName = "new-name";
        
        _validator.ValidateAsync(Arg.Any<UpdateDummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        // Fix the guid and date time providers to control generated values
        using var initialTimeFixture = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var dummy = new Dummy(initialName){ Id = id };
        _dummyRepository.GetByIdAsync(dummy.Id, Arg.Any<CancellationToken>())
            .Returns(dummy);

        var command = GetCommand(id, newName);
        var updateTime = DateTimeOffset.Now;
        using var updateTimeFixture = new DateTimeOffsetProviderContext(updateTime);
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);

        actual.Name.Should().Be(newName);
        actual.DateCreated.Should().Be(DateTimeOffset.UnixEpoch.UtcDateTime);
        actual.DateModified.Should().Be(updateTime.UtcDateTime);
    }
    
    /*
     * Private methods
     */

    private UpdateDummyCommandHandler CreateSut()
    {
        _unitOfWork.Dummies.Returns(_dummyRepository);
        return new UpdateDummyCommandHandler(_validator, _unitOfWork);
    }

    private static UpdateDummyCommand GetCommand(Guid? id = null, string name = "test-name")
        => new((id ?? Guid.NewGuid()).ToString("N"), name);
}