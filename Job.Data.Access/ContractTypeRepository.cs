using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class ContractTypeRepository : IContractTypeRepository
{
    private readonly DataContext _dataContext;

    public ContractTypeRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<ContractTypeEntity?> GetContractTypeByNameAsync(string contractTypeName)
    {
        return await _dataContext.ContractTypes
            .Where(x => x.Name == contractTypeName)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<ContractTypeEntity>> GetAllContractTypesAsync()
    {
        return await _dataContext.ContractTypes
            .Where(x => x.Name != null && x.Name != string.Empty)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task AddContractTypeAsync(ContractTypeEntity contractType)
    {
        await _dataContext.ContractTypes.AddAsync(contractType);
        await _dataContext.SaveChangesAsync();
    }
    public async Task AddContractTypesAsync(List<ContractTypeEntity> contractTypes)
    {
        await _dataContext.ContractTypes.AddRangeAsync(contractTypes);
        await _dataContext.SaveChangesAsync();
    }
}