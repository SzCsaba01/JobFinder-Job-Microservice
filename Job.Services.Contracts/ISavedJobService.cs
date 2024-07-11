using Job.Data.Contracts.Helpers.DTO.Job;

namespace Job.Services.Contracts;
public interface ISavedJobService
{
    public Task<JobFilterResultDto> GetFilteredSavedJobsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId);
    public Task<List<Guid>> GetSavedJobIdsByUserProfileIdAsync(Guid userProfileId);
    public Task AddSavedJobAsync(Guid userProfileId, Guid jobId);
    public Task DeleteSavedJobAsync(Guid userProfileId, Guid jobId);
}
