using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("JobRecommendations")]
public class RecommendationEntity
{
    public Guid Id { get; set; }

    public Guid  UserProfileId { get; set; }

    public ICollection<JobRecommendationMapping> Jobs { get; set;}

    [Required(ErrorMessage = "CreatedDate is required.")]
    public DateTime CreatedDate { get; set; }
}
