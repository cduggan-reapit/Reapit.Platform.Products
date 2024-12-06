﻿using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClients;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.GetClients;

public class GetClientsQueryValidatorTests
{
    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest(1);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    /*
     * Cursor
     */
    
    [Fact]
    public async Task Validate_Succeeds_WhenCursorLessThanZero()
    {
        var request = GetRequest(-1);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(GetClientsQuery.Cursor), CommonValidationMessages.CursorOutOfRange);
    }
    
    /*
     * PageSize
     */
    
    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task Validate_Succeeds_WhenPageSizeOutOfRange(int pageSize)
    {
        var request = GetRequest(pageSize: pageSize);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(GetClientsQuery.PageSize), CommonValidationMessages.PageSizeOutOfRange);
    }
    
    /*
     * Private methods
     */

    private static GetClientsQueryValidator CreateSut() => new();

    private static GetClientsQuery GetRequest(long? cursor = null, int pageSize = 25)
        => new(cursor, pageSize);
}