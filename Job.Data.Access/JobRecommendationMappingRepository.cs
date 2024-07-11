using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class JobRecommendationMappingRepository : IJobRecommendationMappingRepository
{
    private readonly DataContext _dataContext;

    public JobRecommendationMappingRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<JobRecommendationMapping>?> GetJobRecommendationMappingsByUserProfileIdAsync(Guid userProfileId)
    {
        return await _dataContext.Recommendations
            .Where(x => x.UserProfileId == userProfileId)
            .OrderByDescending(x => x.CreatedDate)
            .Take(3)
            .Include(x => x.Jobs)
                .ThenInclude(x => x.Job)
            .SelectMany(x => x.Jobs)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddJobRecommendationMappingsAsync(ICollection<JobRecommendationMapping> recommendation)
    {
        await _dataContext.JobRecommendationMappings.AddRangeAsync(recommendation);
        await _dataContext.SaveChangesAsync();
    }
}
