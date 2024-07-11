using Job.Data.Contracts.Helpers.DTO.Job;

namespace Job.Services.Contracts;
public interface IJobService
{
    public Task<JobFilterResultDto> GetFilteredJobsPaginatedAsync(JobFilterDto jobFilterDto);
    public Task<string> GetJobDescriptionByJobIdAsync(Guid jobId);
    public Task AddJobsFromAPIsAsync();
    public Task AddJobAsync(JobDto job);
    public Task DeactivateJobsAsync();
    public Task DeleteJobByJobIdAsync(Guid jobId);
}
