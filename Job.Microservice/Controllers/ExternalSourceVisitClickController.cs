using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ExternalSourceVisitClickController : ControllerBase
{
    private readonly IExternalSourceVisitClickService _externalSourceVisitClickService;

    public ExternalSourceVisitClickController(IExternalSourceVisitClickService externalSourceVisitClickService)
    {
        _externalSourceVisitClickService = externalSourceVisitClickService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost("GetFilteredExternalSourceVisits")]
    public async Task<IActionResult> GetFilteredExternalSourceVisitsAsync([FromBody] JobFilterDto filteredJobsSearch)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        var externalSourceVisits = await _externalSourceVisitClickService.GetFilteredExternalSourceVisitsByUserProfileIdAsync(filteredJobsSearch, userProfileId);

        return Ok(externalSourceVisits);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost("ClickExternalSourceVisit")]
    public async Task<IActionResult> ClickExternalSourceVisitAsync([FromQuery] Guid jobId)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        await _externalSourceVisitClickService.AddExternalSourceVisitClickAsync(userProfileId, jobId);

        return Ok();
    }
}
