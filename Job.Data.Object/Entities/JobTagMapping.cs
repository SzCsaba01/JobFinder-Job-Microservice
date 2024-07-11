using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;
[Table("JobTagMappings")]
public class JobTagMapping
{
    public Guid JobId { get; set; }

    public string TagName{ get; set; }

    public JobEntity Job { get; set; }

    public TagEntity Tag { get; set; }
}
