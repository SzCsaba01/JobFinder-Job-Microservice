using Job.Data.Contracts.Helpers.Enums;

namespace Job.Data.Contracts.Helpers.DTO.Feedback;
public class UserFeedbackFilterDto
{
    public ICollection<string>? Categories { get; set; }
    public ICollection<string>? Companies { get; set; }
    public ICollection<string>? ContractTypes { get; set; }
    public Guid? JobId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set;}
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public ICollection<ApplicationStatusEnum>? ApplicationStatuses { get; set; }
}
