using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1;

/// <summary>AutoMapper profile for the grants controller.</summary>
public class GrantsProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="GrantsProfile"/> class.</summary>
    public GrantsProfile()
    {
        // Entities.Grant => GrantModel
        CreateMap<Entities.Grant, GrantModel>()
            .ForCtorParam(nameof(GrantModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(GrantModel.Client), ops => ops.MapFrom(entity => entity.Client))
            .ForCtorParam(nameof(GrantModel.ResourceServer), ops => ops.MapFrom(entity => entity.ResourceServer))
            .ForCtorParam(nameof(GrantModel.Scopes), ops => ops.MapFrom(entity => entity.Scopes.Select(scope => scope.Value)))
            .ForCtorParam(nameof(GrantModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(GrantModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // Entities.Client => GrantClientModel
        CreateMap<Entities.Client, GrantClientModel>()
            .ForCtorParam(nameof(GrantClientModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(GrantClientModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(GrantClientModel.Type), ops => ops.MapFrom(entity => entity.Type.Name));
        
        // Entities.ResourceServer => GrantResourceServerModel
        CreateMap<Entities.ResourceServer, GrantResourceServerModel>()
            .ForCtorParam(nameof(GrantResourceServerModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(GrantResourceServerModel.Name), ops => ops.MapFrom(entity => entity.Name));
        
        // IEnumerable<Entities.Grant> => ResultPage<GrantModel>
        CreateMap<IEnumerable<Entities.Grant>, ResultPage<GrantModel>>()
            .ForCtorParam(nameof(ResultPage<GrantModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<GrantModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()))
            .ForCtorParam(nameof(ResultPage<GrantModel>.Count), ops => ops.MapFrom(collection => collection.Count()));
        
        // GetGrantsRequestModel => GetGrantsQuery
        CreateMap<GetGrantsRequestModel, GetGrantsQuery>()
            .ForCtorParam(nameof(GetGrantsQuery.Cursor), ops => ops.MapFrom(model => model.Cursor))
            .ForCtorParam(nameof(GetGrantsQuery.PageSize), ops => ops.MapFrom(model => model.PageSize))
            .ForCtorParam(nameof(GetGrantsQuery.ClientId), ops => ops.MapFrom(model => model.ClientId))
            .ForCtorParam(nameof(GetGrantsQuery.ResourceServerId), ops => ops.MapFrom(model => model.ResourceServerId))
            .ForCtorParam(nameof(GetGrantsQuery.CreatedFrom), ops => ops.MapFrom(model => model.CreatedFrom))
            .ForCtorParam(nameof(GetGrantsQuery.CreatedTo), ops => ops.MapFrom(model => model.CreatedTo))
            .ForCtorParam(nameof(GetGrantsQuery.ModifiedFrom), ops => ops.MapFrom(model => model.ModifiedFrom))
            .ForCtorParam(nameof(GetGrantsQuery.ModifiedTo), ops => ops.MapFrom(model => model.ModifiedTo));
        
        // CreateGrantRequestModel => CreateGrantCommand
        CreateMap<CreateGrantRequestModel, CreateGrantCommand>()
            .ForCtorParam(nameof(CreateGrantCommand.ClientId), ops => ops.MapFrom(model => model.ClientId))
            .ForCtorParam(nameof(CreateGrantCommand.ResourceServerId), ops => ops.MapFrom(model => model.ResourceServerId))
            .ForCtorParam(nameof(CreateGrantCommand.Scopes), ops => ops.MapFrom(model => model.Scopes));
    }
}