using Reapit.Platform.Products.Data.Context.Converters;

namespace Reapit.Platform.Products.Data.UnitTests.Context.Converters;

public class TypeComparersTests
{
    /*
     * StringArrayComparer
     */

    [Fact]
    public void StringArrayComparer_ReturnsTrue_WhenSequenceContentsEqual()
    {
        var a = new[] { "a", "b", "c" };
        var b = new[] { "c", "b", "a" };
        
        var sut = TypeComparers.StringArrayComparer;
        var func = sut.EqualsExpression.Compile();
        var actual = func(a, b);
        actual.Should().BeTrue();
    }
    
    [Fact]
    public void StringArrayComparer_ReturnsFalse_WhenSequenceContentsNotEqual()
    {
        var a = new[] { "a", "b", "c" };
        var b = new[] { "d", "e", "f" };
        
        var sut = TypeComparers.StringArrayComparer;
        var func = sut.EqualsExpression.Compile();
        var actual = func(a, b);
        actual.Should().BeFalse();
    }
    
    [Fact]
    public void StringArrayComparer_ReturnsFalse_WhenEitherElementNull()
    {
        var array = new[] { "a", "b", "c" };
        
        var sut = TypeComparers.StringArrayComparer;
        var func = sut.EqualsExpression.Compile();
        func(array, null).Should().BeFalse();
        func(null, array).Should().BeFalse();
    }
    
    [Fact]
    public void StringArrayComparer_ReturnsTrue_WhenBothElementsNull()
    {
        var sut = TypeComparers.StringArrayComparer;
        var func = sut.EqualsExpression.Compile();
        func(null, null).Should().BeTrue();
    }
    
    [Fact]
    public void StringArrayComparer_CalculatesHashCode_ForCollection()
    {
        var array = new[] { "a", "b", "c" };
        var expected = array.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()));
        
        var sut = TypeComparers.StringArrayComparer;
        var func = sut.HashCodeExpression.Compile();
        func(array).Should().Be(expected);
    }
    
    [Fact]
    public void StringArrayComparer_ReturnsList_ForSnapshot()
    {
        var array = new[] { "a", "b", "c" };
        var sut = TypeComparers.StringArrayComparer;
        var func = sut.SnapshotExpression.Compile();
        func(array).Should().BeEquivalentTo(array);
    }
}