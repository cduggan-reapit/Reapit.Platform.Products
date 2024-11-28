using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClients;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1;

/// <summary>AutoMapper profile for product clients.</summary>
public class ProductClientsProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="ProductClientsProfile"/> class.</summary>
    public ProductClientsProfile()
    {
        // ProductClient => ProductClientModel
        CreateMap<ProductClient, ProductClientModel>()
            .ForCtorParam(nameof(ProductClientModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ProductClientModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ProductClientModel.Type), ops => ops.MapFrom(entity => entity.Type.Name))
            .ForCtorParam(nameof(ProductClientModel.ProductId), ops => ops.MapFrom(entity => entity.ProductId))
            .ForCtorParam(nameof(ProductClientModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ProductClientModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // ProductClient => ProductClientDetailsModel
        CreateMap<ProductClient, ProductClientDetailsModel>()
            .ForCtorParam(nameof(ProductClientDetailsModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ProductClientDetailsModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ProductClientDetailsModel.Description), ops => ops.MapFrom(entity => entity.Description))
            .ForCtorParam(nameof(ProductClientDetailsModel.Type), ops => ops.MapFrom(entity => entity.Type.Name))
            .ForCtorParam(nameof(ProductClientDetailsModel.Audience), ops => ops.MapFrom(entity => entity.Audience))
            .ForCtorParam(nameof(ProductClientDetailsModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ProductClientDetailsModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified))
            .ForCtorParam(nameof(ProductClientDetailsModel.CallbackUrls), ops => ops.MapFrom(entity => entity.CallbackUrls))
            .ForCtorParam(nameof(ProductClientDetailsModel.SignOutUrls), ops => ops.MapFrom(entity => entity.SignOutUrls))
            .ForCtorParam(nameof(ProductClientDetailsModel.Product), ops => ops.MapFrom(entity => new ProductClientDetailsProductModel(entity.ProductId, entity.Product!.Name)));
        
        // IEnumerable<ProductClient> => PagedResult<ProductClientModel>
        CreateMap<IEnumerable<ProductClient>, ResultPage<ProductClientModel>>()
            .ForCtorParam(nameof(ResultPage<ProductClientModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<ProductClientModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<ProductClientModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()));
        
        // GetProductClientsRequestModel => GetProductClientsQuery
        CreateMap<GetProductClientsRequestModel, GetProductClientsQuery>()
            .ForCtorParam(nameof(GetProductClientsQuery.Cursor), ops => ops.MapFrom(model => model.Cursor))
            .ForCtorParam(nameof(GetProductClientsQuery.PageSize), ops => ops.MapFrom(model => model.PageSize))
            .ForCtorParam(nameof(GetProductClientsQuery.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(GetProductClientsQuery.Description), ops => ops.MapFrom(model => model.Description))
            .ForCtorParam(nameof(GetProductClientsQuery.ProductId), ops => ops.MapFrom(model => model.ProductId))
            .ForCtorParam(nameof(GetProductClientsQuery.Type), ops => ops.MapFrom(model => model.Type))
            .ForCtorParam(nameof(GetProductClientsQuery.CreatedFrom), ops => ops.MapFrom(model => model.CreatedFrom))
            .ForCtorParam(nameof(GetProductClientsQuery.CreatedTo), ops => ops.MapFrom(model => model.CreatedTo))
            .ForCtorParam(nameof(GetProductClientsQuery.ModifiedFrom), ops => ops.MapFrom(model => model.ModifiedFrom))
            .ForCtorParam(nameof(GetProductClientsQuery.ModifiedTo), ops => ops.MapFrom(model => model.ModifiedTo));
    }
}