using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface ICompanyRepository
{
    public Task<CompanyEntity?> GetCompanyByNameAsync(string companyName);
    public Task<List<CompanyEntity>> GetMostVisitedCompaniesInLast30DaysAsync();
    public Task<List<CompanyEntity>> GetMostSavedCompaniesInLast30DaysAsync();
    public Task<List<CompanyEntity>> GetAllCompaniesAsync();
    public Task AddCompanyAsync(CompanyEntity company);
    public Task AddCompaniesAsync(List<CompanyEntity> company);
    public Task UpdateCompaniesAsync(List<CompanyEntity> company);
    public Task UpdateCompanyAsync(CompanyEntity company);
}
