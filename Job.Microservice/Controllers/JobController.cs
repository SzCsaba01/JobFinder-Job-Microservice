using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost("GetFilteredJobsPaginated")]
    public async Task<IActionResult> GetFilteredJobsPaginatedAsync([FromBody] JobFilterDto filteredJobsSearch)
    {
        var result = await _jobService.GetFilteredJobsPaginatedAsync(filteredJobsSearch);
        return Ok(result);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetJobDescription")]
    public async Task<IActionResult> GetJobDescriptionAsync([FromQuery] Guid jobId)
    {
        var result = await _jobService.GetJobDescriptionByJobIdAsync(jobId);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("AddJob")]
    public async Task<IActionResult> AddJobAsync([FromForm] JobDto job)
    {
        await _jobService.AddJobAsync(job);

        var message = new { message = "Job added successfully!" };

        return Ok(message);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("DeleteJob")]
    public async Task<IActionResult> DeleteJobAsync([FromQuery] Guid jobId)
    {
        await _jobService.DeleteJobByJobIdAsync(jobId);

        var message = new { message = "Job deleted successfully!" };

        return Ok(message);
    }
}
