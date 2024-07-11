using Job.Services.Contracts;
using Quartz;

namespace Job.Services.Quartz;
public class SendUserFeedbackEmailsJob : IJob
{
    private readonly IEmailService _emailService;

    public SendUserFeedbackEmailsJob(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _emailService.SendFeedbackEmailsAsync();
    }
}
