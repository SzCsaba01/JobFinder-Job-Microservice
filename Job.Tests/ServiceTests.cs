using AutoMapper;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Company;
using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Contracts.Helpers.Enums;
using Job.Data.Object.Entities;
using Job.Services.Business;
using Job.Services.Contracts;
using Location.Data.Access.Helpers.DTO.City;
using Microsoft.AspNetCore.Hosting.Server;
using Moq;
using Xunit;

public class ServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ICompanyRepository> _mockCompanyRepository;
    private readonly Mock<ILocationRepository> _mockLocationRepository;
    private readonly Mock<IContractTypeRepository> _mockContractTypeRepository;
    private readonly Mock<IExternalSourceVisitClickRepository> _mockExternalSourceVisitClickRepository;
    private readonly Mock<IJobRepository> _mockJobRepository;
    private readonly Mock<IJobRecommendationMappingRepository> _mockJobRecommendationMappingRepository;
    private readonly Mock<IRecommendationRepository> _mockRecommendationRepository;
    private readonly Mock<ISavedJobRepository> _mockSavedJobRepository;
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IUserFeedbackRepository> _mockUserFeedbackRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocationCommunicationService> _mockLocationCommunicationService;
    private readonly Mock<IJobAPIService> _mockJobAPIService;
    private readonly Mock<IServer> _mockServer;

    private readonly CategoryService _categoryService;
    private readonly CompanyService _companyService;
    private readonly ContractTypeService _contractTypeService;
    private readonly ExternalSourceVisitClickService _externalSourceVisitClickService;
    private readonly JobService _jobService;
    private readonly JobRecommendationService _jobRecommendationService;
    private readonly SavedJobService _savedJobService;
    private readonly TagService _tagService;
    private readonly UserFeedbackService _userFeedbackService;

    public ServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _mockLocationRepository = new Mock<ILocationRepository>();
        _mockContractTypeRepository = new Mock<IContractTypeRepository>();
        _mockExternalSourceVisitClickRepository = new Mock<IExternalSourceVisitClickRepository>();
        _mockJobRepository = new Mock<IJobRepository>();
        _mockJobRecommendationMappingRepository = new Mock<IJobRecommendationMappingRepository>();
        _mockRecommendationRepository = new Mock<IRecommendationRepository>();
        _mockSavedJobRepository = new Mock<ISavedJobRepository>();
        _mockTagRepository = new Mock<ITagRepository>();
        _mockUserFeedbackRepository = new Mock<IUserFeedbackRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLocationCommunicationService = new Mock<ILocationCommunicationService>();
        _mockJobAPIService = new Mock<IJobAPIService>();
        _mockServer = new Mock<IServer>();

        _categoryService = new CategoryService(_mockCategoryRepository.Object);

        _companyService = new CompanyService(_mockCompanyRepository.Object);

        _contractTypeService = new ContractTypeService(_mockContractTypeRepository.Object);

        _externalSourceVisitClickService = new ExternalSourceVisitClickService(_mockExternalSourceVisitClickRepository.Object);

        _jobRecommendationService = new JobRecommendationService(
            _mockRecommendationRepository.Object,
            _mockJobRecommendationMappingRepository.Object,
            _mockJobRepository.Object,
            _mockMapper.Object
        );

        _savedJobService = new SavedJobService(_mockSavedJobRepository.Object);

        _tagService = new TagService(_mockTagRepository.Object);

        _userFeedbackService = new UserFeedbackService(
            _mockUserFeedbackRepository.Object,
            _mockMapper.Object,
            _mockCompanyRepository.Object
        );

        _jobService = new JobService(
            _mockCategoryRepository.Object,
            _mockTagRepository.Object,
            _mockCompanyRepository.Object,
            _mockJobRepository.Object,
            _mockJobAPIService.Object,
            _mockContractTypeRepository.Object,
            _mockLocationRepository.Object,
            _mockLocationCommunicationService.Object,
            _mockMapper.Object,
            _mockServer.Object
        );
    }

    // CategoryService Tests
    [Fact]
    public async Task CategoryService_GetAllCategoriesAsync_ReturnsAllCategories()
    {
        var categories = new List<CategoryEntity>
    {
        new CategoryEntity { Name = "Category1" },
        new CategoryEntity { Name = "Category2" }
    };
        _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync()).ReturnsAsync(categories);

        var result = await _categoryService.GetAllCategoriesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains("Category1", result);
        Assert.Contains("Category2", result);
    }

    [Fact]
    public async Task CategoryService_GetMostVisitedCategoriesInLast30DaysAsync_ReturnsMostVisitedCategories()
    {
        var categories = new List<CategoryEntity>
    {
        new CategoryEntity { Name = "Category1" },
        new CategoryEntity { Name = "Category2" },
        new CategoryEntity { Name = "Category1" }
    };
        _mockCategoryRepository.Setup(repo => repo.GetMostVisitedCategoriesInLast30DaysAsync()).ReturnsAsync(categories);

        var result = await _categoryService.GetMostVisitedCategoriesInLast30DaysAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Category1", result[0].Name);
        Assert.Equal(2, result[0].Count);
        Assert.Equal("Category2", result[1].Name);
        Assert.Equal(1, result[1].Count);
    }

    [Fact]
    public async Task CategoryService_GetMostSavedCategoriesInLast30DaysAsync_ReturnsMostSavedCategories()
    {
        var categories = new List<CategoryEntity>
    {
        new CategoryEntity { Name = "Category1" },
        new CategoryEntity { Name = "Category2" },
        new CategoryEntity { Name = "Category1" }
    };
        _mockCategoryRepository.Setup(repo => repo.GetSavedCategoriesInLast30DaysAsync()).ReturnsAsync(categories);

        var result = await _categoryService.GetMostSavedCategoriesInLast30DaysAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Category1", result[0].Name);
        Assert.Equal(2, result[0].Count);
        Assert.Equal("Category2", result[1].Name);
        Assert.Equal(1, result[1].Count);
    }

    // CompanyService Tests
    [Fact]
    public async Task CompanyService_GetAllCompaniesAsync_ReturnsAllCompanies()
    {
        var companies = new List<CompanyEntity>
    {
        new CompanyEntity { Name = "Company1" },
        new CompanyEntity { Name = "Company2" }
    };
        _mockCompanyRepository.Setup(repo => repo.GetAllCompaniesAsync()).ReturnsAsync(companies);

        var result = await _companyService.GetAllCompaniesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains("Company1", result);
        Assert.Contains("Company2", result);
    }

    [Fact]
    public async Task CompanyService_GetMostVisitedCompaniesInLast30DaysAsync_ReturnsMostVisitedCompanies()
    {
        var companies = new List<CompanyEntity>
    {
        new CompanyEntity { Name = "Company1", Logo = "Logo1", NumberOfRatings = 10, Rating = 4.5f },
        new CompanyEntity { Name = "Company2", Logo = "Logo2", NumberOfRatings = 20, Rating = 3.5f },
        new CompanyEntity { Name = "Company1", Logo = "Logo1", NumberOfRatings = 10, Rating = 4.5f }
    };
        _mockCompanyRepository.Setup(repo => repo.GetMostVisitedCompaniesInLast30DaysAsync()).ReturnsAsync(companies);

        var result = await _companyService.GetMostVisitedCompaniesInLast30DaysAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Company1", result[0].Name);
        Assert.Equal(2, result[0].Count);
        Assert.Equal("Company2", result[1].Name);
        Assert.Equal(1, result[1].Count);
    }

    [Fact]
    public async Task CompanyService_GetMostSavedCompaniesInLast30DaysAsync_ReturnsMostSavedCompanies()
    {
        var companies = new List<CompanyEntity>
    {
        new CompanyEntity { Name = "Company1", Logo = "Logo1", NumberOfRatings = 10, Rating = 4.5f },
        new CompanyEntity { Name = "Company2", Logo = "Logo2", NumberOfRatings = 20, Rating = 3.5f },
        new CompanyEntity { Name = "Company1", Logo = "Logo1", NumberOfRatings = 10, Rating = 4.5f }
    };
        _mockCompanyRepository.Setup(repo => repo.GetMostSavedCompaniesInLast30DaysAsync()).ReturnsAsync(companies);

        var result = await _companyService.GetMostSavedCompaniesInLast30DaysAsync();
        Assert.Equal(2, result.Count);
        Assert.Equal("Company1", result[0].Name);
        Assert.Equal(2, result[0].Count);
        Assert.Equal("Company2", result[1].Name);
        Assert.Equal(1, result[1].Count);
    }

    // ContractTypeService Tests
    [Fact]
    public async Task ContractTypeService_GetAllContractTypesAsync_ReturnsAllContractTypes()
    {
        var contractTypes = new List<ContractTypeEntity>
    {
        new ContractTypeEntity { Name = "Full-Time" },
        new ContractTypeEntity { Name = "Part-Time" }
    };
        _mockContractTypeRepository.Setup(repo => repo.GetAllContractTypesAsync()).ReturnsAsync(contractTypes);

        var result = await _contractTypeService.GetAllContractTypesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains("Full-Time", result);
        Assert.Contains("Part-Time", result);
    }

    // ExternalSourceVisitClickService Tests
    [Fact]
    public async Task ExternalSourceVisitClickService_GetFilteredExternalSourceVisitsByUserProfileIdAsync_ReturnsFilteredVisits()
    {
        var userProfileId = Guid.NewGuid();
        var jobFilterDto = new JobFilterDto();
        var jobFilterResultDto = new JobFilterResultDto { Jobs = new List<JobDto> { new JobDto { Title = "TestJob" } } };
        _mockExternalSourceVisitClickRepository.Setup(repo => repo.GetFilteredExternalSourceVisitsByUserProfileIdAsync(jobFilterDto, userProfileId))
            .ReturnsAsync(jobFilterResultDto);

        var result = await _externalSourceVisitClickService.GetFilteredExternalSourceVisitsByUserProfileIdAsync(jobFilterDto, userProfileId);

        Assert.Single(result.Jobs);
        Assert.Equal("TestJob", result.Jobs.First().Title);
    }

    [Fact]
    public async Task ExternalSourceVisitClickService_AddExternalSourceVisitClickAsync_AddsVisitClick()
    {
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        _mockExternalSourceVisitClickRepository.Setup(repo => repo.GetExternalSourceVisitClickByUserProfileIdAndJobIdAsync(userProfileId, jobId))
            .ReturnsAsync((ExternalSourceVisitClickEntity)null);

        await _externalSourceVisitClickService.AddExternalSourceVisitClickAsync(userProfileId, jobId);

        _mockExternalSourceVisitClickRepository.Verify(repo => repo.AddExternalSourceVisitClickAsync(It.IsAny<ExternalSourceVisitClickEntity>()), Times.Once);
    }

    // JobService Tests
    [Fact]
    public async Task JobService_GetFilteredJobsPaginatedAsync_ReturnsFilteredJobs()
    {
        var jobFilterDto = new JobFilterDto();
        var jobFilterResultDto = new JobFilterResultDto { Jobs = new List<JobDto> { new JobDto { Title = "TestJob" } } };
        _mockJobRepository.Setup(repo => repo.GetFilteredJobsAsync(jobFilterDto)).ReturnsAsync(jobFilterResultDto);

        var result = await _jobService.GetFilteredJobsPaginatedAsync(jobFilterDto);

        Assert.Single(result.Jobs);
        Assert.Equal("TestJob", result.Jobs.First().Title);
    }

    [Fact]
    public async Task JobService_GetJobDescriptionByJobIdAsync_ReturnsJobDescription()
    {
        var jobId = Guid.NewGuid();
        var jobDescription = "TestDescription";
        _mockJobRepository.Setup(repo => repo.GetJobDescriptionByJobIdAsync(jobId)).ReturnsAsync(jobDescription);

        var result = await _jobService.GetJobDescriptionByJobIdAsync(jobId);

        Assert.Equal(jobDescription, result);
    }

    // JobService Tests
    [Fact]
    public async Task JobService_AddJobAsync_AddsJob()
    {
        var jobDto = new JobDto
        {
            Company = new CompanyDto { Name = "TestCompany" },
            ContractTypeName = "Full-Time",
            Categories = new List<string> { "Category1", "Category2" },
            Tags = new List<string> { "Tag1", "Tag2" },
            Locations = new List<LocationDto> { new LocationDto { City = "City1", Country = "Country1", Region = "Region"} },
            Title = "TestJob",
            Description = "TestDescription",
            DatePosted = DateTime.UtcNow,
            Url = "TestUrl"
        };
        var jobEntity = new JobEntity { 
            Title = "TestJob",
            Company = new CompanyEntity { Name = "TestCompany" },
            ContractType = new ContractTypeEntity { Name = "Full-Time" },
            Categories = new List<JobCategoryMapping> { new JobCategoryMapping { CategoryName = "Category1" }, new JobCategoryMapping { CategoryName = "Category2" } },
            Tags = new List<JobTagMapping> { new JobTagMapping { TagName = "Tag1" }, new JobTagMapping { TagName = "Tag2" } },
            Locations = new List<LocationEntity> { new LocationEntity { City = "City1", Country = "Country1", Region = "Region" } },
            Description = "TestDescription",
            DatePosted = DateTime.UtcNow,
            Url = "TestUrl"
        };
        _mockMapper.Setup(m => m.Map<JobEntity>(jobDto)).Returns(jobEntity);
        _mockCompanyRepository.Setup(repo => repo.GetCompanyByNameAsync(It.IsAny<string>())).ReturnsAsync((CompanyEntity)null);
        _mockContractTypeRepository.Setup(repo => repo.GetContractTypeByNameAsync(It.IsAny<string>())).ReturnsAsync((ContractTypeEntity)null);
        _mockCategoryRepository.Setup(repo => repo.GetCategoriesByNamesAsync(It.IsAny<List<string>>())).ReturnsAsync(new List<CategoryEntity>());
        _mockTagRepository.Setup(repo => repo.GetTagsByNamesAsync(It.IsAny<List<string>>())).ReturnsAsync(new List<TagEntity>());
        _mockLocationCommunicationService.Setup(service => service.GetRegionByCountryIso2CodeAsync(It.IsAny<string>())).ReturnsAsync("Region");

        await _jobService.AddJobAsync(jobDto);

        _mockJobRepository.Verify(repo => repo.AddJobAsync(It.Is<JobEntity>(j => j.Title == jobEntity.Title)), Times.Once);
    }

    [Fact]
    public async Task JobService_DeactivateJobsAsync_DeactivatesJobs()
    {
        var jobs = new List<JobEntity> { new JobEntity { Title = "TestJob", isActive = true } };
        _mockJobRepository.Setup(repo => repo.GetAllJobsForDeactivation()).ReturnsAsync(jobs);

        await _jobService.DeactivateJobsAsync();

        Assert.False(jobs.First().isActive);
        _mockJobRepository.Verify(repo => repo.UpdateJobsAsync(jobs), Times.Once);
    }

    [Fact]
    public async Task JobService_DeleteJobByJobIdAsync_DeletesJob()
    {
        var jobId = Guid.NewGuid();
        var job = new JobEntity { Id = jobId, Title = "TestJob" };
        _mockJobRepository.Setup(repo => repo.GetJobByJobIdAsync(jobId)).ReturnsAsync(job);

        await _jobService.DeleteJobByJobIdAsync(jobId);

        _mockJobRepository.Verify(repo => repo.DeleteJobAsync(job), Times.Once);
    }

    // JobRecommendationService Tests
    [Fact]
    public async Task JobRecommendationService_GetRecommendedJobsByUserProfileIdAsync_ReturnsRecommendedJobs()
    {
        var userProfileId = Guid.NewGuid();
        var jobs = new List<JobEntity> { new JobEntity { Title = "TestJob" } };
        var jobDtos = new List<JobDto> { new JobDto { Title = "TestJob" } };
        _mockRecommendationRepository.Setup(repo => repo.GetNewRecommendedJobsByUserProfileIdAsync(userProfileId, 3)).ReturnsAsync(jobs);
        _mockMapper.Setup(m => m.Map<List<JobDto>>(jobs)).Returns(jobDtos);

        var result = await _jobRecommendationService.GetRecommendedJobsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
        Assert.Equal("TestJob", result.First().Title);
    }

    [Fact]
    public async Task JobRecommendationService_PollRecommendedJobsAfterDateAsync_ReturnsRecommendedJobs()
    {
        var userProfileId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(-1);
        var jobs = new List<JobEntity> { new JobEntity { Title = "TestJob" } };
        var jobDtos = new List<JobDto> { new JobDto { Title = "TestJob" } };
        var recommendation = new RecommendationEntity
        {
            Jobs = new List<JobRecommendationMapping> { new JobRecommendationMapping { JobId = jobs.First().Id } }
        };
        _mockRecommendationRepository.Setup(repo => repo.GetRecommendationAfterDateByUserProfileIdAsync(userProfileId, date)).ReturnsAsync(recommendation);
        _mockJobRepository.Setup(repo => repo.GetJobsByJobIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(jobs);
        _mockMapper.Setup(m => m.Map<List<JobDto>>(jobs)).Returns(jobDtos);

        var result = await _jobRecommendationService.PollRecommendedJobsAfterDateAsync(userProfileId, date);

        Assert.Single(result);
        Assert.Equal("TestJob", result.First().Title);
    }


    // SavedJobService Tests
    [Fact]
    public async Task SavedJobService_GetFilteredSavedJobsByUserProfileIdAsync_ReturnsFilteredSavedJobs()
    {
        var userProfileId = Guid.NewGuid();
        var jobFilterDto = new JobFilterDto();
        var jobFilterResultDto = new JobFilterResultDto { Jobs = new List<JobDto> { new JobDto { Title = "TestJob" } } };
        _mockSavedJobRepository.Setup(repo => repo.GetFilteredSavedJobsByUserProfileIdAsync(jobFilterDto, userProfileId))
            .ReturnsAsync(jobFilterResultDto);

        var result = await _savedJobService.GetFilteredSavedJobsByUserProfileIdAsync(jobFilterDto, userProfileId);

        Assert.Single(result.Jobs);
        Assert.Equal("TestJob", result.Jobs.First().Title);
    }

    [Fact]
    public async Task SavedJobService_GetSavedJobIdsByUserProfileIdAsync_ReturnsSavedJobIds()
    {

        var userProfileId = Guid.NewGuid();
        var savedJobs = new List<SavedJobEntity> { new SavedJobEntity { JobId = Guid.NewGuid() } };
        _mockSavedJobRepository.Setup(repo => repo.GetSavedJobsByUserProfileIdAsync(userProfileId)).ReturnsAsync(savedJobs);

        var result = await _savedJobService.GetSavedJobIdsByUserProfileIdAsync(userProfileId);

        Assert.Single(result);
        Assert.Equal(savedJobs.First().JobId, result.First());
    }

    [Fact]
    public async Task SavedJobService_AddSavedJobAsync_AddsSavedJob()
    {
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();

        await _savedJobService.AddSavedJobAsync(userProfileId, jobId);

        _mockSavedJobRepository.Verify(repo => repo.AddSavedJobAsync(It.IsAny<SavedJobEntity>()), Times.Once);
    }

    [Fact]
    public async Task SavedJobService_DeleteSavedJobAsync_DeletesSavedJob()
    {
        var userProfileId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var savedJob = new SavedJobEntity { UserProfileId = userProfileId, JobId = jobId };
        _mockSavedJobRepository.Setup(repo => repo.GetSavedJobsByUserProfileIdAndJobIdAsync(userProfileId, jobId)).ReturnsAsync(savedJob);

        await _savedJobService.DeleteSavedJobAsync(userProfileId, jobId);

        _mockSavedJobRepository.Verify(repo => repo.DeleteSavedJobAsync(savedJob), Times.Once);
    }

    // TagService Tests
    [Fact]
    public async Task TagService_GetAllTagsAsync_ReturnsAllTags()
    {
        var tags = new List<TagEntity>
    {
        new TagEntity { Name = "Tag1" },
        new TagEntity { Name = "Tag2" }
    };
        _mockTagRepository.Setup(repo => repo.GetAllTagsAsync()).ReturnsAsync(tags);

        var result = await _tagService.GetAllTagsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains("Tag1", result);
        Assert.Contains("Tag2", result);
    }

    // UserFeedbackService Tests
    [Fact]
    public async Task UserFeedbackService_GetFeedbackByTokenAndUserProfileIdAsync_ReturnsUserFeedback()
    {
        var token = "TestToken";
        var userProfileId = Guid.NewGuid();
        var feedback = new UserFeedbackEntity { Token = token, UserProfileId = userProfileId, Feedback = "TestFeedback" };
        _mockUserFeedbackRepository.Setup(repo => repo.GetUserFeedbackByTokenAsync(token, userProfileId)).ReturnsAsync(feedback);
        var feedbackDto = new UserFeedbackDto { Feedback = "TestFeedback" };
        _mockMapper.Setup(m => m.Map<UserFeedbackDto>(feedback)).Returns(feedbackDto);

        var result = await _userFeedbackService.GetFeedbackByTokenAndUserProfileIdAsync(token, userProfileId);

        Assert.Equal("TestFeedback", result.Feedback);
    }

    [Fact]
    public async Task UserFeedbackService_UpdateFeedbackAsync_UpdatesFeedback()
    {
        var userProfileId = Guid.NewGuid();
        var feedbackDto = new UserFeedbackDto { Token = "TestToken", Feedback = "UpdatedFeedback", CompanyRating = 4.5f, Company = new CompanyDto { Name = "TestCompany" } };
        var feedback = new UserFeedbackEntity { Token = "TestToken", UserProfileId = userProfileId, Feedback = "TestFeedback" };
        var company = new CompanyEntity { Name = "TestCompany", NumberOfRatings = 1, TotalRating = 4.0f, Rating = 4.0f };
        _mockUserFeedbackRepository.Setup(repo => repo.GetUserFeedbackByTokenAsync(feedbackDto.Token, userProfileId)).ReturnsAsync(feedback);
        _mockCompanyRepository.Setup(repo => repo.GetCompanyByNameAsync(feedbackDto.Company.Name)).ReturnsAsync(company);

        await _userFeedbackService.UpdateFeedbackAsync(feedbackDto, userProfileId);

        _mockUserFeedbackRepository.Verify(repo => repo.UpdateUserFeedbackAsync(It.Is<UserFeedbackEntity>(f => f.Feedback == feedbackDto.Feedback)), Times.Once);
        _mockCompanyRepository.Verify(repo => repo.UpdateCompanyAsync(It.Is<CompanyEntity>(c => c.Name == feedbackDto.Company.Name)), Times.Once);
    }
}
