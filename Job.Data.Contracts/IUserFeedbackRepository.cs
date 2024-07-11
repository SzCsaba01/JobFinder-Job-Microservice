using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface IUserFeedbackRepository
{
    public Task<List<UserFeedbackEntity>?> GetFilteredFeedbacksAsync(UserFeedbackFilterDto userFeedbackFilter);
    public Task<List<UserFeedbackEntity>?> GetUserFeedbacksByUserProfileIdAsync(Guid userProfileId);
    public Task<UserFeedbackEntity?> GetUserFeedbackByTokenAsync(string token, Guid userProfileId);
    public Task AddUserFeedbacksAsync(List<UserFeedbackEntity> userFeedback);
    public Task UpdateUserFeedbackAsync(UserFeedbackEntity userFeedback);
    public Task DeleteUserFeedbacksAsync(List<UserFeedbackEntity> userFeedbacks);
}
