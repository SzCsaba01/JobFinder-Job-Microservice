using Job.Data.Contracts.Helpers.DTO.Feedback;

namespace Job.Services.Contracts;
public interface IUserFeedbackService
{
    public Task<UserFeedbackFilterResultDto> GetFilteredFeedbacksAsync(UserFeedbackFilterDto userFeedbackFilter);
    public Task<UserFeedbackDto> GetFeedbackByTokenAndUserProfileIdAsync(string token, Guid userProfileId);
    public Task UpdateFeedbackAsync(UserFeedbackDto feedback, Guid userProfileId);
}
