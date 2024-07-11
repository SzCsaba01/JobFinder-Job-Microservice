using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface ITagRepository
{
    public Task<List<TagEntity>?> GetTagsByNamesAsync(ICollection<string> tagNames);
    public Task<List<TagEntity>> GetAllTagsAsync();
    public Task AddTagsAsync(List<TagEntity> tags);
}
