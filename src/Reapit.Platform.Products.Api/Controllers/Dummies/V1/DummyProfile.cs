using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;
using Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;
using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummies;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.Controllers.Dummies.V1;

/// <summary>Automapper profile for Dummy models, requests, commands, and entities.</summary>
public class DummyProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="DummyProfile"/> class.</summary>
    public DummyProfile()
    {
        /*
         * Map request models to Queries
         */
        
        CreateMap<ReadDummiesRequestModel, GetDummiesQuery>()
            .ForCtorParam(nameof(GetDummiesQuery.Name), ops => ops.MapFrom(src => src.Name))
            .ForCtorParam(nameof(GetDummiesQuery.CreatedFrom), ops => ops.MapFrom(src => src.CreatedFrom))
            .ForCtorParam(nameof(GetDummiesQuery.CreatedTo), ops => ops.MapFrom(src => src.CreatedTo))
            .ForCtorParam(nameof(GetDummiesQuery.ModifiedFrom), ops => ops.MapFrom(src => src.ModifiedFrom))
            .ForCtorParam(nameof(GetDummiesQuery.ModifiedTo), ops => ops.MapFrom(src => src.ModifiedTo));
        
        /*
         * Map request models to Commands
         */
        
        CreateMap<WriteDummyRequestModel, CreateDummyCommand>()
            .ForCtorParam(nameof(CreateDummyCommand.Name), ops => ops.MapFrom(model => model.Name));

        /*
         * Map domain entities to response models
         */
        
        CreateMap<Dummy, ReadDummyResponseModel>()
            .ForCtorParam(nameof(ReadDummyResponseModel.Id), ops => ops.MapFrom(entity => entity.Id.ToString("N")))
            .ForCtorParam(nameof(ReadDummyResponseModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(ReadDummyResponseModel.DateCreated), ops => ops.MapFrom(entity => entity.DateCreated))
            .ForCtorParam(nameof(ReadDummyResponseModel.DateModified), ops => ops.MapFrom(entity => entity.DateModified));
    }
}