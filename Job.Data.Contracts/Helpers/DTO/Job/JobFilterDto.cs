namespace Job.Data.Contracts.Helpers.DTO.Job;
public class JobFilterDto
{
    public string? Title { get; set; }
    public bool? isRemote { get; set; }
    public ICollection<string>? Categories { get; set; }
    public ICollection<string>? Companies { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ContractType { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
