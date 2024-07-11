using Job.Services.Contracts;
using Quartz;

namespace Job.Services.Quartz;
public class AddJobsFromAPIsJob : IJob
{
    private readonly IJobService _jobService;

    public AddJobsFromAPIsJob(IJobService jobService)
    {
        _jobService = jobService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _jobService.AddJobsFromAPIsAsync();
    }
}
