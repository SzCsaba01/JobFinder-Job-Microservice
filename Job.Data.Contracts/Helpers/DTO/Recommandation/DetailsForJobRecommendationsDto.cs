using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Contracts.Helpers.DTO.Message;

namespace Job.Data.Contracts.Helpers.DTO.Recommendation;
public class DetailsForJobRecommendationsDto
{
    public UserMessageDetailsDto UserMessageDetails { get; set; }
    public List<JobDto> Jobs { get; set; }
    public List<Guid> SavedJobIds { get; set; }
    public List<Guid> ExternalSourceVisitIds { get; set; }
    public List<Guid> RecommendedJobIds { get; set; }
}
