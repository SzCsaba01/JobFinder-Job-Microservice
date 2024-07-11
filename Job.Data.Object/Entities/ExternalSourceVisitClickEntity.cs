using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("ExternalSourceVisitClicks")]
public class ExternalSourceVisitClickEntity
{
    public Guid UserProfileId { get; set; }

    public Guid JobId { get; set; }

    [Required(ErrorMessage = "ClickDate is required.")]
    public DateTime ClickDate { get; set; }

    public bool isFeedbackMailSent { get; set; }

    public JobEntity Job { get; set; }
}
