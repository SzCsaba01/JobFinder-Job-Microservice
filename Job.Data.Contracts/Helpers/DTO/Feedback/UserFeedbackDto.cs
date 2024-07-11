using Job.Data.Contracts.Helpers.DTO.Company;
using Job.Data.Contracts.Helpers.Enums;

namespace Job.Data.Contracts.Helpers.DTO.Feedback;
public class UserFeedbackDto
{
    public CompanyDto Company { get; set; }
    public string JobTitle { get; set; }
    public float CompanyRating { get; set; }
    public string Feedback { get; set; }
    public DateTime FeedbackDate { get; set; }
    public ApplicationStatusEnum ApplicationStatus { get; set; }
    public string? ContractType { get; set; }
    public string Token { get; set; }
}
