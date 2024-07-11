using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Company;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Location.Data.Access.Helpers.DTO.City;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class SavedJobRepository : ISavedJobRepository
{
    private readonly DataContext _dataContext;

    public SavedJobRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<JobFilterResultDto> GetFilteredSavedJobsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId)
    {
        var query = _dataContext.SavedJobs
            .Where(x => x.UserProfileId == userProfileId)
            .OrderByDescending(x => x.Job.DatePosted)
            .Include(x => x.Job)
                .ThenInclude(x => x.Locations)
            .Include(x => x.Job)
                .ThenInclude(x => x.Categories)
            .Include(x => x.Job)
                .ThenInclude(x => x.ContractType)
            .Include(x => x.Job)
                .ThenInclude(x => x.Tags)
            .Include(x => x.Job)
                .ThenInclude(x => x.Company)
            .Select(x => x.Job)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(jobFilterDto.Title))
        {
            query = query.Where(x => x.Title.ToLower().Contains(jobFilterDto.Title.ToLower()) ||
                                (x.Tags != null && x.Tags.Any(y => y.TagName.ToLower().Contains(jobFilterDto.Title.ToLower()))));
        }

        if (jobFilterDto.isRemote == true)
        {
            query = query.Where(x => x.isRemote == jobFilterDto.isRemote);
        }

        if (jobFilterDto.Categories is not null && jobFilterDto.Categories.Any())
        {
            query = query.Where(x => x.Categories.Any(y => jobFilterDto.Categories.Contains(y.CategoryName)));
        }

        if (jobFilterDto.Companies is not null && jobFilterDto.Companies.Any())
        {
            query = query.Where(x => x.CompanyName != null && jobFilterDto.Companies.Contains(x.CompanyName));
        }

        if (!string.IsNullOrEmpty(jobFilterDto.ContractType))
        {
            query = query.Where(x => x.ContractTypeName == jobFilterDto.ContractType);
        }

        if (!string.IsNullOrEmpty(jobFilterDto.Country))
        {
            query = query.Where(x => x.Locations != null && x.Locations.Any(y => y.Country == jobFilterDto.Country));
        }

        if (!string.IsNullOrEmpty(jobFilterDto.State))
        {
            query = query.Where(x => x.Locations != null && x.Locations.Any(y => y.State == jobFilterDto.State));
        }

        if (!string.IsNullOrEmpty(jobFilterDto.City))
        {
            query = query.Where(x => x.Locations != null && x.Locations.Any(y => y.City == jobFilterDto.City));
        }

        var totalJobs = query.Count();

        var categories = await query
            .SelectMany(x => x.Categories)
            .Where(x => x.CategoryName != null && x.CategoryName != string.Empty)
            .Select(x => x.CategoryName)
            .Distinct()
            .ToListAsync();

        var companies = await query
            .Where(x => x.CompanyName != null && x.CompanyName != string.Empty)
            .Select(x => x.CompanyName)
            .Distinct()
            .ToListAsync();

        var contractTypes = await query
            .Where(x => x.ContractTypeName != null && x.ContractTypeName != string.Empty)
            .Select(x => x.ContractTypeName)
            .Distinct()
            .ToListAsync();


        var paginatedJobs = await query
            .Select(x => new JobEntity
            {
                Id = x.Id,
                Title = x.Title,
                DatePosted = x.DatePosted,
                DateClosed = x.DateClosed,
                isActive = x.isActive,
                isRemote = x.isRemote,
                Categories = x.Categories,
                CompanyName = x.CompanyName,
                Company = x.Company,
                Url = x.Url,
                ContractTypeName = x.ContractTypeName,
                Locations = x.Locations,
                Tags = x.Tags
            })
            .Skip(jobFilterDto.Page * jobFilterDto.PageSize)
            .Take(jobFilterDto.PageSize)
            .ToListAsync();

        var filteredJobs = paginatedJobs.Select(x => new JobDto
        {
            Id = x.Id,
            Title = x.Title,
            DatePosted = x.DatePosted,
            DateClosed = x.DateClosed,
            isActive = x.isActive,
            isRemote = x.isRemote,
            Categories = x.Categories.Select(y => y.CategoryName).ToList(),
            Company = new CompanyDto
            {
                Logo = x.Company != null ? x.Company.Logo : null,
                Name = x.CompanyName,
                Rating = x.Company.Rating,
                NumberOfRatings = x.Company.NumberOfRatings
            },
            Url = x.Url ?? string.Empty,
            ContractTypeName = x.ContractTypeName,
            Locations = x.Locations != null ? x.Locations.Select(y => new LocationDto
            {
                Region = y.Region,
                CountryIso2Code = y.Country,
                StateCode = y.State,
                City = y.City
            }).ToList() : null,
            Tags = x.Tags != null ? x.Tags.Select(y => y.TagName).ToList() : null
        }).ToList();


        var jobFilterResult = new JobFilterResultDto
        {
            Jobs = filteredJobs,
            Categories = categories,
            Companies = companies,
            ContractTypes = contractTypes,
            TotalJobs = totalJobs
        };

        return jobFilterResult;
    }

    public async Task<SavedJobEntity?> GetSavedJobsByUserProfileIdAndJobIdAsync(Guid userProfileId, Guid jobId)
    {
        return await _dataContext.SavedJobs
            .Where(x => x.UserProfileId == userProfileId && x.JobId == jobId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<SavedJobEntity>?> GetSavedJobsByUserProfileIdAsync(Guid userProfileId)
    {
        return await _dataContext.SavedJobs
            .Where(x => x.UserProfileId == userProfileId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<SavedJobEntity>?> GetSavedJobsForRecommendationsByUserProfileIdAsync(Guid userProfileId)
    {
        return await _dataContext.SavedJobs
            .Where(x => x.UserProfileId == userProfileId)
            .OrderByDescending(x => x.SavedDate)
            .Take(5)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddSavedJobAsync(SavedJobEntity savedJob)
    {
        await _dataContext.SavedJobs.AddAsync(savedJob);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteSavedJobAsync(SavedJobEntity savedJob)
    {
        _dataContext.Remove(savedJob);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteSavedJobsAsync(List<SavedJobEntity> savedJobs)
    {
        _dataContext.RemoveRange(savedJobs);
        await _dataContext.SaveChangesAsync();
    }

 
}
