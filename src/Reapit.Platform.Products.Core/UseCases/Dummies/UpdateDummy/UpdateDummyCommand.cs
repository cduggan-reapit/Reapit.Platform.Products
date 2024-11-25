using MediatR;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;

/// <summary>Mediator request representing a command to update an existing Dummy.</summary>
/// <param name="Id">The unique identifier of the object to update.</param>
/// <param name="Name">The name to apply to the object.</param>
public record UpdateDummyCommand(string Id, string Name) : IRequest<Dummy>;