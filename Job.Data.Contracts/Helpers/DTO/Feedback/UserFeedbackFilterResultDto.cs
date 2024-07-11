namespace Job.Data.Contracts.Helpers.DTO.Feedback;
public class UserFeedbackFilterResultDto
{
    public ICollection<UserFeedbackDto>? Feedbacks { get; set; }
    public ICollection<UserFeedbackJobDetailsDto>? Jobs { get; set; }
    public ICollection<string>? Categories { get; set; }
    public ICollection<string>? Companies { get; set; }
    public ICollection<string>? ContractTypes { get; set; }
}
