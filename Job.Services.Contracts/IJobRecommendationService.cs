using Job.Data.Contracts.Helpers.DTO.Job;

namespace Job.Services.Contracts;
public interface IJobRecommendationService
{
    public Task<List<JobDto>> GetRecommendedJobsByUserProfileIdAsync(Guid userProfileId);
    public Task<List<JobDto>> PollRecommendedJobsAfterDateAsync(Guid userProfileId, DateTime date);
}
