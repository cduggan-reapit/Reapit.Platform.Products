using Auth0.ManagementApi.Models;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.IntegrationTests.TestHelpers;

/// <summary>Helper methods for creating seed data objects.</summary>
public static class SeedHelpers
{
    internal static readonly DateTimeOffset BaseDate = new(2020, 1, 1, 1, 15, 30, TimeSpan.Zero);
    
    /// <summary>Get a numeric seed value as a guid.</summary>
    /// <param name="input">The seed value.</param>
    internal static Guid AsGuid(this int input) 
        => new(input.AsIdentity());

    /// <summary>Get a numeric seed value as an identifier string.</summary>
    /// <param name="input">The seed value.</param>
    internal static string AsIdentity(this int input)
        => $"{input:D32}";

    /// <summary>Get a collection of products.</summary>
    /// <param name="count">The number of products to create.</param>
    /// <param name="numberOfClients">The number of clients to create for each product.</param>
    internal static ICollection<Product> GetProductSeedData(int count, int numberOfClients)
        => Enumerable.Range(0, count)
            .Select(seed =>
            {
                // Get clients
                var fixedTime = BaseDate.AddDays(seed);
                using var guidProvider = new GuidProviderContext(seed.AsGuid());
                using var timeProvider = new DateTimeOffsetProviderContext(fixedTime);
                return new Product($"Product {seed:D3}", $"Description of Product {seed:D3}")
                {
                    DateModified = fixedTime.UtcDateTime.AddYears(1),
                    Clients = GetProductClientSeedData(seed, numberOfClients)
                };
            })
            .ToList();

    /// <summary>Get a collection of product clients.</summary>
    /// <param name="productSeed">The seed number of the parent product.</param>
    /// <param name="count">The number of clients to create.</param>
    private static ICollection<ProductClient> GetProductClientSeedData(int productSeed, int count)
        => Enumerable.Range(0, count)
            .Select(clientSeed =>
            {
                var fixedTime = BaseDate.AddDays(productSeed).AddHours(clientSeed);
                using var guidProvider = new GuidProviderContext(new Guid($"{productSeed:D8}-0000-0000-0000-{clientSeed:D12}"));
                using var timeProvider = new DateTimeOffsetProviderContext(fixedTime);
                
                // Even numbered seed records (inc. 0) are client_credentials clients, odd are authCode 
                var type = clientSeed % 2 == 0 ? ClientType.ClientCredentials : ClientType.AuthorizationCode;
                return new ProductClient(
                    productSeed.AsIdentity(),
                    $"client-id-{clientSeed:D3}",
                    $"grant-id-{clientSeed:D3}",
                    $"ProductClient {clientSeed:D3}",
                    $"Description of Product Client {clientSeed:D3}",
                    type,
                    type == ClientType.ClientCredentials ? "https://example.net/audience" : null,
                    [$"https://example.net/callback/{clientSeed:D3}"],
                    [$"https://example.net/sign-out/{clientSeed:D3}"])
                {
                    DateModified = fixedTime.AddYears(1).UtcDateTime
                };
            })
            .ToList();
}