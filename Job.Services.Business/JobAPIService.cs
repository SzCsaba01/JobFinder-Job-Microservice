using AutoMapper;
using Job.Data.Access.Helpers.JobDto.Adzuna;
using Job.Data.Access.Helpers.JobDto.Jobicy;
using Job.Data.Access.Helpers.JobEntities.Remote;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Job.Services.Contracts;
using Location.Data.Access.Helpers.DTO.CountryStateCity;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Job.Services.Business;
public class JobAPIService : IJobAPIService
{
    private readonly ILocationCommunicationService _locationCommunicationService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public JobAPIService(
        ILocationCommunicationService locationCommunicationService,
        HttpClient httpClient,
        IConfiguration config,
        IMapper mapper
        )
    {
        _locationCommunicationService = locationCommunicationService;
        _httpClient = httpClient;
        _config = config;
        _mapper = mapper;
    }

    public async Task<List<JobEntity>> GetAdzunaJobsAsync(JobFilterDto jobSearchDto)
    {
        var url = "https://api.adzuna.com/v1/api/jobs/gb/search/1";

        var appId = _config["Adzuna:AppId"];
        var appKey = _config["Adzuna:AppKey"];

        url += $"?app_id={appId}&app_key={appKey}&results_per_page=50";

        if (!string.IsNullOrEmpty(jobSearchDto.Title))
        {
            url += $"&title_only={jobSearchDto.Title}";
        }

        string location = "";

        if (jobSearchDto.Country == "gb")
        {
            if (!string.IsNullOrEmpty(jobSearchDto.City))
            {
                location = jobSearchDto.City;
            }
            else if (!string.IsNullOrEmpty(jobSearchDto.State))
            {
                location = jobSearchDto.State;
            }
        }

        if (!string.IsNullOrEmpty(location))
        {
            url += $"&where={location}";
        }

        var response = await _httpClient.GetAsync(url);

        var jobs = new AdzunaJobListDto();

        if (response.IsSuccessStatusCode)
        {
            var byteArray = await response.Content.ReadAsByteArrayAsync();
            Encoding encoding = Encoding.UTF8;
            string decodedContent = encoding.GetString(byteArray);
            jobs = JsonSerializer.Deserialize<AdzunaJobListDto>(decodedContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        var mappedJobs = new List<JobEntity>();

        foreach (var job in jobs.Results)
        {
            try
            {
                var country = "GB";
                var countryStateCity = new List<CountryStateCityRegionDto>();

                foreach (var area in job.Location.Area)
                {
                    countryStateCity.Add(new CountryStateCityRegionDto
                    {
                        Country = country,
                        City = area,
                    });
                }

                var searchCities = await _locationCommunicationService.GetCitiesByCityAndCountryNames(countryStateCity);
                var locations = new List<LocationEntity>();

                if (searchCities.Any())
                {
                    foreach(var searchCity in searchCities)
                    {
                        locations.Add(new LocationEntity
                        {
                            City = searchCity.City,
                            Country = searchCity.CountryIso2Code,
                            State = searchCity.StateCode,
                            Region = searchCity.Region
                        });
                    }
                }

                var mappedJob = new JobEntity
                {
                    ExternalId = job.Id,
                    ExtrnalSource = "Adzuna",
                    isActive = true,
                    isRemote = false,
                    Description = job.Description,
                    Title = job.Title.Trim(),
                    Url = job.Redirect_Url,
                    DatePosted = DateTime.Parse(job.Created),
                    Locations = locations,
                    Company = new CompanyEntity { Name = job.Company.Display_Name.Trim() },
                    CompanyName = job.Company.Display_Name.Trim(),
                    ContractTypeName = job.Contract_Time is not null ?  job.Contract_Time.Trim() : null,
                    ContractType = job.Contract_Time is not null ? new ContractTypeEntity { Name = job.Contract_Time.Trim() } : null,
                    Categories = new List<JobCategoryMapping>
                    {
                        new JobCategoryMapping { CategoryName = job.Category.Label.Trim() }
                    },
                };

                mappedJobs.Add(mappedJob);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return mappedJobs;
    }

    public async Task<List<JobEntity>> GetJobicyJobsAsync(JobFilterDto jobSearchDto)
    {
        var url = "https://jobicy.com/api/v2/remote-jobs?count=50&geo=romania";

        if (!string.IsNullOrEmpty(jobSearchDto.Title))
        {
            url += $"&tag={jobSearchDto.Title}";
        }

        var response = await _httpClient.GetAsync(url);

        var jobs = new JobicyJobListDto();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            jobs = JsonSerializer.Deserialize<JobicyJobListDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        var mappedJobs = new List<JobEntity>();

        foreach (var job in jobs.Jobs)
        {
            try
            {
                var locations = new List<LocationEntity>();

                if (job.JobGeo != "Anywhere")
                {
                    var areaNames = job.JobGeo.Split(",").Select(area => area.Trim()).ToList();

                    var areaNamesToBeRemoved = new List<string>();
                    foreach (var areaName in areaNames)
                    {
                        if (areaName == "EMEA")
                        {
                            locations.AddRange(GetEMEALocations());
                            areaNamesToBeRemoved.Add(areaName);
                        }
                        else if (areaName == "LATAM")
                        {
                            locations.AddRange(GetLATAMLocations());
                            areaNamesToBeRemoved.Add(areaName);
                        }
                        else if (areaName == "APAC")
                        {
                            locations.AddRange(GetAPACLocations());
                            areaNamesToBeRemoved.Add(areaName);
                        }
                        else if (areaName == "UK")
                        {
                            locations.Add(new LocationEntity { Country = "GB", Region = "Europe" });
                            areaNamesToBeRemoved.Add(areaName);
                        }
                    }

                    areaNames.RemoveAll(x => areaNamesToBeRemoved.Contains(x));

                    if (areaNames.Any())
                    {
                        var regions = await _locationCommunicationService.GetRegionsByNamesAsync(areaNames);

                        if (regions.Any())
                        {
                            var regionNames = regions.Select(x => x.Region).ToList();
                            locations.RemoveAll(x => regionNames.Contains(x.Region));
                            locations.AddRange(_mapper.Map<List<LocationEntity>>(regions));
                            areaNames.RemoveAll(x => regionNames.Contains(x));
                        }

                        if (areaNames.Any())
                        {
                            var countries = await _locationCommunicationService.GetCountriesByNamesAsync(areaNames);

                            if (countries.Any())
                            {
                                locations.AddRange(_mapper.Map<List<LocationEntity>>(countries));
                            }
                        }
                    }
                }

                var jobType = job.JobType.FirstOrDefault().Trim();

                if (jobType.ToLower().Equals("full-time"))
                {
                    jobType = "full_time";
                }
                else if (jobType.ToLower().Equals("part-time"))
                {
                    jobType = "part_time";
                }


                var mappedJob = new JobEntity
                {
                    ExternalId = job.Id.ToString(),
                    ExtrnalSource = "Jobicy",
                    isActive = true,
                    isRemote = true,
                    Locations = locations,
                    Description = job.JobDescription,
                    Title = job.JobTitle.Trim(),
                    Url = job.Url,
                    DatePosted = DateTime.Parse(job.PubDate),
                    ContractTypeName = jobType.Trim(),
                    CompanyName = job.CompanyName.Trim(),
                    Company = new CompanyEntity { Name = job.CompanyName.Trim(), Logo = job.CompanyLogo },
                    ContractType = new ContractTypeEntity { Name = jobType.Trim() },
                    Categories = job.JobIndustry.Select(industry => new JobCategoryMapping { CategoryName = industry.Trim() }).ToList()
                };

                mappedJobs.Add(mappedJob);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return mappedJobs;

    }

    public async Task<List<JobEntity>> GetRemotiveJobsAsync(JobFilterDto jobSearchDto)
    {
        var url = "https://remotive.com/api/remote-jobs?limit=100";

        if (!string.IsNullOrEmpty(jobSearchDto.Title))
        {
            url += $"&search={jobSearchDto.Title}";
        }

        var response = await _httpClient.GetAsync(url);

        var jobs = new RemoteJobListDto();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            jobs = JsonSerializer.Deserialize<RemoteJobListDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        var cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        var mappedJobs = new List<JobEntity>();

        foreach (var job in jobs.Jobs)
        {
            try
            {
                var locationStrings = job.Candidate_Required_Location.Split(",").Select(area => area.Trim()).ToList();

                var locations = new List<LocationEntity>();

                var countriesAndRegions = new List<string>();
                var locationsToBeRemoved = new List<string>();

                foreach (var locationString in locationStrings)
                {
                    if (locationString == "EMEA")
                    {
                        locations.AddRange(GetEMEALocations());
                        locationsToBeRemoved.Add(locationString);
                    }
                    else if (locationString == "LATAM")
                    {
                        locations.AddRange(GetLATAMLocations());
                        locationsToBeRemoved.Add(locationString);
                    }
                    else if (locationString == "APAC")
                    {
                        locations.AddRange(GetAPACLocations());
                        locationsToBeRemoved.Add(locationString);
                    }
                    else if (locationString == "UK")
                    {
                        locations.Add(new LocationEntity { Country = "GB", Region = "Europe" });
                        locationsToBeRemoved.Add(locationString);
                    } 
                    else if (locationString == "Worldwide" || ContainsSpecialCharacters(locationString))
                    {
                       locationsToBeRemoved.Add(locationString);
                    }
                }

                locationStrings.RemoveAll(x => locationsToBeRemoved.Contains(x));

                if (locationStrings.Any())
                {
                    var regions = await _locationCommunicationService.GetRegionsByNamesAsync(locationStrings);

                    if (regions.Any())
                    {
                        var regionNames = regions.Select(x => x.Region).ToList();
                        locations.RemoveAll(x => regionNames.Contains(x.Region));
                        locations.AddRange(_mapper.Map<List<LocationEntity>>(regions));
                        locationStrings.RemoveAll(x => regionNames.Contains(x));
                    }

                    if (locationStrings.Any())
                    {
                        var countries = await _locationCommunicationService.GetCountriesByNamesAsync(locationStrings);

                        if (countries.Any())
                        {
                            locations.AddRange(_mapper.Map<List<LocationEntity>>(countries));
                        }
                    }
                }

                var mappedJob = new JobEntity
                {
                    ExternalId = job.Id.ToString(),
                    ExtrnalSource = "Remotive",
                    isActive = true,
                    isRemote = true,
                    Description = job.Description,
                    Title = job.Title.Trim(),
                    Url = job.Url,
                    DatePosted = DateTime.Parse(job.Publication_Date),
                    Locations = locations,
                    CompanyName = job.Company_Name.Trim(),
                    ContractTypeName = job.Job_Type.Trim(),
                    Company = new CompanyEntity { Name = job.Company_Name.Trim(), Logo = job.Company_Logo },
                    ContractType = new ContractTypeEntity { Name = job.Job_Type.Trim() },
                    Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = job.Category.Trim() } },
                    Tags = job.Tags.Select(tag => new JobTagMapping { TagName = tag.Trim() }).ToList()
                };

                mappedJobs.Add(mappedJob);
            }
            catch (Exception)
            {
                continue;
            }

        }

        return mappedJobs;
    }

    private ICollection<LocationEntity> GetEMEALocations() 
    {
        var locations = new List<LocationEntity>
        {
            new LocationEntity { Region = "Europe" },
            new LocationEntity { Region = "Africa" },
            new LocationEntity { Region = "Asia" }
        };

        return locations;
    }

    private ICollection<LocationEntity> GetLATAMLocations() 
    {
        var locations = new List<LocationEntity>
        {
            new LocationEntity { Region = "Americas" }
        };

        return locations;
    }

    private ICollection<LocationEntity> GetAPACLocations() 
    { 
        var locations = new List<LocationEntity>
        {
            new LocationEntity { Region = "Asia" },
            new LocationEntity { Region = "Oceania" }
        };

        return locations;
    }

    private bool ContainsSpecialCharacters(string input)
    {
        return input.Any(c => !char.IsLetterOrDigit(c));
    }
}
