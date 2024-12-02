using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1;

/// <summary>AutoMapper profile for Applications.</summary>
public class ApplicationsProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="ApplicationsProfile"/> class.</summary>
    public ApplicationsProfile()
    {
        // App => ApplicationModel
        CreateMap<App, ApplicationModel>()
            .ForCtorParam(nameof(ApplicationModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ApplicationModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ApplicationModel.IsFirstParty), ops => ops.MapFrom(entity => entity.IsFirstParty))
            .ForCtorParam(nameof(ApplicationModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ApplicationModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
        
        // IEnumerable<App> => ResultPage<ApplicationModel>
        CreateMap<IEnumerable<App>, ResultPage<ApplicationModel>>()
            .ForCtorParam(nameof(ResultPage<ApplicationModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<ApplicationModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<ApplicationModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()));
        
        // App => ApplicationDetailsModel
        CreateMap<App, ApplicationDetailsModel>()
            .ForCtorParam(nameof(ApplicationDetailsModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ApplicationDetailsModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ApplicationDetailsModel.Description), ops => ops.MapFrom(entity => entity.Description))
            .ForCtorParam(nameof(ApplicationDetailsModel.IsFirstParty), ops => ops.MapFrom(entity => entity.IsFirstParty))
            .ForCtorParam(nameof(ApplicationDetailsModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ApplicationDetailsModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified))
            .ForCtorParam(nameof(ApplicationDetailsModel.Clients), ops => ops.MapFrom(entity => entity.Clients));
        
        // Client => ApplicationClientModel
        CreateMap<Client, ApplicationClientModel>()
            .ForCtorParam(nameof(ApplicationClientModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(ApplicationClientModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ApplicationClientModel.Type), ops => ops.MapFrom(entity => entity.Type.Name));
        
        // CreateApplicationRequestModel => CreateApplicationCommand
        CreateMap<CreateApplicationRequestModel, CreateApplicationCommand>()
            .ForCtorParam(nameof(CreateApplicationCommand.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(CreateApplicationCommand.Description), ops => ops.MapFrom(model => model.Description))
            .ForCtorParam(nameof(CreateApplicationCommand.IsFirstParty), ops => ops.MapFrom(model => model.IsFirstParty));
        
        // GetApplicationsRequestModel => GetApplicationsQuery
        CreateMap<GetApplicationsRequestModel, GetApplicationsQuery>()
            .ForCtorParam(nameof(GetApplicationsQuery.Cursor), ops => ops.MapFrom(model => model.Cursor))
            .ForCtorParam(nameof(GetApplicationsQuery.PageSize), ops => ops.MapFrom(model => model.PageSize))
            .ForCtorParam(nameof(GetApplicationsQuery.Name), ops => ops.MapFrom(model => model.Name))
            .ForCtorParam(nameof(GetApplicationsQuery.Description), ops => ops.MapFrom(model => model.Description))
            .ForCtorParam(nameof(GetApplicationsQuery.IsFirstParty), ops => ops.MapFrom(model => model.IsFirstParty))
            .ForCtorParam(nameof(GetApplicationsQuery.CreatedFrom), ops => ops.MapFrom(model => model.CreatedFrom))
            .ForCtorParam(nameof(GetApplicationsQuery.CreatedTo), ops => ops.MapFrom(model => model.CreatedTo))
            .ForCtorParam(nameof(GetApplicationsQuery.ModifiedFrom), ops => ops.MapFrom(model => model.ModifiedFrom))
            .ForCtorParam(nameof(GetApplicationsQuery.ModifiedTo), ops => ops.MapFrom(model => model.ModifiedTo));
    }
}