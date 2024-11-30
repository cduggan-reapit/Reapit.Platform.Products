using Microsoft.AspNetCore.WebUtilities;

namespace Reapit.Platform.Products.Core.UnitTests.TestHelpers;

public record MockHttpMessageRequestLog(string Uri, string? PostData, Dictionary<string, IEnumerable<string?>>? FormData)
{
    public static async Task<MockHttpMessageRequestLog> FromRequestMessageAsync(HttpRequestMessage request) 
        => new(
            Uri: request.RequestUri?.ToString() ?? "https://error", 
            PostData: await GetPostDataFromRequest(request),
            FormData: await GetFormDataFromRequest(request));

    private static async Task<Dictionary<string, IEnumerable<string?>>?> GetFormDataFromRequest(HttpRequestMessage request)
    {
        if (request.Content is not FormUrlEncodedContent formContent)
            return null;

        var rawFormData = await formContent.ReadAsStringAsync();
        using var reader = new FormReader(rawFormData);
        var formData = await reader.ReadFormAsync();

        return formData.ToDictionary(
            keySelector: item => item.Key, 
            elementSelector: item => item.Value.Select(v => v));
    }

    private static async Task<string?> GetPostDataFromRequest(HttpRequestMessage request)
    {
        if (request.Content == null)
            return null;
        
        var writeMethods = new[] { HttpMethod.Post, HttpMethod.Patch, HttpMethod.Put };
        if (!writeMethods.Contains(request.Method))
            return null;

        if (request.Content is FormUrlEncodedContent _)
            return null;

        return await request.Content.ReadAsStringAsync();
    }
}