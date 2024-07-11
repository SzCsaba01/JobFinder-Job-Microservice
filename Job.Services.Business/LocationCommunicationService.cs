using Job.Data.Contracts.Helpers;
using Job.Services.Contracts;
using Location.Data.Access.Helpers.DTO.City;
using Location.Data.Access.Helpers.DTO.CountryStateCity;
using System.Text;
using System.Text.Json;

namespace Job.Services.Business;
public class LocationCommunicationService : ILocationCommunicationService
{
    private readonly HttpClient _httpClient;

    public LocationCommunicationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetRegionByCountryIso2CodeAsync(string countryIso2Code)
    {
        var response = await _httpClient.GetAsync(AppConstants.REGION_API_URL + "GetRegionByCountryIso2Code?countryIso2Code=" + countryIso2Code);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while fetching region by country iso2 code");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<ICollection<LocationDto>> GetCitiesByCityAndCountryNames(ICollection<CountryStateCityRegionDto> countryStateCityRegions)
    {
        var content = new StringContent(JsonSerializer.Serialize(countryStateCityRegions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(AppConstants.CITY_API_URL + "GetCitiesByCityAndCountryNames", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while fetching cities by city and country names");
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ICollection<LocationDto>>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions);
    }

    public async Task<ICollection<LocationDto>> GetCitiesByNamesAsync(ICollection<string> cityNames)
    {
        var content = new StringContent(JsonSerializer.Serialize(cityNames), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(AppConstants.CITY_API_URL + "GetCitiesByNames", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while fetching cities by names");
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ICollection<LocationDto>>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions);
    }

    public async Task<ICollection<LocationDto>> GetCountriesByNamesAsync(ICollection<string> countryNames)
    {
        var content = new StringContent(JsonSerializer.Serialize(countryNames), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(AppConstants.COUNTRY_API_URL + "GetCountriesByNames", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while fetching countries by names");
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ICollection<LocationDto>>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions);
    }

    public async Task<ICollection<LocationDto>> GetRegionsByNamesAsync(ICollection<string> regionNames)
    {
        var content = new StringContent(JsonSerializer.Serialize(regionNames), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(AppConstants.REGION_API_URL + "GetRegionsByNames", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while fetching regions by names");
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ICollection<LocationDto>>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions);
    }

    public async Task<ICollection<LocationDto>> GetStatesByNamesAsync(ICollection<string> stateNames)
    {
        var content = new StringContent(JsonSerializer.Serialize(stateNames), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(AppConstants.STATE_API_URL + "GetStatesByNames", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while fetching states by names");
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ICollection<LocationDto>>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions);
    }
}
