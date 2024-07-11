using AutoMapper;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Company;
using Job.Services.Contracts;

namespace Job.Services.Business;
public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<List<string>> GetAllCompaniesAsync()
    {
        var companyNames = (await _companyRepository.GetAllCompaniesAsync())
            .Select(x => x.Name)
            .ToList();

        return companyNames;
    }

    public async Task<List<CompanyDto>> GetMostVisitedCompaniesInLast30DaysAsync()
    {
        var companies = await _companyRepository.GetMostVisitedCompaniesInLast30DaysAsync();

        var companiesDto = companies
            .GroupBy(x => x.Name)
            .Select(x => new CompanyDto
            {
                Name = x.Key,
                Logo = x.First().Logo,
                NumberOfRatings = x.First().NumberOfRatings,
                Rating = x.First().Rating,
                Count = x.Count()
            })
            .Take(10)
            .OrderByDescending(x => x.Count)
            .ToList();

        return companiesDto;
    }

    public async Task<List<CompanyDto>> GetMostSavedCompaniesInLast30DaysAsync()
    {
        var companies = await _companyRepository.GetMostSavedCompaniesInLast30DaysAsync();

        var companiesDto = companies
            .GroupBy(x => x.Name)
            .Select(x => new CompanyDto
            {
                Name = x.Key,
                Logo = x.First().Logo,
                NumberOfRatings = x.First().NumberOfRatings,
                Rating = x.First().Rating,
                Count = x.Count()
            })
            .Take(10)
            .OrderByDescending(x => x.Count)
            .ToList();

        return companiesDto;
    }
}
