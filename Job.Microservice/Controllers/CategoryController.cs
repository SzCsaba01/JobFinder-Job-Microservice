using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetAllCategories")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetMostVisitedCategoriesInLast30Days")]
    public async Task<IActionResult> GetMostVisitedCategoriesInLast30DaysAsync()
    {
        var categories = await _categoryService.GetMostVisitedCategoriesInLast30DaysAsync();
        return Ok(categories);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetMostSavedCategoriesInLast30Days")]
    public async Task<IActionResult> GetMostSavedCategoriesInLast30DaysAsync()
    {
        var categories = await _categoryService.GetMostSavedCategoriesInLast30DaysAsync();
        return Ok(categories);
    }
}
