using Job.Data.Contracts.Helpers.DTO.Message;

namespace Job.Services.Contracts;
public interface IQueueMessageProcesserService
{
    public Task RecommendJobsAsync(UserMessageDetailsDto userDetails);
    public Task DeleteUserAsync(Guid userProfileId);
}
