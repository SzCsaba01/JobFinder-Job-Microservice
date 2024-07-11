using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface IRecommendationRepository
{
    public Task<List<RecommendationEntity>?> GetRecommendationsByUserProfileIdAsync(Guid userProfileId);
    public Task<List<JobEntity>?> GetNewRecommendedJobsByUserProfileIdAsync(Guid userProfileId, int count);
    public Task<RecommendationEntity?> GetRecommendationAfterDateByUserProfileIdAsync(Guid userProfileId, DateTime date);
    public Task AddRecommendationAsync(RecommendationEntity recommendation);
    public Task DeleteRecommendationAsync(RecommendationEntity recommendation);
    public Task DeleteRecommendationsAsync(List<RecommendationEntity> recommendations);
}
