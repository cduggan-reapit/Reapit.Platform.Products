using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1;

/// <summary>AutoMapper profile for the products endpoints.</summary>
public class ProductsProfile : Profile
{
    /// <summary>Initialize a new instance of the <see cref="ProductsProfile"/> class.</summary>
    public ProductsProfile()
    {
        // Product => ProductModel
        CreateMap<Product, ProductModel>()
            .ForCtorParam(nameof(ProductModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ProductModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ProductModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ProductModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // Product => ProductDetailsModel
        CreateMap<Product, ProductDetailsModel>()
            .ForCtorParam(nameof(ProductDetailsModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ProductDetailsModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ProductDetailsModel.Description), ops => ops.MapFrom(entity => entity.Description))
            .ForCtorParam(nameof(ProductDetailsModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ProductDetailsModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified))
            .ForCtorParam(nameof(ProductDetailsModel.Clients), ops => ops.MapFrom(entity => entity.Clients));
        
        // ProductClient => ProductDetailsClientModel
        CreateMap<ProductClient, ProductDetailsClientModel>()
            .ForCtorParam(nameof(ProductDetailsClientModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ProductDetailsClientModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ProductDetailsClientModel.Type), ops => ops.MapFrom(entity => entity.Type.Name));
        
        // IEnumerable<Product> => PagedResult<ProductModel>
        CreateMap<IEnumerable<Product>, ResultPage<ProductModel>>()
            .ForCtorParam(nameof(ResultPage<ProductModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<ProductModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<ProductModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()));
    }
}