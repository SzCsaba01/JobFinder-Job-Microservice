using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SavedJobController : ControllerBase
{
    private readonly ISavedJobService _savedJobService;

    public SavedJobController(ISavedJobService savedJobService)
    {
        _savedJobService = savedJobService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost("GetFilteredSavedJobs")]
    public async Task<IActionResult> GetFilteredSavedJobsAsync([FromBody] JobFilterDto filteredJobsSearch)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        var savedJobs = await _savedJobService.GetFilteredSavedJobsByUserProfileIdAsync(filteredJobsSearch, userProfileId);

        return Ok(savedJobs);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetSavedJobIds")]
    public async Task<IActionResult> GetSavedJobIdsAsync()
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        var savedJobIds = await _savedJobService.GetSavedJobIdsByUserProfileIdAsync(userProfileId);

        return Ok(savedJobIds);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost("SaveJob")]
    public async Task<IActionResult> SaveJobAsync([FromQuery] Guid jobId)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        await _savedJobService.AddSavedJobAsync(userProfileId, jobId);

        return Ok();
    }

    [Authorize(Roles = "Admin, User")]
    [HttpDelete("UnsaveJob")]
    public async Task<IActionResult> UnsaveJobAsync([FromQuery] Guid jobId)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        await _savedJobService.DeleteSavedJobAsync(userProfileId, jobId);

        return Ok();
    }
}
