using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("JobRecommendationMappings")]
public class JobRecommendationMapping
{
    public Guid RecommendationId { get; set; }

    public Guid JobId { get; set; }

    public RecommendationEntity Recommendation { get; set; }

    public JobEntity Job { get; set; }
}
