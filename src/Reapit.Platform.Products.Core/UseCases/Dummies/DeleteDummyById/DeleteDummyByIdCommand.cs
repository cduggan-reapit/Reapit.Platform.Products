using MediatR;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.DeleteDummyById;

/// <summary>Mediator request representing a command to delete a single Dummy.</summary>
/// <param name="Id">The unique identifier of the Dummy to return.</param>
public record DeleteDummyByIdCommand(string Id) : IRequest;