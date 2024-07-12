using Job.Data.Access;
using Job.Data.Access.Data;
using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class RepositoryTests
{
    private readonly DbContextOptions<DataContext> _contextOptions;

    public RepositoryTests()
    {
        _contextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    // CategoryRepository Tests
    [Fact]
    public async Task CategoryRepository_GetCategoriesByNamesAsync_ReturnsCategories()
    {
        using var context = new DataContext(_contextOptions);
        context.Categories.Add(new CategoryEntity { Name = "TestCategory" });
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);
        var result = await repository.GetCategoriesByNamesAsync(new List<string> { "TestCategory" });

        Assert.Single(result);
        Assert.Equal("TestCategory", result.First().Name);
    }

    [Fact]
    public async Task CategoryRepository_GetMostVisitedCategoriesInLast30DaysAsync_ReturnsCategories()
    {
        using var context = new DataContext(_contextOptions);
        var category = new CategoryEntity { Name = "TestCategory" };
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = category } }
        };
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = Guid.NewGuid(),
            Job = job,
            ClickDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);
        var result = await repository.GetMostVisitedCategoriesInLast30DaysAsync();

        Assert.Single(result);
        Assert.Equal("TestCategory", result.First().Name);
    }

    [Fact]
    public async Task CategoryRepository_GetSavedCategoriesInLast30DaysAsync_ReturnsCategories()
    {
        using var context = new DataContext(_contextOptions);
        var category = new CategoryEntity { Name = "TestCategory" };
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = category } }
        };
        context.SavedJobs.Add(new SavedJobEntity
        {
            UserProfileId = Guid.NewGuid(),
            Job = job,
            SavedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);
        var result = await repository.GetSavedCategoriesInLast30DaysAsync();

        Assert.Single(result);
        Assert.Equal("TestCategory", result.First().Name);
    }

    [Fact]
    public async Task CategoryRepository_GetAllCategoriesAsync_ReturnsAllCategories()
    {
        using var context = new DataContext(_contextOptions);
        context.Categories.Add(new CategoryEntity { Name = "Category1" });
        context.Categories.Add(new CategoryEntity { Name = "Category2" });
        await context.SaveChangesAsync();

        var repository = new CategoryRepository(context);
        var result = await repository.GetAllCategoriesAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CategoryRepository_AddCategoriesAsync_AddsCategories()
    {
        using var context = new DataContext(_contextOptions);
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Name = "Category1" },
            new CategoryEntity { Name = "Category2" }
        };

        var repository = new CategoryRepository(context);
        await repository.AddCategoriesAsync(categories);

        Assert.Equal(2, context.Categories.Count());
    }

    // CompanyRepository Tests
    [Fact]
    public async Task CompanyRepository_GetCompanyByNameAsync_ReturnsCompany()
    {
        using var context = new DataContext(_contextOptions);
        context.Companies.Add(new CompanyEntity { Name = "TestCompany" });
        await context.SaveChangesAsync();

        var repository = new CompanyRepository(context);
        var result = await repository.GetCompanyByNameAsync("TestCompany");

        Assert.NotNull(result);
        Assert.Equal("TestCompany", result.Name);
    }

    [Fact]
    public async Task CompanyRepository_GetMostVisitedCompaniesInLast30DaysAsync_ReturnsCompanies()
    {
        using var context = new DataContext(_contextOptions);
        var company = new CompanyEntity { Name = "TestCompany" };
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = Guid.NewGuid(),
            Job = new JobEntity
            {
                Id = Guid.NewGuid(),
                Title = "TestJob",
                Description = "TestDescription",
                CompanyName = "TestCompany",
                DatePosted = DateTime.Now,
                Company = company
            },
            ClickDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new CompanyRepository(context);
        var result = await repository.GetMostVisitedCompaniesInLast30DaysAsync();

        Assert.Single(result);
        Assert.Equal("TestCompany", result.First().Name);
    }

    [Fact]
    public async Task CompanyRepository_GetMostSavedCompaniesInLast30DaysAsync_ReturnsCompanies()
    {
        using var context = new DataContext(_contextOptions);
        var company = new CompanyEntity { Name = "SavedCompany" };
        context.SavedJobs.Add(new SavedJobEntity
        {
            Job = new JobEntity
            {
                Id = Guid.NewGuid(),
                Title = "TestJob",
                Description = "TestDescription",
                CompanyName = "SavedCompany",
                DatePosted = DateTime.Now,
                Company = company
            },
            SavedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new CompanyRepository(context);
        var result = await repository.GetMostSavedCompaniesInLast30DaysAsync();

        Assert.Single(result);
        Assert.Equal("SavedCompany", result.First().Name);
    }

    [Fact]
    public async Task CompanyRepository_GetAllCompaniesAsync_ReturnsAllCompanies()
    {
        using var context = new DataContext(_contextOptions);
        context.Companies.Add(new CompanyEntity { Name = "Company1" });
        context.Companies.Add(new CompanyEntity { Name = "Company2" });
        await context.SaveChangesAsync();

        var repository = new CompanyRepository(context);
        var result = await repository.GetAllCompaniesAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CompanyRepository_AddCompanyAsync_AddsCompany()
    {
        using var context = new DataContext(_contextOptions);
        var company = new CompanyEntity { Name = "NewCompany" };

        var repository = new CompanyRepository(context);
        await repository.AddCompanyAsync(company);

        Assert.Single(context.Companies);
    }

    [Fact]
    public async Task CompanyRepository_AddCompaniesAsync_AddsCompanies()
    {
        using var context = new DataContext(_contextOptions);
        var companies = new List<CompanyEntity>
        {
            new CompanyEntity { Name = "Company1" },
            new CompanyEntity { Name = "Company2" }
        };

        var repository = new CompanyRepository(context);
        await repository.AddCompaniesAsync(companies);

        Assert.Equal(2, context.Companies.Count());
    }

    [Fact]
    public async Task CompanyRepository_UpdateCompaniesAsync_UpdatesCompanies()
    {
        using var context = new DataContext(_contextOptions);
        var company1 = new CompanyEntity { Name = "Company1", Logo = "OldLogo1" };
        var company2 = new CompanyEntity { Name = "Company2", Logo = "OldLogo2" };
        context.Companies.AddRange(company1, company2);
        await context.SaveChangesAsync();

        company1.Logo = "NewLogo1";
        company2.Logo = "NewLogo2";
        var companiesToUpdate = new List<CompanyEntity> { company1, company2 };

        var repository = new CompanyRepository(context);
        await repository.UpdateCompaniesAsync(companiesToUpdate);

        Assert.Equal("NewLogo1", context.Companies.First(c => c.Name == "Company1").Logo);
        Assert.Equal("NewLogo2", context.Companies.First(c => c.Name == "Company2").Logo);
    }

    [Fact]
    public async Task CompanyRepository_UpdateCompanyAsync_UpdatesCompany()
    {
        using var context = new DataContext(_contextOptions);
        var company = new CompanyEntity { Name = "Company", Logo = "OldLogo" };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        company.Logo = "NewLogo";
        var repository = new CompanyRepository(context);
        await repository.UpdateCompanyAsync(company);

        Assert.Equal("NewLogo", context.Companies.First(c => c.Name == "Company").Logo);
    }

    // ContractTypeRepository Tests
    [Fact]
    public async Task ContractTypeRepository_GetContractTypeByNameAsync_ReturnsContractType()
    {
        using var context = new DataContext(_contextOptions);
        context.ContractTypes.Add(new ContractTypeEntity { Name = "TestContractType" });
        await context.SaveChangesAsync();

        var repository = new ContractTypeRepository(context);
        var result = await repository.GetContractTypeByNameAsync("TestContractType");

        Assert.NotNull(result);
        Assert.Equal("TestContractType", result.Name);
    }

    [Fact]
    public async Task ContractTypeRepository_GetAllContractTypesAsync_ReturnsAllContractTypes()
    {
        using var context = new DataContext(_contextOptions);
        context.ContractTypes.Add(new ContractTypeEntity { Name = "ContractType1" });
        context.ContractTypes.Add(new ContractTypeEntity { Name = "ContractType2" });
        await context.SaveChangesAsync();

        var repository = new ContractTypeRepository(context);
        var result = await repository.GetAllContractTypesAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ContractTypeRepository_AddContractTypeAsync_AddsContractType()
    {
        using var context = new DataContext(_contextOptions);
        var contractType = new ContractTypeEntity { Name = "NewContractType" };

        var repository = new ContractTypeRepository(context);
        await repository.AddContractTypeAsync(contractType);

        Assert.Single(context.ContractTypes);
    }

    [Fact]
    public async Task ContractTypeRepository_AddContractTypesAsync_AddsContractTypes()
    {
        using var context = new DataContext(_contextOptions);
        var contractTypes = new List<ContractTypeEntity>
        {
            new ContractTypeEntity { Name = "ContractType1" },
            new ContractTypeEntity { Name = "ContractType2" }
        };

        var repository = new ContractTypeRepository(context);
        await repository.AddContractTypesAsync(contractTypes);

        Assert.Equal(2, context.ContractTypes.Count());
    }

    // ExternalSourceVisitClickRepository Tests
    [Fact]
    public async Task ExternalSourceVisitClickRepository_GetFilteredExternalSourceVisitsByUserProfileIdAsync_ReturnsFilteredVisits()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } },
            isActive = true
        };
        context.Jobs.Add(job);
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = userProfileId,
            Job = job,
            ClickDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new ExternalSourceVisitClickRepository(context);
        var result = await repository.GetFilteredExternalSourceVisitsByUserProfileIdAsync(new JobFilterDto() {Page = 0, PageSize = 5 }, userProfileId);

        Assert.Single(result.Jobs);
        Assert.Equal("TestJob", result.Jobs.First().Title);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_GetExternalSourceVisitClicksWhereEmailNotSentAsync_ReturnsClicks()
    {
        using var context = new DataContext(_contextOptions);
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = Guid.NewGuid(),
            Job = new JobEntity
            {
                Id = Guid.NewGuid(),
                Title = "TestJob",
                Description = "TestDescription",
                CompanyName = "TestCompany",
                DatePosted = DateTime.Now
            },
            ClickDate = DateTime.Now,
            isFeedbackMailSent = false
        });
        await context.SaveChangesAsync();

        var repository = new ExternalSourceVisitClickRepository(context);
        var result = await repository.GetExternalSourceVisitClicksWhereEmailNotSentAsync();

        Assert.Single(result);
        Assert.False(result.First().isFeedbackMailSent);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_GetExternalSourceVisitClicksByUserProfileIdAsync_ReturnsVisitClicks()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = userProfileId,
            Job = new JobEntity
            {
                Id = Guid.NewGuid(),
                Title = "TestJob",
                Description = "TestDescription",
                CompanyName = "TestCompany",
                DatePosted = DateTime.Now
            },
            ClickDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new ExternalSourceVisitClickRepository(context);
        var result = await repository.GetExternalSourceVisitClicksByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_GetExternalSourceVisitClickByUserProfileIdAndJobIdAsync_ReturnsVisitClick()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = userProfileId,
            JobId = jobId
        });
        await context.SaveChangesAsync();

        var repository = new ExternalSourceVisitClickRepository(context);
        var result = await repository.GetExternalSourceVisitClickByUserProfileIdAndJobIdAsync(userProfileId, jobId);

        Assert.NotNull(result);
        Assert.Equal(userProfileId, result.UserProfileId);
        Assert.Equal(jobId, result.JobId);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_GetExternalSourceVisitClicksForRecommendationsByUserProfileIdAsync_ReturnsVisitClicks()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        context.ExternalSourceVisitClicks.Add(new ExternalSourceVisitClickEntity
        {
            UserProfileId = userProfileId,
            JobId = jobId,
            ClickDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new ExternalSourceVisitClickRepository(context);
        var result = await repository.GetExternalSourceVisitClicksForRecommendationsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
        Assert.Equal(userProfileId, result.First().UserProfileId);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_AddExternalSourceVisitClickAsync_AddsVisitClick()
    {
        using var context = new DataContext(_contextOptions);
        var visitClick = new ExternalSourceVisitClickEntity
        {
            UserProfileId = Guid.NewGuid(),
            JobId = Guid.NewGuid()
        };

        var repository = new ExternalSourceVisitClickRepository(context);
        await repository.AddExternalSourceVisitClickAsync(visitClick);

        Assert.Single(context.ExternalSourceVisitClicks);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_UpdateExternalSourceVisitClicksAsync_UpdatesVisitClicks()
    {
        using var context = new DataContext(_contextOptions);
        var visitClick1 = new ExternalSourceVisitClickEntity { UserProfileId = Guid.NewGuid(), JobId = Guid.NewGuid(), isFeedbackMailSent = false };
        var visitClick2 = new ExternalSourceVisitClickEntity { UserProfileId = Guid.NewGuid(), JobId = Guid.NewGuid(), isFeedbackMailSent = false };
        context.ExternalSourceVisitClicks.AddRange(visitClick1, visitClick2);
        await context.SaveChangesAsync();

        visitClick1.isFeedbackMailSent = true;
        visitClick2.isFeedbackMailSent = true;

        var repository = new ExternalSourceVisitClickRepository(context);
        await repository.UpdateExternalSourceVisitClicksAsync(new List<ExternalSourceVisitClickEntity> { visitClick1, visitClick2 });

        Assert.True(context.ExternalSourceVisitClicks.First(v => v.UserProfileId == visitClick1.UserProfileId).isFeedbackMailSent);
        Assert.True(context.ExternalSourceVisitClicks.First(v => v.UserProfileId == visitClick2.UserProfileId).isFeedbackMailSent);
    }

    [Fact]
    public async Task ExternalSourceVisitClickRepository_DeleteExternalSourceVisitClicksAsync_DeletesVisitClicks()
    {
        using var context = new DataContext(_contextOptions);
        var visitClick1 = new ExternalSourceVisitClickEntity { UserProfileId = Guid.NewGuid(), JobId = Guid.NewGuid() };
        var visitClick2 = new ExternalSourceVisitClickEntity { UserProfileId = Guid.NewGuid(), JobId = Guid.NewGuid() };
        context.ExternalSourceVisitClicks.AddRange(visitClick1, visitClick2);
        await context.SaveChangesAsync();

        var repository = new ExternalSourceVisitClickRepository(context);
        await repository.DeleteExternalSourceVisitClicksAsync(new List<ExternalSourceVisitClickEntity> { visitClick1, visitClick2 });

        Assert.Empty(context.ExternalSourceVisitClicks);
    }

    // JobRecommendationMappingRepository Tests
    [Fact]
    public async Task JobRecommendationMappingRepository_GetJobRecommendationMappingsByUserProfileIdAsync_ReturnsMappings()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        };
        var recommendation = new RecommendationEntity
        {
            UserProfileId = userProfileId,
            CreatedDate = DateTime.Now,
            Jobs = new List<JobRecommendationMapping>
        {
            new JobRecommendationMapping
            {
                Job = job
            }
        }
        };
        context.Jobs.Add(job);
        context.Recommendations.Add(recommendation);
        await context.SaveChangesAsync();

        var repository = new JobRecommendationMappingRepository(context);
        var result = await repository.GetJobRecommendationMappingsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
    }

    [Fact]
    public async Task JobRecommendationMappingRepository_AddJobRecommendationMappingsAsync_AddsMappings()
    {
        using var context = new DataContext(_contextOptions);
        var mappings = new List<JobRecommendationMapping>
        {
            new JobRecommendationMapping { RecommendationId = Guid.NewGuid(), JobId = Guid.NewGuid() },
            new JobRecommendationMapping { RecommendationId = Guid.NewGuid(), JobId = Guid.NewGuid() }
        };

        var repository = new JobRecommendationMappingRepository(context);
        await repository.AddJobRecommendationMappingsAsync(mappings);

        Assert.Equal(2, context.JobRecommendationMappings.Count());
    }

    // JobRepository Tests
    [Fact]
    public async Task JobRepository_GetExistingJobIdsByJobIdsAsync_ReturnsJobIds()
    {
        using var context = new DataContext(_contextOptions);
        var jobId = Guid.NewGuid();
        context.Jobs.Add(new JobEntity
        {
            Id = jobId,
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
        });
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetExistingJobIdsByJobIdsAsync(new List<Guid> { jobId });

        Assert.Single(result);
        Assert.Equal(jobId, result.First());
    }

    [Fact]
    public async Task JobRepository_GetJobsByJobIdsAsync_ReturnsJobs()
    {
        using var context = new DataContext(_contextOptions);
        var jobId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = jobId,
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            ExtrnalSource = "Adzuna",
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetJobsByJobIdsAsync(new List<Guid> { jobId });

        Assert.Single(result);
        Assert.Equal("TestJob", result.First().Title);
    }

    [Fact]
    public async Task JobRepository_GetJobsForRecommendationAsync_ReturnsJobs()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            ExtrnalSource = "Adzuna",
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetJobsForRecommendationAsync(0, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task JobRepository_GetJobsWithoutDetailsForRecommendationsAsync_ReturnsJobs()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            ExtrnalSource = "Adzuna",
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetJobsWithoutDetailsForRecommendationsAsync(0, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task JobRepository_GetJobByJobIdAsync_ReturnsJob()
    {
        using var context = new DataContext(_contextOptions);
        var jobId = Guid.NewGuid();
        context.Jobs.Add(new JobEntity
        {
            Id = jobId,
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetJobByJobIdAsync(jobId);

        Assert.NotNull(result);
        Assert.Equal("TestJob", result.Title);
    }

    [Fact]
    public async Task JobRepository_GetJobDescriptionByJobIdAsync_ReturnsDescription()
    {
        using var context = new DataContext(_contextOptions);
        var jobId = Guid.NewGuid();
        context.Jobs.Add(new JobEntity
        {
            Id = jobId,
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetJobDescriptionByJobIdAsync(jobId);

        Assert.Equal("TestDescription", result);
    }

    [Fact]
    public async Task JobRepository_GetFilteredJobsAsync_ReturnsFilteredJobs()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);

        var result = await repository.GetFilteredJobsAsync(new JobFilterDto());

        Assert.NotNull(result);
    }

    [Fact]
    public async Task JobRepository_GetAllJobsAsync_ReturnsAllJobs()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetAllJobsAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task JobRepository_GetAllJobsForDeactivation_ReturnsJobs()
    {
        using var context = new DataContext(_contextOptions);
        context.Jobs.Add(new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now.AddDays(-61),
            isActive = true
        });
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var result = await repository.GetAllJobsForDeactivation();

        Assert.Single(result);
    }

    [Fact]
    public async Task JobRepository_AddJobAsync_AddsJob()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        };

        var repository = new JobRepository(context);
        await repository.AddJobAsync(job);

        Assert.Single(context.Jobs);
    }

    [Fact]
    public async Task JobRepository_AddJobsAsync_AddsJobs()
    {
        using var context = new DataContext(_contextOptions);
        var jobs = new List<JobEntity>
    {
        new JobEntity { Id = Guid.NewGuid(), Title = "TestJob1", Description = "TestDescription1", CompanyName = "TestCompany1", DatePosted = DateTime.Now },
        new JobEntity { Id = Guid.NewGuid(), Title = "TestJob2", Description = "TestDescription2", CompanyName = "TestCompany2", DatePosted = DateTime.Now }
    };

        var repository = new JobRepository(context);
        await repository.AddJobsAsync(jobs);

        Assert.Equal(2, context.Jobs.Count());
    }

    [Fact]
    public async Task JobRepository_UpdateAPIJobsAsync_UpdatesJobs()
    {
        using var context = new DataContext(_contextOptions);
        var jobId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = jobId,
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var updatedJob = new JobEntity
        {
            Id = jobId,
            Title = "UpdatedJob",
            Description = "UpdatedDescription",
            CompanyName = "UpdatedCompany",
            DatePosted = DateTime.Now
        };

        context.Entry(job).State = EntityState.Detached;
        var repository = new JobRepository(context);
        context.Entry(updatedJob).State = EntityState.Modified;
        await repository.UpdateAPIJobsAsync(new List<JobEntity> { updatedJob });

        Assert.Equal("UpdatedJob", context.Jobs.First(j => j.Id == jobId).Title);
    }


    [Fact]
    public async Task JobRepository_UpdateJobsAsync_UpdatesJobs()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        job.Title = "UpdatedJob";

        var repository = new JobRepository(context);
        await repository.UpdateJobsAsync(new List<JobEntity> { job });

        Assert.Equal("UpdatedJob", context.Jobs.First(j => j.Id == job.Id).Title);
    }

    [Fact]
    public async Task JobRepository_DeleteJobAsync_DeletesJob()
    {
        using var context = new DataContext(_contextOptions);
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        await repository.DeleteJobAsync(job);

        Assert.Empty(context.Jobs);
    }

    // LocationRepository Tests
    [Fact]
    public async Task LocationRepository_AddLocationsAsync_AddsLocations()
    {
        using var context = new DataContext(_contextOptions);
        var locations = new List<LocationEntity>
        {
            new LocationEntity { Id = Guid.NewGuid(), City = "City1" },
            new LocationEntity { Id = Guid.NewGuid(), City = "City2" }
        };

        var repository = new LocationRepository(context);
        await repository.AddLocationsAsync(locations);

        Assert.Equal(2, context.Locations.Count());
    }

    [Fact]
    public async Task LocationRepository_AddLocationAsync_AddsLocation()
    {
        using var context = new DataContext(_contextOptions);
        var location = new LocationEntity { Id = Guid.NewGuid(), City = "NewCity" };

        var repository = new LocationRepository(context);
        await repository.AddLocationAsync(location);

        Assert.Single(context.Locations);
    }

    // RecommendationRepository Tests
    [Fact]
    public async Task RecommendationRepository_GetRecommendationsByUserProfileIdAsync_ReturnsRecommendations()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        context.Recommendations.Add(new RecommendationEntity
        {
            UserProfileId = userProfileId,
            CreatedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new RecommendationRepository(context);
        var result = await repository.GetRecommendationsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
        Assert.Equal(userProfileId, result.First().UserProfileId);
    }

    [Fact]
    public async Task RecommendationRepository_GetNewRecommendedJobsByUserProfileIdAsync_ReturnsJobs()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            ExtrnalSource = "Adzuna",
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        var recommendation = new RecommendationEntity
        {
            Id = Guid.NewGuid(),
            UserProfileId = userProfileId,
            CreatedDate = DateTime.Now,
            Jobs = new List<JobRecommendationMapping>
        {
            new JobRecommendationMapping
            {
                Job = job
            }
        }
        };
        context.Jobs.Add(job);
        context.Recommendations.Add(recommendation);
        await context.SaveChangesAsync();

        var repository = new RecommendationRepository(context);
        var result = await repository.GetNewRecommendedJobsByUserProfileIdAsync(userProfileId, 1);

        Assert.Single(result);
        Assert.Equal("TestJob", result.First().Title);
    }

    [Fact]
    public async Task RecommendationRepository_GetRecommendationAfterDateByUserProfileIdAsync_ReturnsRecommendation()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var date = DateTime.Now.AddDays(-1);
        context.Recommendations.Add(new RecommendationEntity
        {
            UserProfileId = userProfileId,
            CreatedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new RecommendationRepository(context);
        var result = await repository.GetRecommendationAfterDateByUserProfileIdAsync(userProfileId, date);

        Assert.NotNull(result);
        Assert.Equal(userProfileId, result.UserProfileId);
    }

    [Fact]
    public async Task RecommendationRepository_AddRecommendationAsync_AddsRecommendation()
    {
        using var context = new DataContext(_contextOptions);
        var recommendation = new RecommendationEntity
        {
            Id = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            CreatedDate = DateTime.Now
        };

        var repository = new RecommendationRepository(context);
        await repository.AddRecommendationAsync(recommendation);

        Assert.Single(context.Recommendations);
    }

    [Fact]
    public async Task RecommendationRepository_DeleteRecommendationAsync_DeletesRecommendation()
    {
        using var context = new DataContext(_contextOptions);
        var recommendation = new RecommendationEntity
        {
            Id = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            CreatedDate = DateTime.Now
        };
        context.Recommendations.Add(recommendation);
        await context.SaveChangesAsync();

        var repository = new RecommendationRepository(context);
        await repository.DeleteRecommendationAsync(recommendation);

        Assert.Empty(context.Recommendations);
    }

    [Fact]
    public async Task RecommendationRepository_DeleteRecommendationsAsync_DeletesRecommendations()
    {
        using var context = new DataContext(_contextOptions);
        var recommendations = new List<RecommendationEntity>
        {
            new RecommendationEntity { Id = Guid.NewGuid(), UserProfileId = Guid.NewGuid(), CreatedDate = DateTime.Now },
            new RecommendationEntity { Id = Guid.NewGuid(), UserProfileId = Guid.NewGuid(), CreatedDate = DateTime.Now }
        };
        context.Recommendations.AddRange(recommendations);
        await context.SaveChangesAsync();

        var repository = new RecommendationRepository(context);
        await repository.DeleteRecommendationsAsync(recommendations);

        Assert.Empty(context.Recommendations);
    }

    // SavedJobRepository Tests
    [Fact]
    public async Task SavedJobRepository_GetFilteredSavedJobsByUserProfileIdAsync_ReturnsFilteredJobs()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            ExtrnalSource = "Adzuna",
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        context.SavedJobs.Add(new SavedJobEntity
        {
            UserProfileId = userProfileId,
            Job = job,
            SavedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new SavedJobRepository(context);
        var result = await repository.GetFilteredSavedJobsByUserProfileIdAsync(new JobFilterDto() { Page = 0, PageSize = 5}, userProfileId);

        Assert.Single(result.Jobs);
        Assert.Equal("TestJob", result.Jobs.First().Title);
    }

    [Fact]
    public async Task SavedJobRepository_GetSavedJobsByUserProfileIdAndJobIdAsync_ReturnsSavedJob()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        context.SavedJobs.Add(new SavedJobEntity
        {
            UserProfileId = userProfileId,
            JobId = jobId
        });
        await context.SaveChangesAsync();

        var repository = new SavedJobRepository(context);
        var result = await repository.GetSavedJobsByUserProfileIdAndJobIdAsync(userProfileId, jobId);

        Assert.NotNull(result);
        Assert.Equal(userProfileId, result.UserProfileId);
        Assert.Equal(jobId, result.JobId);
    }

    [Fact]
    public async Task SavedJobRepository_GetSavedJobsByUserProfileIdAsync_ReturnsSavedJobs()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        context.SavedJobs.Add(new SavedJobEntity
        {
            UserProfileId = userProfileId,
            JobId = jobId,
            Job = new JobEntity
            {
                Id = jobId,
                Title = "TestJob",
                Description = "TestDescription",
                CompanyName = "TestCompany",
                DatePosted = DateTime.Now
            },
            SavedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new SavedJobRepository(context);
        var result = await repository.GetSavedJobsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
    }

    [Fact]
    public async Task SavedJobRepository_GetSavedJobsForRecommendationsByUserProfileIdAsync_ReturnsSavedJobs()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        context.SavedJobs.Add(new SavedJobEntity
        {
            UserProfileId = userProfileId,
            JobId = jobId,
            Job = new JobEntity
            {
                Id = jobId,
                Title = "TestJob",
                Description = "TestDescription",
                CompanyName = "TestCompany",
                DatePosted = DateTime.Now
            },
            SavedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new SavedJobRepository(context);
        var result = await repository.GetSavedJobsForRecommendationsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
    }

    [Fact]
    public async Task SavedJobRepository_AddSavedJobAsync_AddsSavedJob()
    {
        using var context = new DataContext(_contextOptions);
        var savedJob = new SavedJobEntity
        {
            UserProfileId = Guid.NewGuid(),
            JobId = Guid.NewGuid()
        };

        var repository = new SavedJobRepository(context);
        await repository.AddSavedJobAsync(savedJob);

        Assert.Single(context.SavedJobs);
    }

    [Fact]
    public async Task SavedJobRepository_DeleteSavedJobAsync_DeletesSavedJob()
    {
        using var context = new DataContext(_contextOptions);
        var savedJob = new SavedJobEntity
        {
            UserProfileId = Guid.NewGuid(),
            JobId = Guid.NewGuid()
        };
        context.SavedJobs.Add(savedJob);
        await context.SaveChangesAsync();

        var repository = new SavedJobRepository(context);
        await repository.DeleteSavedJobAsync(savedJob);

        Assert.Empty(context.SavedJobs);
    }

    [Fact]
    public async Task SavedJobRepository_DeleteSavedJobsAsync_DeletesSavedJobs()
    {
        using var context = new DataContext(_contextOptions);
        var savedJobs = new List<SavedJobEntity>
        {
            new SavedJobEntity { UserProfileId = Guid.NewGuid(), JobId = Guid.NewGuid() },
            new SavedJobEntity { UserProfileId = Guid.NewGuid(), JobId = Guid.NewGuid() }
        };
        context.SavedJobs.AddRange(savedJobs);
        await context.SaveChangesAsync();

        var repository = new SavedJobRepository(context);
        await repository.DeleteSavedJobsAsync(savedJobs);

        Assert.Empty(context.SavedJobs);
    }

    // TagRepository Tests
    [Fact]
    public async Task TagRepository_AddTagsAsync_AddsTags()
    {
        using var context = new DataContext(_contextOptions);
        var tags = new List<TagEntity>
        {
            new TagEntity { Name = "Tag1" },
            new TagEntity { Name = "Tag2" }
        };

        var repository = new TagRepository(context);
        await repository.AddTagsAsync(tags);

        Assert.Equal(2, context.Tags.Count());
    }

    [Fact]
    public async Task TagRepository_GetTagsByNamesAsync_ReturnsTags()
    {
        using var context = new DataContext(_contextOptions);
        context.Tags.Add(new TagEntity { Name = "TestTag" });
        await context.SaveChangesAsync();

        var repository = new TagRepository(context);
        var result = await repository.GetTagsByNamesAsync(new List<string> { "TestTag" });

        Assert.Single(result);
        Assert.Equal("TestTag", result.First().Name);
    }

    [Fact]
    public async Task TagRepository_GetAllTagsAsync_ReturnsAllTags()
    {
        using var context = new DataContext(_contextOptions);
        context.Tags.Add(new TagEntity { Name = "Tag1" });
        context.Tags.Add(new TagEntity { Name = "Tag2" });
        await context.SaveChangesAsync();

        var repository = new TagRepository(context);
        var result = await repository.GetAllTagsAsync();

        Assert.Equal(2, result.Count);
    }

    // UserFeedbackRepository Tests
    [Fact]
    public async Task UserFeedbackRepository_GetUserFeedbackByTokenAsync_ReturnsUserFeedback()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            isActive = true,
            DatePosted = DateTime.Now,
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        context.UserFeedbacks.Add(new UserFeedbackEntity
        {
            UserProfileId = userProfileId,
            JobId = job.Id,
            Feedback = "TestFeedback",
            Token = "TestToken"
        });
        await context.SaveChangesAsync();

        var repository = new UserFeedbackRepository(context);
        var result = await repository.GetUserFeedbackByTokenAsync("TestToken", userProfileId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UserFeedbackRepository_GetFilteredFeedbacksAsync_ReturnsFilteredFeedbacks()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Title = "TestJob",
            Description = "TestDescription",
            CompanyName = "TestCompany",
            DatePosted = DateTime.Now,
            isActive = true,
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "TestCategory", Category = new CategoryEntity { Name = "TestCategory" } } },
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "TestContractType" },
            Locations = new List<LocationEntity> { new LocationEntity { City = "TestCity", Country = "TestCountry" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "TestTag" } }
        };
        context.Jobs.Add(job);
        context.UserFeedbacks.Add(new UserFeedbackEntity
        {
            UserProfileId = userProfileId,
            JobId = job.Id,
            Feedback = "TestFeedback",
            Token = "TestToken"
        });
        await context.SaveChangesAsync();

        var repository = new UserFeedbackRepository(context);
        var result = await repository.GetFilteredFeedbacksAsync(new UserFeedbackFilterDto());

        Assert.Single(result);
    }

    [Fact]
    public async Task UserFeedbackRepository_GetUserFeedbacksByUserProfileIdAsync_ReturnsUserFeedbacks()
    {
        using var context = new DataContext(_contextOptions);
        var userProfileId = Guid.NewGuid();
        context.UserFeedbacks.Add(new UserFeedbackEntity
        {
            UserProfileId = userProfileId,
            JobId = Guid.NewGuid(),
            Feedback = "TestFeedback",
            Token = "TestToken"
        });
        await context.SaveChangesAsync();

        var repository = new UserFeedbackRepository(context);
        var result = await repository.GetUserFeedbacksByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
    }

    [Fact]
    public async Task UserFeedbackRepository_AddUserFeedbacksAsync_AddsUserFeedbacks()
    {
        using var context = new DataContext(_contextOptions);
        var feedbacks = new List<UserFeedbackEntity>
    {
        new UserFeedbackEntity
        {
            UserProfileId = Guid.NewGuid(),
            JobId = Guid.NewGuid(),
            Feedback = "TestFeedback",
            Token = "TestToken"
        }
    };

        var repository = new UserFeedbackRepository(context);
        await repository.AddUserFeedbacksAsync(feedbacks);

        Assert.Single(context.UserFeedbacks);
    }

    [Fact]
    public async Task UserFeedbackRepository_DeleteUserFeedbacksAsync_DeletesUserFeedbacks()
    {
        using var context = new DataContext(_contextOptions);
        var feedbacks = new List<UserFeedbackEntity>
    {
        new UserFeedbackEntity
        {
            UserProfileId = Guid.NewGuid(),
            JobId = Guid.NewGuid(),
            Feedback = "TestFeedback",
            Token = "TestToken"
        }
    };
        context.UserFeedbacks.AddRange(feedbacks);
        await context.SaveChangesAsync();

        var repository = new UserFeedbackRepository(context);
        await repository.DeleteUserFeedbacksAsync(feedbacks);

        Assert.Empty(context.UserFeedbacks);
    }

    [Fact]
    public async Task UserFeedbackRepository_UpdateUserFeedbackAsync_UpdatesUserFeedback()
    {
        using var context = new DataContext(_contextOptions);
        var feedback = new UserFeedbackEntity
        {
            UserProfileId = Guid.NewGuid(),
            JobId = Guid.NewGuid(),
            Feedback = "TestFeedback",
            Token = "TestToken"
        };
        context.UserFeedbacks.Add(feedback);
        await context.SaveChangesAsync();

        feedback.Feedback = "UpdatedFeedback";

        var repository = new UserFeedbackRepository(context);
        await repository.UpdateUserFeedbackAsync(feedback);

        Assert.Equal("UpdatedFeedback", context.UserFeedbacks.First(f => f.UserProfileId == feedback.UserProfileId).Feedback);
    }
}
