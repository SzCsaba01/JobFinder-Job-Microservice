using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface IJobRecommendationMappingRepository
{
    public Task<List<JobRecommendationMapping>?> GetJobRecommendationMappingsByUserProfileIdAsync(Guid userProfileId);
    public Task AddJobRecommendationMappingsAsync(ICollection<JobRecommendationMapping> recommendation);
}
