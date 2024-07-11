using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface IExternalSourceVisitClickRepository
{
    public Task<List<ExternalSourceVisitClickEntity>> GetExternalSourceVisitClicksWhereEmailNotSentAsync();
    public Task<JobFilterResultDto> GetFilteredExternalSourceVisitsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId);
    public Task<ExternalSourceVisitClickEntity?> GetExternalSourceVisitClickByUserProfileIdAndJobIdAsync(Guid userProfileId, Guid jobId);
    public Task<List<ExternalSourceVisitClickEntity>?> GetExternalSourceVisitClicksByUserProfileIdAsync(Guid userProfileId);
    public Task<List<ExternalSourceVisitClickEntity>?> GetExternalSourceVisitClicksForRecommendationsByUserProfileIdAsync(Guid userProfileId);
    public Task AddExternalSourceVisitClickAsync(ExternalSourceVisitClickEntity externalSourceVisitClick);
    public Task UpdateExternalSourceVisitClicksAsync(List<ExternalSourceVisitClickEntity> externalSourceVisitClicks);
    public Task DeleteExternalSourceVisitClicksAsync(List<ExternalSourceVisitClickEntity> externalSourceVisitClicks);
}
