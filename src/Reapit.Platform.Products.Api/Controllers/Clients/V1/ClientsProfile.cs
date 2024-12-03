using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClients;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1;

/// <summary>AutoMapper profile for the clients controller.</summary>
public class ClientsProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="ClientsProfile"/> class.</summary>
    public ClientsProfile()
    {
        // GetClientsRequestModel => GetClientsQuery
        CreateMap<GetClientsRequestModel, GetClientsQuery>()
            .ForCtorParam(nameof(GetClientsQuery.Cursor), ops => ops.MapFrom(model => model.Cursor))
            .ForCtorParam(nameof(GetClientsQuery.PageSize), ops => ops.MapFrom(model => model.PageSize))
            .ForCtorParam(nameof(GetClientsQuery.AppId), ops => ops.MapFrom(model => model.AppId))
            .ForCtorParam(nameof(GetClientsQuery.Type), ops => ops.MapFrom(model => model.Type))
            .ForCtorParam(nameof(GetClientsQuery.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(GetClientsQuery.Description), ops => ops.MapFrom(model => model.Description))
            .ForCtorParam(nameof(GetClientsQuery.CreatedFrom), ops => ops.MapFrom(model => model.CreatedFrom))
            .ForCtorParam(nameof(GetClientsQuery.CreatedTo), ops => ops.MapFrom(model => model.CreatedTo))
            .ForCtorParam(nameof(GetClientsQuery.ModifiedFrom), ops => ops.MapFrom(model => model.ModifiedFrom))
            .ForCtorParam(nameof(GetClientsQuery.ModifiedTo), ops => ops.MapFrom(model => model.ModifiedTo));
        
        // Client => ClientModel
        CreateMap<Entities.Client, ClientModel>()
            .ForCtorParam(nameof(ClientModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ClientModel.AppId), ops => ops.MapFrom(entity => entity.AppId))
            .ForCtorParam(nameof(ClientModel.Type), ops => ops.MapFrom(entity => entity.Type.Name))
            .ForCtorParam(nameof(ClientModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ClientModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ClientModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // IEnumerable<Client> => ResultPage<ClientModel>
        CreateMap<IEnumerable<Entities.Client>, ResultPage<ClientModel>>()
            .ForCtorParam(nameof(ResultPage<ClientModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<ClientModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<ClientModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()));
        
        // Client => ClientDetailsModel
        CreateMap<Entities.Client, ClientDetailsModel>()
            .ForCtorParam(nameof(ClientDetailsModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ClientDetailsModel.AppId), ops => ops.MapFrom(entity => entity.AppId))
            .ForCtorParam(nameof(ClientDetailsModel.Type), ops => ops.MapFrom(entity => entity.Type.Name))
            .ForCtorParam(nameof(ClientDetailsModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ClientDetailsModel.Description), ops => ops.MapFrom(entity => entity.Description))
            .ForCtorParam(nameof(ClientDetailsModel.LoginUrl), ops => ops.MapFrom(entity => entity.LoginUrl))
            .ForCtorParam(nameof(ClientDetailsModel.CallbackUrls), ops => ops.MapFrom(entity => entity.CallbackUrls))
            .ForCtorParam(nameof(ClientDetailsModel.SignOutUrls), ops => ops.MapFrom(entity => entity.SignOutUrls))
            .ForCtorParam(nameof(ClientDetailsModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ClientDetailsModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified))
            .ForCtorParam(nameof(ClientDetailsModel.Grants), ops => ops.MapFrom(entity => entity.Grants));
        
        // Grant => ClientGrantModel
        CreateMap<Entities.Grant, ClientGrantModel>()
            .ForCtorParam(nameof(ClientGrantModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ClientGrantModel.ResourceServerId), ops => ops.MapFrom(entity => entity.ResourceServerId))
            .ForCtorParam(nameof(ClientGrantModel.ResourceServerName), ops => ops.MapFrom(entity => entity.ResourceServer == null ? null : entity.ResourceServer.Name));
        
        // CreateClientRequestModel => CreateClientCommand
        CreateMap<CreateClientRequestModel, CreateClientCommand>()
            .ForCtorParam(nameof(CreateClientCommand.AppId), ops => ops.MapFrom(model => model.AppId))
            .ForCtorParam(nameof(CreateClientCommand.Type), ops => ops.MapFrom(model => model.Type))
            .ForCtorParam(nameof(CreateClientCommand.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(CreateClientCommand.Description), ops => ops.MapFrom(model => model.Description))
            .ForCtorParam(nameof(CreateClientCommand.LoginUrl), ops => ops.MapFrom(model => model.LoginUrl))
            .ForCtorParam(nameof(CreateClientCommand.CallbackUrls), ops => ops.MapFrom(model => model.CallbackUrls))
            .ForCtorParam(nameof(CreateClientCommand.SignOutUrls), ops => ops.MapFrom(model => model.SignOutUrls));
    }
}