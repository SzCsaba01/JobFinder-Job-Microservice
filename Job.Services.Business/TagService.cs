using Job.Data.Contracts;
using Job.Services.Contracts;

namespace Job.Services.Business;
public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<string>> GetAllTagsAsync()
    {
        var tags = (await _tagRepository.GetAllTagsAsync())
            .Select(x => x.Name)
            .ToList();

        return tags;
    }
}
