using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("Locations")]
public class LocationEntity
{
    public Guid Id { get; set; }

    public Guid? JobId { get; set; }

    public string? Region { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public JobEntity? Job { get; set; }
}
