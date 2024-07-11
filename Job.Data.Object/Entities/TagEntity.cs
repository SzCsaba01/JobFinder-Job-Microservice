using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Job.Data.Object.Entities;

[Table("Tags")]
public class TagEntity
{
    [Key]
    public string Name { get; set; }

    public ICollection<JobTagMapping> Jobs { get; set; }
}

