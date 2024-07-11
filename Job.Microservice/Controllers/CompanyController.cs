using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetAllCompanies")]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(companies);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetMostVisitedCompaniesInLast30Days")]
    public async Task<IActionResult> GetMostVisitedCompaniesInLast30DaysAsync()
    {
        var companies = await _companyService.GetMostVisitedCompaniesInLast30DaysAsync();
        return Ok(companies);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetMostSavedCompaniesInLast30Days")]
    public async Task<IActionResult> GetMostSavedCompaniesInLast30DaysAsync()
    {
        var companies = await _companyService.GetMostSavedCompaniesInLast30DaysAsync();
        return Ok(companies);
    }
}
