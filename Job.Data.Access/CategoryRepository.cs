using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _dataContext;

    public CategoryRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<CategoryEntity>?> GetCategoriesByNamesAsync(ICollection<string> categoryNames)
    {
        return await _dataContext.Categories
            .AsNoTracking()
            .Where(x => categoryNames.Contains(x.Name))
            .ToListAsync();
    }

    public async Task<List<CategoryEntity>> GetMostVisitedCategoriesInLast30DaysAsync()
    {
        return await _dataContext.ExternalSourceVisitClicks
            .Where(x => x.ClickDate >= DateTime.Now.AddDays(-50))
            .Include(x => x.Job)
                .ThenInclude(x => x.Categories)
            .SelectMany(click => click.Job.Categories).Select(x => x.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<CategoryEntity>> GetSavedCategoriesInLast30DaysAsync()
    {
        return await _dataContext.SavedJobs
            .Where(x => x.SavedDate >= DateTime.Now.AddDays(-50))
            .Include(x => x.Job)
                .ThenInclude(x => x.Categories)
            .SelectMany(click => click.Job.Categories).Select(x => x.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<CategoryEntity>> GetAllCategoriesAsync()
    {
        return await _dataContext.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddCategoriesAsync(List<CategoryEntity> categories)
    {
        await _dataContext.Categories.AddRangeAsync(categories);
        await _dataContext.SaveChangesAsync();
    }
}
