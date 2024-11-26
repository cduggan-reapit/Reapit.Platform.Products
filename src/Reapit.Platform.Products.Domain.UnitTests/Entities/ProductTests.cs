using System.Text.Json;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class ProductTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_SetsProperties_WhenInitializingInstance()
    {
        var fixedId = Guid.NewGuid();
        using var guidContext = new GuidProviderContext(fixedId);
        
        var fixedDate = new DateTimeOffset(2024, 11, 26, 15, 2, 32, TimeSpan.Zero);
        using var timeContext = new DateTimeOffsetProviderContext(fixedDate);

        const string name = "name";
        const string description = "description";
        var expectedEpochTime = (long)(fixedDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        
        var entity = new Product(name, description);
        
        // Explicit
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        
        // Implicit
        entity.Id.Should().Be($"{fixedId:N}");
        entity.DateCreated.Should().Be(fixedDate.UtcDateTime);
        entity.DateModified.Should().Be(fixedDate.UtcDateTime);
        entity.Cursor.Should().Be(expectedEpochTime);
    }
    
    /*
     * Update
     */
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenParametersNull()
    {
        var entity = GetProduct();
        entity.Update(null, null);

        entity.IsDirty.Should().BeFalse();
        entity.DateModified.Should().Be(entity.DateCreated);
    }
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenValuesUnchanged()
    {
        var entity = GetProduct();
        entity.Update(entity.Name, entity.Description);

        entity.IsDirty.Should().BeFalse();
        entity.DateModified.Should().Be(entity.DateCreated);
    }
    
    [Fact]
    public void Update_UpdatesEntity_WhenValuesChanged()
    {
        var entity = GetProduct();
        entity.Update("new name", entity.Description);

        entity.IsDirty.Should().BeTrue();
        entity.DateModified.Should().NotBe(entity.DateCreated);
    }
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenArrayContentUnchanged()
    {
        var entity = GetProduct();
        entity.Update(entity.Name, entity.Description);

        entity.IsDirty.Should().BeFalse();
        entity.DateModified.Should().Be(entity.DateCreated);
    }
    
    /*
     * SoftDelete
     */
    
    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        using var timeFixture = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var entity = GetProduct();
        entity.DateDeleted.Should().BeNull();
        
        var fixedDate = new DateTimeOffset(2024, 10, 18, 15, 12, 17, TimeSpan.FromHours(1));
        using var secondTimeFixture = new DateTimeOffsetProviderContext(fixedDate);

        entity.SoftDelete();

        entity.DateDeleted.Should().Be(fixedDate.UtcDateTime);
    }
    
    /*
     * AsSerializable
     */

    [Fact]
    public void AsSerializable_ReturnsAnonymousObject_ForGroup()
    {
        const string name = "name", description = "description", organisationId = "organisation-id";
        
        var entity = GetProduct();
        var expected = new { Id = entity.Id, entity.Name, entity.DateCreated, entity.DateModified };
        
        var actual = entity.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_ForEntity()
    {
        var entity = GetProduct();
        var expected = JsonSerializer.Serialize(entity.AsSerializable());
        var actual = entity.ToString();
        actual.Should().Be(expected);
    }
    
    /*
     * Private methods
     */
    
    private static Product GetProduct(string name = "name", string? description = "description") 
        => new(name, description);
}