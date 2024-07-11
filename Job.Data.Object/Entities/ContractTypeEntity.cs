using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("ContractTypes")]
public class ContractTypeEntity
{
    [Key]
    public string Name { get; set; }

    public ICollection<JobEntity>? Jobs { get; set; }
}

