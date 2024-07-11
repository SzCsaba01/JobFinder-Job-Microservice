using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("SavedJobs")]
public class SavedJobEntity
{
    public Guid UserProfileId { get; set; }

    public Guid JobId { get; set; }

    public DateTime SavedDate { get; set; }

    public JobEntity Job { get; set; }
}
