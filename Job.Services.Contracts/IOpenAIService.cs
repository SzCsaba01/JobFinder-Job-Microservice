using Job.Data.Contracts.Helpers.DTO.Recommendation;

namespace Job.Services.Contracts;
public interface IOpenAIService
{
    public Task<string> RecommendJobsAsync(DetailsForJobRecommendationsDto detailsForJobRecommendations);
}
