using AutoMapper;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Job.Services.Contracts;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Job.Services.Business;
public class JobService : IJobService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobAPIService _jobSearchService;
    private readonly IContractTypeRepository _contractTypeRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILocationCommunicationService _locationCommunicationService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public JobService
        (
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ICompanyRepository companyRepository,
            IJobRepository jobRepository,
            IJobAPIService jobSearchService,
            IContractTypeRepository contractTypeRepository,
            ILocationRepository locationRepository,
            ILocationCommunicationService locationCommunicationService,
            IMapper mapper,
            IConfiguration configuration
        )
    {
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _companyRepository = companyRepository;
        _jobRepository = jobRepository;
        _jobSearchService = jobSearchService;
        _contractTypeRepository = contractTypeRepository;
        _locationRepository = locationRepository;
        _locationCommunicationService = locationCommunicationService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<JobFilterResultDto> GetFilteredJobsPaginatedAsync(JobFilterDto jobFilterDto)
    {
        var jobs = await _jobRepository.GetFilteredJobsAsync(jobFilterDto);

        return jobs;
    }

    public async Task<string> GetJobDescriptionByJobIdAsync(Guid jobId)
    {
        var jobDescription = await _jobRepository.GetJobDescriptionByJobIdAsync(jobId);

        if (String.IsNullOrEmpty(jobDescription))
        {
            return "";
        }

        return jobDescription;
    }

    public async Task AddJobAsync(JobDto job)
    {
        if (job.Categories is not null && job.Categories.Count > 10)
        {
            throw new ValidationException("Categories count should be less than 10");
        }

        if (job.Tags is not null && job.Tags.Count > 10)
        {
            throw new ValidationException("Tags count should be less than 10");
        }

        if (job.Locations is not null && job.Locations.Count > 10)
        {
            throw new ValidationException("Locations count should be less than 5");
        }

        var companyToBeAdded = new CompanyEntity();
        var contractTypeToBeAdded = new ContractTypeEntity();
        var categoriesToBeAdded = new List<CategoryEntity>();
        var tagsToBeAdded = new List<TagEntity>();

        var existingCompany = await _companyRepository.GetCompanyByNameAsync(job.CompanyName);
        var existingContractType = await _contractTypeRepository.GetContractTypeByNameAsync(job.ContractTypeName);
        var existingCategories = await _categoryRepository.GetCategoriesByNamesAsync(job.Categories);
        var existingTags = await _tagRepository.GetTagsByNamesAsync(job.Tags);

        if (existingCompany is null)
        {
            companyToBeAdded.Name = job.CompanyName;

            if (job.CompanyLogoFile is not null && job.CompanyLogoFile.Length > 0)
            {
                var file = job.CompanyLogoFile;
                var folderName = Path.Combine(AppConstants.RESOURCES, AppConstants.COMPANY_LOGOS, job.CompanyName);
                var pathToSave = "";
                var fullPath = "";
                var fileName = job.CompanyName.ToLower().Replace(" ", "_") + Path.GetExtension(file.FileName);
                if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") is null)
                {
                    pathToSave = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, AppConstants.MICROSERVICE_NAME, folderName);
                    fullPath = Path.Combine(pathToSave, fileName);
                    var serverAddress = AppConstants.MICROSERVICE_URL;
                    var dbPath = Path.Combine(serverAddress, folderName, fileName);
                    companyToBeAdded.Logo = dbPath;
                }
                else
                {
                    pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(AppConstants.MICROSERVICE_URL, folderName, fileName);
                    companyToBeAdded.Logo = dbPath;
                }

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }
           
            await _companyRepository.AddCompanyAsync(companyToBeAdded);
        }

        if (existingContractType is null)
        {
            contractTypeToBeAdded.Name = job.ContractTypeName;
            await _contractTypeRepository.AddContractTypeAsync(contractTypeToBeAdded);
        }

        if (existingCategories is not null)
        {
            foreach (var category in job.Categories)
            {
                var existingCategory = existingCategories.FirstOrDefault(x => x.Name == category);
                if (existingCategory is null)
                {
                    categoriesToBeAdded.Add(new CategoryEntity { Name = category });
                }
            }
        }
        else { 
            categoriesToBeAdded = job.Categories.Select(x => new CategoryEntity { Name = x }).ToList();
        }

        if (existingTags is not null)
        {
            foreach (var tag in job.Tags)
            {
                var existingTag = existingTags.FirstOrDefault(x => x.Name == tag);
                if (existingTag is null)
                {
                    tagsToBeAdded.Add(new TagEntity { Name = tag });
                }
            }
        }
        else
        {
            tagsToBeAdded = job.Tags.Select(x => new TagEntity { Name = x }).ToList();
        }   

        var jobEntity = _mapper.Map<JobEntity>(job);
        jobEntity.Company = null;

        jobEntity.isActive = true;
        jobEntity.DatePosted = DateTime.Now;

        foreach (var jobLocation in jobEntity.Locations)
        {
            var region = await _locationCommunicationService.GetRegionByCountryIso2CodeAsync(jobLocation.Country);
            
            if (region is not null)
            {
                jobLocation.Region = region;
            }
        }

        await _categoryRepository.AddCategoriesAsync(categoriesToBeAdded);
        await _tagRepository.AddTagsAsync(tagsToBeAdded);

        await _jobRepository.AddJobAsync(jobEntity);
    }

    public async Task AddJobsFromAPIsAsync()
    {
        var jobSearch = new JobFilterDto();

        var jobs = new List<JobEntity>();

        jobs.AddRange(await _jobSearchService.GetAdzunaJobsAsync(jobSearch));
        jobs.AddRange(await _jobSearchService.GetJobicyJobsAsync(jobSearch));
        jobs.AddRange(await _jobSearchService.GetRemotiveJobsAsync(jobSearch));

        await AddJobsAsync(jobs);
    }

    public async Task DeactivateJobsAsync()
    {
        var jobs = await _jobRepository.GetAllJobsForDeactivation();

        foreach (var job in jobs) {
            job.isActive = false;
            job.DateClosed = DateTime.Now;
        }

        await _jobRepository.UpdateJobsAsync(jobs);
    }

    public async Task DeleteJobByJobIdAsync(Guid jobId)
    {
        var job = await _jobRepository.GetJobByJobIdAsync(jobId);

        if (job is null)
        {
            throw new Exception("Job not found");
        }

        await _jobRepository.DeleteJobAsync(job);
    }

    private async Task AddJobsAsync(List<JobEntity> jobs)
    {
        var existingJobs = await _jobRepository.GetAllJobsAsync();
        var existingCategories = await _categoryRepository.GetAllCategoriesAsync();
        var existingTags = await _tagRepository.GetAllTagsAsync();
        var existingCompanies = await _companyRepository.GetAllCompaniesAsync();
        var existingContractTypes = await _contractTypeRepository.GetAllContractTypesAsync();

        CategorizeJobsTagsCategoriesAndCompanies(
            jobs, existingJobs, existingCategories, existingTags, existingCompanies, existingContractTypes,
            out List<CategoryEntity> categoriesToBeAdded, out List<TagEntity> tagsToBeAdded,
            out List<CompanyEntity> companiesToBeAdded, out List<ContractTypeEntity> contractTypesToBeAdded,
            out List<CompanyEntity> companiesToBeUpdated, out List<JobEntity> jobsToBeAdded, out List<JobEntity> jobsToBeUpdated);

        if (categoriesToBeAdded.Any())
        {
            try
            {
                await _categoryRepository.AddCategoriesAsync(categoriesToBeAdded);
            }
            catch (Exception)
            {
            }
        }

        if (tagsToBeAdded.Any())
        {
            try
            {
                await _tagRepository.AddTagsAsync(tagsToBeAdded);
            }
            catch (Exception)
            {

            }
        }

        if (companiesToBeAdded.Any())
        {
            try
            {
                await _companyRepository.AddCompaniesAsync(companiesToBeAdded);
            }
            catch (Exception)
            {

            }
        }

        if (contractTypesToBeAdded.Any())
        {
            try
            {
                await _contractTypeRepository.AddContractTypesAsync(contractTypesToBeAdded);
            }
            catch (Exception)
            {

            }
        }

        if (companiesToBeUpdated.Any())
        {
            try
            {
                await _companyRepository.UpdateCompaniesAsync(companiesToBeUpdated);
            }
            catch (Exception)
            {

            }
        }

        if (jobsToBeUpdated.Any())
        {
            try
            {
                await _jobRepository.UpdateAPIJobsAsync(jobsToBeUpdated);
            }
            catch (Exception)
            {

            }
        }

        if (jobsToBeAdded.Any())
        {
            try
            {
                await _jobRepository.AddJobsAsync(jobsToBeAdded);
            }
            catch (Exception)
            {

            }
        }
    }

    private void CategorizeJobsTagsCategoriesAndCompanies(
        List<JobEntity> jobs, List<JobEntity> existingJobs,
        List<CategoryEntity> existingCategories, List<TagEntity> existingTags,
        List<CompanyEntity> existingCompanies, List<ContractTypeEntity> existingContractTypes,
        out List<CategoryEntity> categoriesToBeAdded, out List<TagEntity> tagsToBeAdded, out List<CompanyEntity> companiesToBeAdded,
        out List<ContractTypeEntity> contractTypesToBeAdded, out List<CompanyEntity> companiesToBeUpdated,
        out List<JobEntity> jobsToBeAdded, out List<JobEntity> jobsToBeUpdated)
    {
        categoriesToBeAdded = new List<CategoryEntity>();

        contractTypesToBeAdded = new List<ContractTypeEntity>();

        tagsToBeAdded = new List<TagEntity>();

        companiesToBeAdded = new List<CompanyEntity>();
        companiesToBeUpdated = new List<CompanyEntity>();

        jobsToBeAdded = new List<JobEntity>();
        jobsToBeUpdated = new List<JobEntity>();

        foreach (var job in jobs)
        {
            try
            {
                var existingJob = existingJobs.FirstOrDefault(x => x.ExternalId == job.ExternalId && x.ExtrnalSource == job.ExtrnalSource);
                if (existingJob != null)
                {
                    if (CheckIfJobIsUpdated(existingJob, job))
                    {
                        if (existingJob.DatePosted != job.DatePosted)
                        {
                            TimeSpan timeSpan = existingJob.DatePosted - job.DatePosted;

                            if (Math.Abs(timeSpan.TotalDays) - 30.0 < 30)
                            {
                                existingJob.isActive = true;
                                existingJob.DateClosed = null;
                            }
                            existingJob.DatePosted = job.DatePosted;
                        }

                        if (existingJob.CompanyName is not null && job.CompanyName is not null &&
                            !existingJob.CompanyName.ToLower().Equals(job.CompanyName.ToLower()))
                        {
                            var existingCompany = existingCompanies.FirstOrDefault(x => x.Name.ToLower().Equals(job.CompanyName.ToLower()));
                            if (existingCompany is not null)
                            {
                                existingJob.CompanyName = existingCompany.Name;
                                if (existingCompany.Logo != job.Company.Logo)
                                {
                                    existingCompany.Logo = job.Company.Logo;
                                    companiesToBeUpdated.Add(existingCompany);
                                }
                            }
                            else if (job.Company is not null)
                            {
                                companiesToBeAdded.Add(job.Company);
                                existingCompanies.Add(job.Company);
                            }
                        }

                        existingJob.Title = job.Title;
                        existingJob.Description = job.Description;
                        existingJob.Company = null;
                        existingJob.Locations = job.Locations;
                        existingJob.Url = job.Url;
                        existingJob.CompanyName = job.CompanyName;
                        existingJob.ContractTypeName = job.ContractTypeName;
                        existingJob.ContractType = null;
                        if (job.Categories is not null && job.Categories.Any())
                        {
                            existingJob.Categories = job.Categories;
                        }
                        existingJob.Tags = job.Tags;

                        jobsToBeUpdated.Add(existingJob);
                    }
                    continue;
                }

                jobsToBeAdded.Add(job);

                var company = existingCompanies.FirstOrDefault(x => job.CompanyName != null && x.Name.ToLower().Equals(job.CompanyName.ToLower()));

                if (company is null && job.Company is not null)
                {
                    companiesToBeAdded.Add(job.Company);
                    existingCompanies.Add(job.Company);
                }
                else if (company is not null)
                {
                    job.CompanyName = company.Name;
                }

                if (job.ContractType is not null)
                {
                    var existingContractType = existingContractTypes.FirstOrDefault(x => x.Name.ToLower().Equals(job.ContractTypeName.ToLower()));
                    if (existingContractType is null)
                    {
                        contractTypesToBeAdded.Add(job.ContractType);
                        existingContractTypes.Add(job.ContractType);
                    }
                    else
                    {
                        job.ContractTypeName = existingContractType.Name;
                    }
                }

                if (job.Categories is not null)
                {
                    foreach (var category in job.Categories)
                    {
                        var existingCategory = existingCategories.FirstOrDefault(x => x.Name.ToLower().Equals(category.CategoryName.ToLower()));
                        if (existingCategory is null)
                        {
                            categoriesToBeAdded.Add(new CategoryEntity { Name = category.CategoryName });
                            existingCategories.Add(new CategoryEntity { Name = category.CategoryName });
                        }
                    }
                }

                if (job.Tags is not null)
                {
                    foreach (var tag in job.Tags)
                    {
                        var existingTag = existingTags.FirstOrDefault(x => x.Name.ToLower().Equals(tag.TagName.ToLower()));
                        if (existingTag is null)
                        {
                            tagsToBeAdded.Add(new TagEntity { Name = tag.TagName });
                            existingTags.Add(new TagEntity { Name = tag.TagName });
                        }
                    }
                }

                job.Company = null;
                job.ContractType = null;
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    private bool CheckIfJobIsUpdated(JobEntity existingJob, JobEntity job)
    {
        if (existingJob.Title != job.Title || existingJob.Description != job.Description || existingJob.CompanyName != job.CompanyName ||
            existingJob.DatePosted != job.DatePosted || existingJob.Company.Logo != job.Company.Logo || existingJob.Url != job.Url || 
            existingJob.ContractTypeName != job.ContractTypeName || existingJob.Categories.Count != job.Categories.Count || 
            existingJob.Categories.Any(x => !job.Categories.Any(y => y.CategoryName == x.CategoryName)) ||
            (existingJob.Tags.Any() && (existingJob.Tags.Count != job.Tags.Count || existingJob.Tags.Any(x => !job.Tags.Any(y => y.TagName == x.TagName)))) ||
            existingJob.Locations.Count != job.Locations.Count)
        {
            return true;
        }

        return false;
    }
}
