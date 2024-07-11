using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Data.Object.Entities;

[Table("Jobs")]
public class JobEntity
{
    public Guid Id { get; set; }

    public string? ExternalId { get; set; }

    public string? ExtrnalSource { get; set; }

    public bool isActive { get; set; }

    public bool isRemote { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(25000, ErrorMessage = "Description cannot be longer than 25000 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Company is required")]
    public string? CompanyName { get; set; }

    public string? ContractTypeName { get; set; }

    [Required(ErrorMessage = "Date posted is required")]
    public DateTime DatePosted { get; set; }

    [Required(ErrorMessage = "Categories are required")]
    public ICollection<JobCategoryMapping> Categories { get; set; }

    public DateTime? DateClosed { get; set; }

    public string? Url { get; set; }

    public CompanyEntity? Company { get; set; }

    public ContractTypeEntity? ContractType { get; set; }

    public ICollection<LocationEntity>? Locations { get; set; }

    public ICollection<JobTagMapping>? Tags { get; set; }

    public ICollection<SavedJobEntity> SavedJobs { get; set; }

    public ICollection<ExternalSourceVisitClickEntity> ExternalSourceVisitClicks { get; set; }

    public ICollection<UserFeedbackEntity> UserFeedbacks { get; set; }
}
