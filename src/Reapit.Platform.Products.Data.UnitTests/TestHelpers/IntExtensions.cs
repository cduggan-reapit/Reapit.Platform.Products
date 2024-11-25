namespace Reapit.Platform.Products.Data.UnitTests.TestHelpers;

public static class IntExtensions
{
    /// <summary>Get a guid representing an integer (e.g. 1 => 00000000-0000-0000-0000-000000000001).</summary>
    /// <param name="value">The integer to convert.</param>
    public static Guid ToGuid(this int value)
        => new($"{value:D32}");
}