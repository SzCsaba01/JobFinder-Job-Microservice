namespace Job.Services.Contracts;
public interface IContractTypeService
{
    public Task<List<string>> GetAllContractTypesAsync();
}
