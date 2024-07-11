using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers;
using Job.Data.Contracts.Helpers.DTO.Company;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Location.Data.Access.Helpers.DTO.City;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class JobRepository : IJobRepository
{
    private readonly DataContext _dataContext;

    public JobRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<Guid>> GetExistingJobIdsByJobIdsAsync(List<Guid> jobIds)
    {
        return await _dataContext.Jobs
            .Where(x => jobIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<JobEntity>> GetJobsByJobIdsAsync(List<Guid> jobIds)
    {
        return await _dataContext.Jobs
            .Where(x => jobIds.Contains(x.Id))
            .Include(x => x.Categories)
            .Include(x => x.ContractType)
            .Include(x => x.Tags)
            .Include(x => x.Company)
            .Include(x => x.Locations)
            .Select(x => new JobEntity
            {
                Id = x.Id,
                Categories = x.Categories,
                Company = x.Company,
                CompanyName = x.CompanyName,
                ContractTypeName = x.ContractTypeName,
                DateClosed = x.DateClosed,
                DatePosted = x.DatePosted,
                isActive = x.isActive,
                isRemote = x.isRemote,
                Locations = x.Locations,
                Tags = x.Tags,
                Title = x.Title,
                Url = x.Url
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<JobEntity>> GetJobsForRecommendationAsync(int page, int pageSize)
    {
        return await _dataContext.Jobs
            .Where(x => x.isActive)
            .Skip(page * pageSize)
            .Take(pageSize)
            .OrderByDescending(x => x.DatePosted)
            .Include(x => x.Categories)
            .Include(x => x.ContractType)
            .Include(x => x.Tags)
            .Include(x => x.Company)
            .Include(x => x.Locations)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<JobEntity>> GetJobsWithoutDetailsForRecommendationsAsync(int page, int pageSize)
    {
        return await _dataContext.Jobs
            .Where(x => x.isActive)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(x => new JobEntity
            {
                Id = x.Id,
                Categories = x.Categories,
                Company = x.Company,
                CompanyName = x.CompanyName,
                ContractTypeName = x.ContractTypeName,
                isActive = x.isActive,
                isRemote = x.isRemote,
                Locations = x.Locations,
                Tags = x.Tags,
                Title = x.Title,
                Url = x.Url
            })
            .OrderBy(x => x.DatePosted)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<JobEntity?> GetJobByJobIdAsync(Guid jobId)
    {
        return await _dataContext.Jobs
            .Where(x => x.Id == jobId)
            .Include(x => x.Locations)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetJobDescriptionByJobIdAsync(Guid jobId)
    {
        return await _dataContext.Jobs
            .Where(x => x.Id == jobId)
            .Select(x => x.Description)
            .FirstOrDefaultAsync();
    }

    public async Task<JobFilterResultDto> GetFilteredJobsAsync(JobFilterDto jobFilterDto)
    {
        var query = _dataContext.Jobs
            .OrderByDescending(x => x.DatePosted)
            .Include(x => x.Categories)
            .Include(x => x.ContractType)
            .Include(x => x.Tags)
            .Include(x => x.Company)
            .Include(x => x.Locations)
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
                Categories = x.Categories,
                Company = x.Company,
                CompanyName = x.CompanyName,
                ContractTypeName = x.ContractTypeName,
                DateClosed = x.DateClosed,
                DatePosted = x.DatePosted,
                isActive = x.isActive,
                isRemote = x.isRemote,
                Locations = x.Locations,
                Tags = x.Tags,
                Title = x.Title,
                Url = x.Url
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

    public async Task<List<JobEntity>> GetAllJobsAsync()
    {
        return await _dataContext.Jobs
            .Include(x => x.Categories)
            .Include(x => x.ContractType)
            .Include(x => x.Tags)
            .Include(x => x.Company)
            .Include(x => x.Locations)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<JobEntity>> GetAllJobsForDeactivation()
    {
        var date = DateTime.UtcNow.AddDays(-60);

        return await _dataContext.Jobs
            .Where(x => x.isActive && x.DatePosted < date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddJobAsync(JobEntity job)
    {
        await _dataContext.Jobs.AddAsync(job);
        await _dataContext.SaveChangesAsync();
    }   

    public async Task AddJobsAsync(List<JobEntity> jobs)
    {
        await _dataContext.Jobs.AddRangeAsync(jobs);
        await _dataContext.SaveChangesAsync();
    }

    public async Task UpdateAPIJobsAsync(List<JobEntity> jobs)
    {
        var jobsToBeUpdatedIds = jobs.Select(x => x.Id).ToList();
        var oldJobs = await _dataContext.Jobs
            .Where(x => jobsToBeUpdatedIds.Contains(x.Id))
            .Include(x => x.Categories)
            .Include(x => x.ContractType)
            .Include(x => x.Tags)
            .Include(x => x.Company)
            .Include(x => x.Locations)
            .AsNoTracking()
            .ToListAsync();

        var jobCategorieToBeDeleted = new List<JobCategoryMapping>();
        var jobCategoriesToBeAdded = new List<JobCategoryMapping>();

        var jobTagsToBeDeleted = new List<JobTagMapping>();
        var jobTagsToBeAdded = new List<JobTagMapping>();

        var jobLocationToBeDeleted = new List<LocationEntity>();

        foreach (var job in jobs)
        {

            var oldJob = oldJobs.FirstOrDefault(x => x.Id == job.Id);

            if (oldJob == null)
            {
                continue;
            }

            jobCategorieToBeDeleted.AddRange(oldJob.Categories
                .Where(x => !job.Categories.Any(y => y.CategoryName.ToLower().Equals(x.CategoryName.ToLower()))).ToList());
            jobCategoriesToBeAdded.AddRange(job.Categories
                .Where(x => !oldJob.Categories.Any(y => y.CategoryName.ToLower().Equals(x.CategoryName.ToLower()))).ToList());

            if (oldJob.Tags is not null && oldJob.Tags.Any())
            {
                jobTagsToBeDeleted.AddRange(oldJob.Tags
                    .Where(x => job.Tags != null && !job.Tags.Any(y => y.TagName.ToLower().Equals(x.TagName.ToLower()))).ToList());

                if (job.Tags is not null)
                {
                    jobTagsToBeAdded.AddRange(job.Tags
                        .Where(x => !oldJob.Tags.Any(y => y.TagName.ToLower().Equals(x.TagName.ToLower()))).ToList());
                }
            }

            if (oldJob.Locations is not null && oldJob.Locations.Any())
            {
                jobLocationToBeDeleted.AddRange(oldJob.Locations);
            }

            job.Categories = jobCategoriesToBeAdded;
            job.Tags = jobTagsToBeAdded;
        }

        jobCategoriesToBeAdded.ForEach(x => x.Job = null);
        jobTagsToBeAdded.ForEach(x => x.Job = null);
        jobLocationToBeDeleted.ForEach(x => x.Job = null);

        _dataContext.JobCategoryMappings.RemoveRange(jobCategorieToBeDeleted);
        _dataContext.JobTagMappings.RemoveRange(jobTagsToBeDeleted);
        _dataContext.Locations.RemoveRange(jobLocationToBeDeleted);

        var test = jobs.Where(x => x.Company != null).ToList();

        _dataContext.Jobs.UpdateRange(jobs);
        await _dataContext.SaveChangesAsync();
    }

    public async Task UpdateJobsAsync(List<JobEntity> jobs)
    {
        _dataContext.Jobs.UpdateRange(jobs);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteJobAsync(JobEntity job)
    {
        _dataContext.Jobs.Remove(job);
        await _dataContext.SaveChangesAsync();
    }
}
