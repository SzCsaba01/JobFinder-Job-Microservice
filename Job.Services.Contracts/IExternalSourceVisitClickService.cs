using Job.Data.Contracts.Helpers.DTO.Job;

namespace Job.Services.Contracts;
public interface IExternalSourceVisitClickService
{
    public Task<JobFilterResultDto> GetFilteredExternalSourceVisitsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId);
    public Task AddExternalSourceVisitClickAsync(Guid userProfileId, Guid jobId);
}
