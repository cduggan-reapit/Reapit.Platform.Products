using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Products.Core.Extensions;

namespace Reapit.Platform.Products.Api.IntegrationTests.TestHelpers;

public static class HttpResponseMessageAssertionsExtensions
{
    /// <summary>Tests that the payload of an HttpResponseMessage matches the expected value.</summary>
    /// <param name="assertions">The assertions object.</param>
    /// <param name="title">The expected problem detail title.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by Format(string, params object?[]) explaining why the assertion is needed.
    /// If the phrase does not start with the word because, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in because.</param>
    public static async Task<AndConstraint<HttpResponseMessageAssertions>> BeProblemDescriptionAsync(
        this HttpResponseMessageAssertions assertions,
        string? title = null,
        string because = "",
        params object[] becauseArgs)
    {
        var actualPayload = await assertions.Subject.Content.ReadFromJsonAsync<ProblemDetails>();
        
        // Actual payload should not be null and the title should match or not be provided.
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(actualPayload is not null)
            .FailWith("Expected ProblemDetails response but found {0}.", await assertions.Subject.Content.ReadAsStringAsync())
            .Then
            .ForCondition(actualPayload?.Title == title || title == null)
            .FailWith("Expected ProblemDetails with title {0} but found {1}.", title, actualPayload?.Title);

        return new AndConstraint<HttpResponseMessageAssertions>(assertions);
    }
    
    /// <summary>Tests that the payload of an HttpResponseMessage matches the expected value.</summary>
    /// <param name="assertions">The assertions object.</param>
    /// <param name="expected">The expected payload object.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by Format(string, params object?[]) explaining why the assertion is needed.
    /// If the phrase does not start with the word because, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in because.</param>
    public static async Task<AndConstraint<HttpResponseMessageAssertions>> HavePayloadAsync(
        this HttpResponseMessageAssertions assertions,
        object expected, 
        string because = "",
        params object[] becauseArgs)
    {
        var actualPayload = await assertions.Subject.Content.ReadAsStringAsync();
        var expectedPayload = expected.ToJson();
        
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(actualPayload == expectedPayload)
            .FailWith("Expected Content {0} but found {1}.", expectedPayload, actualPayload);

        return new AndConstraint<HttpResponseMessageAssertions>(assertions);
    }
    
    /// <summary>Tests that the payload of an HttpResponseMessage matches the expected criteria.</summary>
    /// <param name="assertions">The assertions object.</param>
    /// <param name="expected">The expected criteria.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by Format(string, params object?[]) explaining why the assertion is needed.
    /// If the phrase does not start with the word because, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in because.</param>
    public static async Task<AndConstraint<HttpResponseMessageAssertions>> MatchPayloadAsync<T>(
        this HttpResponseMessageAssertions assertions,
        Func<T, bool> expected, 
        string because = "",
        params object[] becauseArgs)
    {
        var actualPayload = await assertions.Subject.Content.ReadFromJsonAsync<T>();
        
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(actualPayload != null && expected(actualPayload))
            .FailWith("Expected content to match criteria but found '{0}'.", await assertions.Subject.Content.ReadAsStringAsync());

        return new AndConstraint<HttpResponseMessageAssertions>(assertions);
    }
}