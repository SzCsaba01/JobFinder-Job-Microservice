using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class JobRecommendationController : ControllerBase
{
    private readonly IJobRecommendationService _jobRecommendationService;

    public JobRecommendationController(IJobRecommendationService jobRecommendationService)
    {
        _jobRecommendationService = jobRecommendationService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetRecommendedJobs")]
    public async Task<IActionResult> GetRecommendedJobsByUserProfileId()
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        var recommendedJobs = await _jobRecommendationService.GetRecommendedJobsByUserProfileIdAsync(userProfileId);
        return Ok(recommendedJobs);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("PollNewRecommendedJobs")]
    public async Task<IActionResult> PollRecommendedJobsAfterDateAsync()
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        var date = DateTime.Now;
        var recommendedJobs = await _jobRecommendationService.PollRecommendedJobsAfterDateAsync(userProfileId, date);
        return Ok(recommendedJobs);
    }
}
