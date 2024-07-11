using Location.Data.Access.Helpers.DTO.City;
using Location.Data.Access.Helpers.DTO.CountryStateCity;

namespace Job.Services.Contracts;
public interface ILocationCommunicationService
{
    public Task<string> GetRegionByCountryIso2CodeAsync(string countryIso2Code);
    public Task<ICollection<LocationDto>> GetRegionsByNamesAsync(ICollection<string> regionNames);
    public Task<ICollection<LocationDto>> GetCountriesByNamesAsync(ICollection<string> countryNames);
    public Task<ICollection<LocationDto>> GetStatesByNamesAsync(ICollection<string> stateNames);
    public Task<ICollection<LocationDto>> GetCitiesByNamesAsync(ICollection<string> cityNames);
    public Task<ICollection<LocationDto>> GetCitiesByCityAndCountryNames(ICollection<CountryStateCityRegionDto> countryStateCityRegions);
}
