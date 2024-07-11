using Job.Data.Contracts.Helpers.DTO.Company;

namespace Job.Services.Contracts;
public interface ICompanyService
{
    public Task<List<string>> GetAllCompaniesAsync();
    public Task<List<CompanyDto>> GetMostVisitedCompaniesInLast30DaysAsync();
    public Task<List<CompanyDto>> GetMostSavedCompaniesInLast30DaysAsync();
}
