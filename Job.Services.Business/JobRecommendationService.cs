using AutoMapper;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Job.Services.Business.Exceptions;
using Job.Services.Contracts;
using Project.Services.Business.Exceptions;

namespace Job.Services.Business;
public class JobRecommendationService : IJobRecommendationService
{
    private readonly IJobRecommendationMappingRepository _jobRecommendationMappingRepository;
    private readonly IRecommendationRepository _recommendationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IMapper _mapper;

    public JobRecommendationService
        (
            IRecommendationRepository recommendationRepository,
            IJobRecommendationMappingRepository jobRecommendationMappingRepository, 
            IJobRepository jobRepository,
            IMapper mapper
        )
    {
        _recommendationRepository = recommendationRepository;
        _jobRecommendationMappingRepository = jobRecommendationMappingRepository;
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<List<JobDto>> GetRecommendedJobsByUserProfileIdAsync(Guid userProfileId)
    {
        var jobs = await _recommendationRepository.GetNewRecommendedJobsByUserProfileIdAsync(userProfileId, 3);

        var mappedJobs = _mapper.Map<List<JobDto>>(jobs);

        return mappedJobs;
    }

    public async Task<List<JobDto>> PollRecommendedJobsAfterDateAsync(Guid userProfileId, DateTime date)
    {
        var startTime = DateTime.UtcNow;
        var jobs = new List<JobEntity>();

        for (int attempts = 0; attempts < 15; attempts++)
        {
            var recommendation = await _recommendationRepository.GetRecommendationAfterDateByUserProfileIdAsync(userProfileId, date);

            if (recommendation is not null)
            {
                jobs = await _jobRepository.GetJobsByJobIdsAsync(recommendation.Jobs.Select(x => x.JobId).ToList());
                if (jobs is not null && !jobs.Any())
                {
                    throw new RecommendationException("Sorry! We couldn't find new recommendations for you. Please try later !");
                }
                break;
            }

            await Task.Delay(1000);
        }

        if (jobs is null || !jobs.Any())
        {
            throw new RecommendationException("You can generate new recommendations only 3 times a day. Please try later !");
        }

        var mappedJobs = _mapper.Map<List<JobDto>>(jobs);

        return mappedJobs;
    }
}
