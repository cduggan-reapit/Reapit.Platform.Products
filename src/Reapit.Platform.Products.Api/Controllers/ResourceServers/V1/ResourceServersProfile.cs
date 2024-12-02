using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1;

/// <summary>AutoMapper profile for ResourceServers.V1</summary>
public class ResourceServersProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="ResourceServersProfile"/> class.</summary>
    public ResourceServersProfile()
    {
        // GetResourceServersRequestModel => GetResourceServersQuery
        CreateMap<GetResourceServersRequestModel, GetResourceServersQuery>()
            .ForCtorParam(nameof(GetResourceServersQuery.Cursor), ops => ops.MapFrom(model => model.Cursor))
            .ForCtorParam(nameof(GetResourceServersQuery.PageSize), ops => ops.MapFrom(model => model.PageSize))
            .ForCtorParam(nameof(GetResourceServersQuery.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(GetResourceServersQuery.Audience), ops => ops.MapFrom(model => model.Audience))
            .ForCtorParam(nameof(GetResourceServersQuery.CreatedFrom), ops => ops.MapFrom(model => model.CreatedFrom))
            .ForCtorParam(nameof(GetResourceServersQuery.CreatedTo), ops => ops.MapFrom(model => model.CreatedTo))
            .ForCtorParam(nameof(GetResourceServersQuery.ModifiedFrom), ops => ops.MapFrom(model => model.ModifiedFrom))
            .ForCtorParam(nameof(GetResourceServersQuery.ModifiedTo), ops => ops.MapFrom(model => model.ModifiedTo));

        // ResourceServer => ResourceServerModel
        CreateMap<ResourceServer, ResourceServerModel>()
            .ForCtorParam(nameof(ResourceServerModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ResourceServerModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ResourceServerModel.Audience), ops => ops.MapFrom(entity => entity.Audience))
            .ForCtorParam(nameof(ResourceServerModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ResourceServerModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // ResourceServer => ResourceServerDetailsModel
        CreateMap<ResourceServer, ResourceServerDetailsModel>()
            .ForCtorParam(nameof(ResourceServerDetailsModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ResourceServerDetailsModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ResourceServerDetailsModel.Audience), ops => ops.MapFrom(entity => entity.Audience))
            .ForCtorParam(nameof(ResourceServerDetailsModel.TokenLifetime), ops => ops.MapFrom(entity => entity.TokenLifetime))
            .ForCtorParam(nameof(ResourceServerDetailsModel.Scopes), ops => ops.MapFrom(entity => entity.Scopes))
            .ForCtorParam(nameof(ResourceServerDetailsModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ResourceServerDetailsModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // IEnumerable<ResourceServer> => PagedResult<ResourceServerModel>
        CreateMap<IEnumerable<ResourceServer>, ResultPage<ResourceServerModel>>()
            .ForCtorParam(nameof(ResultPage<ResourceServerModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<ResourceServerModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<ResourceServerModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()));
        
        // ResourceServerScopeModel => RequestScopeModel
        CreateMap<ResourceServerScopeModel, RequestScopeModel>()
            .ForCtorParam(nameof(RequestScopeModel.Value), ops => ops.MapFrom(model => model.Value))
            .ForCtorParam(nameof(RequestScopeModel.Description), ops => ops.MapFrom(model => model.Description));
        
        // Scope => ResourceServerScopeModel
        CreateMap<Scope, ResourceServerScopeModel>()
            .ForCtorParam(nameof(ResourceServerScopeModel.Value), ops => ops.MapFrom(entity => entity.Value))
            .ForCtorParam(nameof(ResourceServerScopeModel.Description), ops => ops.MapFrom(entity => entity.Description));
        
        // CreateResourceServerRequestModel => CreateResourceServerCommand
        CreateMap<CreateResourceServerRequestModel, CreateResourceServerCommand>()
            .ForCtorParam(nameof(CreateResourceServerCommand.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(CreateResourceServerCommand.Audience), ops => ops.MapFrom(model => model.Audience))
            .ForCtorParam(nameof(CreateResourceServerCommand.TokenLifetime), ops => ops.MapFrom(model => model.TokenLifetime))
            .ForCtorParam(nameof(CreateResourceServerCommand.Scopes), ops => ops.MapFrom(model => model.Scopes));
    }
}