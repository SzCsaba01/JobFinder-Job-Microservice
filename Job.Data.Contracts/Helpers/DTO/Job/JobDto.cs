using Job.Data.Contracts.Helpers.DTO.Company;
using Location.Data.Access.Helpers.DTO.City;
using Microsoft.AspNetCore.Http;

namespace Job.Data.Contracts.Helpers.DTO.Job;
public class JobDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public CompanyDto? Company { get; set; }
    public string CompanyName { get; set; }
    public IFormFile? CompanyLogoFile { get; set; }
    public string Url { get; set; }
    public string? ContractTypeName { get; set; }
    public DateTime DatePosted { get; set; }
    public DateTime? DateClosed { get; set; }
    public bool isActive { get; set; }
    public bool isRemote { get; set; }
    public ICollection<string>? Categories { get; set; }
    public ICollection<string>? Tags { get; set; }
    public ICollection<LocationDto>? Locations { get; set; }
}
