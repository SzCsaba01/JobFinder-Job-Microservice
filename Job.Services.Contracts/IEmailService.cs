using Job.Data.Contracts.Helpers.DTO.Message;

namespace Job.Services.Contracts;
public interface IEmailService
{
    public Task SendFeedbackEmailsAsync();
}
