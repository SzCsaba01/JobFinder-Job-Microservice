using Job.Data.Contracts;
using Job.Services.Contracts;

namespace Job.Services.Business;
public class ContractTypeService : IContractTypeService
{
    private readonly IContractTypeRepository _contractTypeRepository;

    public ContractTypeService(IContractTypeRepository contractTypeRepository)
    {
        _contractTypeRepository = contractTypeRepository;
    }

    public async Task<List<string>> GetAllContractTypesAsync()
    {
        var contractTypeNames = (await _contractTypeRepository.GetAllContractTypesAsync())
            .Select(x => x.Name)
            .ToList();

        return contractTypeNames;
    }
}
