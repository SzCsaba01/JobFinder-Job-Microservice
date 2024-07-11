using Job.Data.Contracts.Helpers.DTO.Message;

namespace Job.Services.Contracts;
public interface IQueueMessageSenderService
{
    public Task SendUserFeedbackEmailsAsync(JobEmailMessageDto message);
}
