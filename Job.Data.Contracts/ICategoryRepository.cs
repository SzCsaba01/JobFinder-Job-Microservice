using Job.Data.Contracts.Helpers.DTO.Category;
using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface ICategoryRepository
{
    public Task<List<CategoryEntity>?> GetCategoriesByNamesAsync(ICollection<string> categoryNames);
    public Task<List<CategoryEntity>> GetMostVisitedCategoriesInLast30DaysAsync();
    public Task<List<CategoryEntity>> GetSavedCategoriesInLast30DaysAsync();
    public Task<List<CategoryEntity>> GetAllCategoriesAsync();
    public Task AddCategoriesAsync(List<CategoryEntity> categories);
}
