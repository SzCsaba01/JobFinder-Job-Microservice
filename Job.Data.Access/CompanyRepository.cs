using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class CompanyRepository : ICompanyRepository
{
    private readonly DataContext _dataContext;

    public CompanyRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<CompanyEntity?> GetCompanyByNameAsync(string companyName)
    {
        return await _dataContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == companyName);
    }

    public async Task<List<CompanyEntity>> GetMostVisitedCompaniesInLast30DaysAsync()
    {
        return await _dataContext.ExternalSourceVisitClicks
            .Where(x => x.ClickDate >= DateTime.Now.AddDays(-50))
            .Include(x => x.Job)
                .ThenInclude(x => x.Company)
            .Select(x => x.Job.Company)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<CompanyEntity>> GetMostSavedCompaniesInLast30DaysAsync()
    {
        return await _dataContext.SavedJobs
            .Where(x => x.SavedDate >= DateTime.Now.AddDays(-50))
            .Include(x => x.Job)
                .ThenInclude(x => x.Company)
            .Select(x => x.Job.Company)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<CompanyEntity>> GetAllCompaniesAsync()
    {
        return await _dataContext.Companies
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task AddCompanyAsync(CompanyEntity company)
    {
        await _dataContext.AddAsync(company);
        await _dataContext.SaveChangesAsync();
    }

    public async Task AddCompaniesAsync(List<CompanyEntity> company)
    {
        await _dataContext.Companies.AddRangeAsync(company);
        await _dataContext.SaveChangesAsync();
    }

    public async Task UpdateCompaniesAsync(List<CompanyEntity> company)
    {
        var companyNames = company.Select(x => x.Name).ToList();
        var oldCompanies = await _dataContext.Companies
            .Where(x => companyNames.Contains(x.Name))
            .ToListAsync();

        foreach (var oldCompany in oldCompanies)
        {
            var newCompany = company.FirstOrDefault(x => x.Name == oldCompany.Name);
            if (newCompany is not null)
            {
                oldCompany.Logo = newCompany.Logo;
            }
        }

        if (oldCompanies.Any())
        {
            _dataContext.Companies.UpdateRange(oldCompanies);
        }

        await _dataContext.SaveChangesAsync();
    }

    public async Task UpdateCompanyAsync(CompanyEntity company)
    {
        _dataContext.Companies.Update(company);
        await _dataContext.SaveChangesAsync();
    }
}