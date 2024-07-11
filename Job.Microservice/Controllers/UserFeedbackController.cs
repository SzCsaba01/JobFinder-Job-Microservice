using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserFeedbackController : ControllerBase
{
    private readonly IUserFeedbackService _userFeedbackService;

    public UserFeedbackController(IUserFeedbackService userFeedbackService)
    {
        _userFeedbackService = userFeedbackService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("GetFilteredFeedbacks")]
    public async Task<IActionResult> GetFilteredFeedbacksAsync([FromBody] UserFeedbackFilterDto userFeedbackFilter)
    {
        var userFeedbacks = await _userFeedbackService.GetFilteredFeedbacksAsync(userFeedbackFilter);

        return Ok(userFeedbacks);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetFeedbackByToken")]
    public async Task<IActionResult> GetFeedbackByTokenAsync([FromQuery] string token)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        var userFeedback = await _userFeedbackService.GetFeedbackByTokenAndUserProfileIdAsync(token, userProfileId);

        return Ok(userFeedback);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("UpdateFeedback")]
    public async Task<IActionResult> UpdateFeedbackAsync([FromBody] UserFeedbackDto userFeedback)
    {
        var userProfileId = new Guid(User.FindFirst("Id").Value);

        await _userFeedbackService.UpdateFeedbackAsync(userFeedback, userProfileId);

        var message = new { message = "Feedback added successfully!" };

        return Ok();
    }
}
