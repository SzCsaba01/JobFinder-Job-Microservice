using AutoMapper;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Message;
using Job.Data.Contracts.Helpers.DTO.Recommendation;
using Job.Data.Object.Entities;
using Job.Services.Contracts;
using System.Text.RegularExpressions;

namespace Job.Services.Business;
public class QueueMessageProcesserService : IQueueMessageProcesserService
{
    private readonly IJobRepository _jobRepository;
    private readonly IExternalSourceVisitClickRepository _jobApplicationClickRepository;
    private readonly IJobRecommendationMappingRepository _jobRecommendationMappingRepository;
    private readonly IRecommendationRepository _recommendationRepository;
    private readonly IUserFeedbackRepository _userFeedbackRepository;
    private readonly ISavedJobRepository _savedJobRepository;
    private readonly IOpenAIService _openAIService;
    private readonly IMapper _mapper;

    public QueueMessageProcesserService
        (
            ISavedJobRepository savedJobRepository,
            IOpenAIService openAIService,
            IMapper mapper,
            IJobRepository jobRepository,
            IExternalSourceVisitClickRepository jobApplicationClickRepository,
            IJobRecommendationMappingRepository jobRecommendationMappingRepository,
            IRecommendationRepository recommendationRepository,
            IUserFeedbackRepository userFeedbackRepository
        )
    {
        _savedJobRepository = savedJobRepository;
        _openAIService = openAIService;
        _mapper = mapper;
        _jobRepository = jobRepository;
        _jobApplicationClickRepository = jobApplicationClickRepository;
        _jobRecommendationMappingRepository = jobRecommendationMappingRepository;
        _recommendationRepository = recommendationRepository;
        _userFeedbackRepository = userFeedbackRepository;
    }

    public async Task RecommendJobsAsync(UserMessageDetailsDto userDetails)
    {
        var userRecommendations = await _recommendationRepository.GetRecommendationsByUserProfileIdAsync(userDetails.UserProfileId);

        if (userRecommendations is not null && userRecommendations.Any())
        {
            var numberOfRecommendationsToday = userRecommendations.Count(x => x.CreatedDate.Date == DateTime.Now.Date);
            if (numberOfRecommendationsToday >= 3)
            {
                return;
            }
        }

        var savedJobs = await _savedJobRepository.GetSavedJobsForRecommendationsByUserProfileIdAsync(userDetails.UserProfileId);

        var savedJobIds = new List<Guid>();
        if (savedJobs is not null && savedJobs.Any())
        {
            savedJobIds = savedJobs.Select(x => x.JobId).ToList();
        }

        var appliedJobs = await _jobApplicationClickRepository.GetExternalSourceVisitClicksForRecommendationsByUserProfileIdAsync(userDetails.UserProfileId);

        var appliedJobIds = new List<Guid>();
        if (appliedJobs is not null && appliedJobs.Any())
        {
            appliedJobIds = appliedJobs.Select(x => x.JobId).ToList();
        }

        var alreadyRecommendedJobs = await _jobRecommendationMappingRepository.GetJobRecommendationMappingsByUserProfileIdAsync(userDetails.UserProfileId);

        var alreadyRecommendedJobIds = new List<Guid>();
        if (alreadyRecommendedJobs is not null && alreadyRecommendedJobs.Any())
        {
            alreadyRecommendedJobIds = alreadyRecommendedJobs.Select(x => x.JobId).ToList();
        }

        var detailsForJobRecommendations = new DetailsForJobRecommendationsDto
        {
            UserMessageDetails = userDetails,
            SavedJobIds = savedJobIds,
            ExternalSourceVisitIds = appliedJobIds,
            RecommendedJobIds = alreadyRecommendedJobIds
        };

        var jobs = new List<JobDto>();
        var existingJobIds = new List<Guid>();
        var numberOfJobs = 250;
        var numberOfRecommendations = 1;
        for (var number = 0; number < numberOfRecommendations; number++)
        {
            jobs = (await _jobRepository.GetJobsWithoutDetailsForRecommendationsAsync(number, numberOfJobs)).Select(x => _mapper.Map<JobDto>(x)).ToList();

            detailsForJobRecommendations.Jobs = jobs;

            try
            {
                var response = await _openAIService.RecommendJobsAsync(detailsForJobRecommendations);
                var recommendedJobIds = ProcessRecommendationsString(response);

                existingJobIds.AddRange(await _jobRepository.GetExistingJobIdsByJobIdsAsync(recommendedJobIds));
            }
            catch (Exception)
            {
                continue;
            }
        }

        existingJobIds = existingJobIds.Distinct().ToList();

        existingJobIds = existingJobIds.Except(alreadyRecommendedJobIds).ToList();

        var newJobRecommendation = new RecommendationEntity
        {
            UserProfileId = userDetails.UserProfileId,
            CreatedDate = DateTime.Now
        };

        await _recommendationRepository.AddRecommendationAsync(newJobRecommendation);

        var newJobRecommendationMappings = existingJobIds
            .Select(x => new JobRecommendationMapping
            {
                JobId = x,
                RecommendationId = newJobRecommendation.Id
            })
            .ToList();

        await _jobRecommendationMappingRepository.AddJobRecommendationMappingsAsync(newJobRecommendationMappings);
    }

    public async Task DeleteUserAsync(Guid userProfileId)
    {
        if (userProfileId != Guid.Empty)
        {
            var savedJobs = await _savedJobRepository.GetSavedJobsByUserProfileIdAsync(userProfileId);
            if (savedJobs != null && savedJobs.Any())
            {
                await _savedJobRepository.DeleteSavedJobsAsync(savedJobs);
            }

            var jobApplicationClicks = await _jobApplicationClickRepository.GetExternalSourceVisitClicksByUserProfileIdAsync(userProfileId);
            if (jobApplicationClicks != null && jobApplicationClicks.Any())
            {
                await _jobApplicationClickRepository.DeleteExternalSourceVisitClicksAsync(jobApplicationClicks);
            }

            var recommendations = await _recommendationRepository.GetRecommendationsByUserProfileIdAsync(userProfileId);
            if (recommendations != null && recommendations.Any())
            {
                await _recommendationRepository.DeleteRecommendationsAsync(recommendations);
            }

            var userFeedbacks = await _userFeedbackRepository.GetUserFeedbacksByUserProfileIdAsync(userProfileId);
            if (userFeedbacks != null && userFeedbacks.Any())
            {
                await _userFeedbackRepository.DeleteUserFeedbacksAsync(userFeedbacks);
            }
        }
    }

    private List<Guid> ProcessRecommendationsString(string result)
    {
        string[] lines = result.Split('\n');

        string pattern = @"\[Recommended Job Ids: (.+)\]";
        Regex regex = new Regex(pattern);

        List<Guid> recommendedJobIds = new List<Guid>();

        foreach (string line in lines)
        {
            Match match = regex.Match(line.Trim());
            if (match.Success)
            {
                string jobIdsString = match.Groups[1].Value;
                string[] jobIds = jobIdsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string jobIdString in jobIds)
                {
                    if (Guid.TryParse(jobIdString.Trim(), out Guid jobId))
                    {
                        recommendedJobIds.Add(jobId);
                    }
                }
                break;
            }
        }

        recommendedJobIds = recommendedJobIds.Distinct().ToList();

        return recommendedJobIds;
    }
}
