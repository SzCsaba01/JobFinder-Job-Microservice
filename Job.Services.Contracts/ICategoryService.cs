using Job.Data.Contracts.Helpers.DTO.Category;

namespace Job.Services.Contracts;
public interface ICategoryService
{
    public Task<List<string>> GetAllCategoriesAsync();
    public Task<List<CategoryDto>> GetMostVisitedCategoriesInLast30DaysAsync();
    public Task<List<CategoryDto>> GetMostSavedCategoriesInLast30DaysAsync();
}
