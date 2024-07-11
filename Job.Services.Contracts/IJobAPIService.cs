using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;

namespace Job.Services.Contracts;
public interface IJobAPIService
{
    public Task<List<JobEntity>> GetAdzunaJobsAsync(JobFilterDto jobSearchDto);
    public Task<List<JobEntity>> GetJobicyJobsAsync(JobFilterDto jobSearchDto);
    public Task<List<JobEntity>> GetRemotiveJobsAsync(JobFilterDto jobSearchDto);
}
