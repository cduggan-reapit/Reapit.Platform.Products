using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Reapit.Platform.Products.Data.Context.Converters;

public static class TypeComparers
{
    public static ValueComparer<ICollection<string>> StringArrayComparer => 
        new(equalsExpression: (a, b) => CompareCollections(a, b),
            hashCodeExpression: c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            snapshotExpression: c => c.ToList());

    private static bool CompareCollections(ICollection<string>? a, ICollection<string>? b)
    {
        if (a == null && b == null)
            return true;

        if (a == null || b == null)
            return false;

        return !a.Except(b).Concat(b.Except(a)).Any();
    }
}