using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetAllTags")]
    public async Task<IActionResult> GetAllTagsAsync()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Ok(tags);
    }
}
