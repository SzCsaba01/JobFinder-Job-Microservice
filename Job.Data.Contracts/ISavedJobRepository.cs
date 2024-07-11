using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface ISavedJobRepository
{
    public Task<JobFilterResultDto> GetFilteredSavedJobsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId);
    public Task<SavedJobEntity?> GetSavedJobsByUserProfileIdAndJobIdAsync(Guid userProfileId, Guid jobId);
    public Task<List<SavedJobEntity>?> GetSavedJobsByUserProfileIdAsync(Guid userProfileId);
    public Task<List<SavedJobEntity>?> GetSavedJobsForRecommendationsByUserProfileIdAsync(Guid userProfileId);
    public Task AddSavedJobAsync(SavedJobEntity savedJob);
    public Task DeleteSavedJobAsync(SavedJobEntity savedJob);
    public Task DeleteSavedJobsAsync(List<SavedJobEntity> savedJobs);
}
