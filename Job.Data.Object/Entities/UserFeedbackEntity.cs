using Job.Data.Contracts.Helpers.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("UserFeedbacks")]
public class UserFeedbackEntity
{
    public Guid UserProfileId { get; set; }

    public Guid JobId { get; set; }

    [Range(0, 5, ErrorMessage = "Rating should be between 0 and 5")]
    public float CompanyRating { get; set; }

    [Required(ErrorMessage = "Feedback is required.")]
    [MaxLength(500, ErrorMessage = "Feedback should not exceed 500 characters.")]
    public string? Feedback { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public ApplicationStatusEnum ApplicationStatus { get; set; }

    public string Token { get; set; }

    public JobEntity Job { get; set; }
}
