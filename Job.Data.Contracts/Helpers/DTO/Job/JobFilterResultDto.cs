namespace Job.Data.Contracts.Helpers.DTO.Job;
public class JobFilterResultDto
{
    public ICollection<JobDto>? Jobs { get; set; }
    public ICollection<string>? Categories { get; set; }
    public ICollection<string>? Companies { get; set; }
    public ICollection<string>? ContractTypes { get; set; }
    public int TotalJobs { get; set; }
}
