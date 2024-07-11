using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Job.Data.Object.Entities;

[Table("Companies")]
public class CompanyEntity
{
    [Key]
    public string Name { get; set; }

    public string? Logo { get; set; }

    [Range(0, 5, ErrorMessage = "Rating should be between 0 and 5")]
    public float Rating { get; set; }

    public float TotalRating { get; set; }

    public int NumberOfRatings { get; set; }

    public ICollection<JobEntity>? Jobs { get; set; }
}
