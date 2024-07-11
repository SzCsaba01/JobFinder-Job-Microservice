using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface IJobRepository
{
    public Task<List<Guid>> GetExistingJobIdsByJobIdsAsync(List<Guid> jobIds);
    public Task<List<JobEntity>> GetJobsByJobIdsAsync(List<Guid> jobIds);
    public Task<List<JobEntity>> GetJobsForRecommendationAsync(int page, int pageSize);
    public Task<List<JobEntity>> GetJobsWithoutDetailsForRecommendationsAsync(int page, int pageSize);
    public Task<JobEntity?> GetJobByJobIdAsync(Guid jobId);
    public Task<string?> GetJobDescriptionByJobIdAsync(Guid jobId);
    public Task<List<JobEntity>> GetAllJobsAsync();
    public Task<List<JobEntity>> GetAllJobsForDeactivation();
    public Task<JobFilterResultDto> GetFilteredJobsAsync(JobFilterDto jobFilterDto);
    public Task AddJobAsync(JobEntity job);
    public Task AddJobsAsync(List<JobEntity> jobs);
    public Task UpdateAPIJobsAsync(List<JobEntity> jobs);
    public Task UpdateJobsAsync(List<JobEntity> jobs);
    public Task DeleteJobAsync(JobEntity job);
}
