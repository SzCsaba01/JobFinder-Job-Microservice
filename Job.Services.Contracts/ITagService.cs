namespace Job.Services.Contracts;
public interface ITagService
{
    public Task<List<string>> GetAllTagsAsync();
}
