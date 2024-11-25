using MediatR;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.GetDummies;

/// <summary>Mediator request representing a query to fetch one or more Dummy objects.</summary>
/// <param name="Name">The name of the Dummy.</param>
/// <param name="CreatedFrom">Apply filter for Dummies created on or after a given date and time (UTC).</param>
/// <param name="CreatedTo">Apply filter for Dummies created before a given date and time (UTC).</param>
/// <param name="ModifiedFrom">Apply filter for Dummies last modified on or after a given date and time (UTC).</param>
/// <param name="ModifiedTo">Apply filter for Dummies last modified before a given date and time (UTC).</param>
public record GetDummiesQuery(
    string? Name,
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    DateTime? ModifiedFrom,
    DateTime? ModifiedTo) : IRequest<IEnumerable<Dummy>>;