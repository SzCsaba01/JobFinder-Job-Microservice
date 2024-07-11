using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface IContractTypeRepository
{
    public Task<ContractTypeEntity> GetContractTypeByNameAsync(string contractTypeName);
    public Task<List<ContractTypeEntity>> GetAllContractTypesAsync();
    public Task AddContractTypeAsync(ContractTypeEntity contractType);
    public Task AddContractTypesAsync(List<ContractTypeEntity> contractTypes);
}
