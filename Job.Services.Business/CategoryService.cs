using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Category;
using Job.Services.Contracts;

namespace Job.Services.Business;
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<string>> GetAllCategoriesAsync()
    {
       var categoryNames = (await _categoryRepository.GetAllCategoriesAsync())
           .Select(x => x.Name)
           .ToList();

        return categoryNames;
    }

    public async Task<List<CategoryDto>> GetMostVisitedCategoriesInLast30DaysAsync()
    {
        var categories = await _categoryRepository.GetMostVisitedCategoriesInLast30DaysAsync();

        var categoryDtos = categories
            .GroupBy(x => x.Name)
            .Select(x => new CategoryDto
            {
                Name = x.Key,
                Count = x.Count()
            })
            .Take(10)
            .OrderByDescending(x => x.Count)
            .ToList();

        return categoryDtos;
    }

    public async Task<List<CategoryDto>> GetMostSavedCategoriesInLast30DaysAsync()
    {
        var categories = await _categoryRepository.GetSavedCategoriesInLast30DaysAsync();

        var categoryDtos = categories
            .GroupBy(x => x.Name)
            .Select(x => new CategoryDto
            {
                Name = x.Key,
                Count = x.Count()
            })
            .Take(10)
            .OrderByDescending(x => x.Count)
            .ToList();

        return categoryDtos;
    }
}
