using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("JobCategoryMappings")]
public class JobCategoryMapping
{
    public Guid JobId { get; set; }

    public JobEntity Job { get; set; }

    public string CategoryName { get; set; }

    public CategoryEntity Category { get; set; }
}

