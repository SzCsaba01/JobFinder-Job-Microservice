using Job.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.Microservice.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ContractTypeController : ControllerBase
{
    private readonly IContractTypeService _contractTypeService;

    public ContractTypeController(IContractTypeService contractTypeService)
    {
        _contractTypeService = contractTypeService;
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("GetAllContractTypes")]
    public async Task<IActionResult> GetAllCotractTypesAsync()
    {
        var result = await _contractTypeService.GetAllContractTypesAsync();
        return Ok(result);
    }
}
