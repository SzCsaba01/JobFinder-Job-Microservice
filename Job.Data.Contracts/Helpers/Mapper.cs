using AutoMapper;
using Job.Data.Contracts.Helpers.DTO.Company;
using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Location.Data.Access.Helpers.DTO.City;
using Location.Data.Access.Helpers.DTO.CountryStateCity;

namespace Job.Data.Contracts.Helpers;
public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<LocationDto, LocationEntity>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.CountryIso2Code))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StateCode))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<LocationEntity, LocationDto>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region))
            .ForMember(dest => dest.CountryIso2Code, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


        CreateMap<CountryStateCityRegionDto, LocationEntity>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
           
        CreateMap<JobEntity,  JobDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(x => x.CategoryName)))
            .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.Locations))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(x => x.TagName)))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<JobDto, JobEntity>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(x => new JobCategoryMapping { CategoryName = x })))
            .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.Locations))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(x => new JobTagMapping { TagName = x })))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CompanyEntity, CompanyDto>();
        CreateMap<CompanyDto, CompanyEntity>();

        CreateMap<UserFeedbackDto, UserFeedbackEntity>();
        CreateMap<UserFeedbackEntity, UserFeedbackDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Job.Company))
            .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.Job.ContractType.Name))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UserFeedbackJobDetailsDto, JobEntity>();
        CreateMap<JobEntity, UserFeedbackJobDetailsDto>()
            .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Title));
    }
}
