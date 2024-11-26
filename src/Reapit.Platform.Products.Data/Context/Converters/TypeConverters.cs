using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Context.Converters;

/// <summary>Custom converters for database configuration.</summary>
public static class TypeConverters
{
    /// <summary>Convert a datetime to/from a persisted UTC value.</summary>
    public static readonly ValueConverter<DateTime, DateTime> DateTimeConverter = new(
        convertToProviderExpression: value => value.ToUniversalTime(),
        convertFromProviderExpression: value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

    // <summary>Convert a ClientType to an integer and back for data persistence.</summary>
    public static readonly ValueConverter<ClientType, int> ClientTypeConverter = new(
        convertToProviderExpression: clientType => clientType.Value,
        convertFromProviderExpression: integer => ClientType.GetByValue(integer) ?? ClientType.None);
    
    /// <summary>Convert a collection of strings to a single string and back for data persistence.</summary>
    public static readonly ValueConverter<ICollection<string>?, string?> StringArrayConverter = new(
        convertToProviderExpression: array => array == null ? null : string.Join(',', array),
        convertFromProviderExpression: list => list == null ? null : list.Split(',', StringSplitOptions.RemoveEmptyEntries));
}