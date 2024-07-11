using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Job.Data.Object.Entities;

[Table("Categories")]
public class CategoryEntity
{
    [Key]
    public string Name { get; set; }

    public ICollection<JobCategoryMapping>? Jobs { get; set; }
}