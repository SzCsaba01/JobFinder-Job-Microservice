using Job.Services.Contracts;
using Quartz;

namespace Job.Services.Quartz;
public class JobsDeactivationJob : IJob
{
    private readonly IJobService _jobService;

    public JobsDeactivationJob(IJobService jobService)
    {
        _jobService = jobService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _jobService.DeactivateJobsAsync();
    }
}
