using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class TagRepository : ITagRepository
{
    private readonly DataContext _dataContext;

    public TagRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task AddTagsAsync(List<TagEntity> tags)
    {
        await _dataContext.Tags.AddRangeAsync(tags);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<List<TagEntity>?> GetTagsByNamesAsync(ICollection<string> tagNames)
    {
        return await _dataContext.Tags
                .Where(x => tagNames.Contains(x.Name))
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<List<TagEntity>> GetAllTagsAsync()
    {
        return await _dataContext.Tags
                .AsNoTracking()
                .ToListAsync();
    }

}