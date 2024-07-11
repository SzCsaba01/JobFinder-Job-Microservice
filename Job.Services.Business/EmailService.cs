using Job.Data.Contracts;
using Job.Data.Contracts.Helpers;
using Job.Data.Contracts.Helpers.DTO.Email;
using Job.Data.Contracts.Helpers.DTO.Message;
using Job.Data.Object.Entities;
using Job.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace Job.Services.Business;
public class EmailService : IEmailService
{
    private readonly IExternalSourceVisitClickRepository _jobApplicationClickRepository;
    private readonly IUserFeedbackRepository _userFeedbackRepository;
    private readonly ITokenService _tokenService;
    private readonly IQueueMessageSenderService _queueMessageSender;

    public EmailService(
            IExternalSourceVisitClickRepository jobApplicationClickRepository,
            IUserFeedbackRepository userFeedbackRepository,
            ITokenService tokenService,
            IQueueMessageSenderService queueMessageSender
        )
    {
        _jobApplicationClickRepository = jobApplicationClickRepository;
        _userFeedbackRepository = userFeedbackRepository;
        _tokenService = tokenService;
        _queueMessageSender = queueMessageSender;
    }

    public async Task SendFeedbackEmailsAsync()
    {
        var jobApplicationClicks = await _jobApplicationClickRepository.GetExternalSourceVisitClicksWhereEmailNotSentAsync();
        var feedbacks = new List<UserFeedbackEntity>();
        var jobEmailMessageDto = new JobEmailMessageDto();
        jobEmailMessageDto.Data = new List<EmailDto>();

        foreach (var jobApplicationClick in jobApplicationClicks)
        {
            var date = jobApplicationClick.ClickDate.ToString("yyyy-MM-dd-HH-mm-ss");
            var token = await _tokenService.GenerateRandomTokenAsync();
            var feedback = new UserFeedbackEntity
            {
                UserProfileId = jobApplicationClick.UserProfileId,
                JobId = jobApplicationClick.JobId,
                Token = $"{token}_{date}",
                Feedback = "",
            };
            feedbacks.Add(feedback);

            var emailDto = new EmailDto
            {
                UserProfileId = jobApplicationClick.UserProfileId,
                Subject = "Feedback - " + jobApplicationClick.Job.Title, 
                Body = GenerateFeedbackEmailBody(feedback.Token)
            };
            jobEmailMessageDto.Data.Add(emailDto);

            jobApplicationClick.isFeedbackMailSent = true;
            jobApplicationClick.Job = null;
        }
        await _jobApplicationClickRepository.UpdateExternalSourceVisitClicksAsync(jobApplicationClicks);
        await _userFeedbackRepository.AddUserFeedbacksAsync(feedbacks);
        await _queueMessageSender.SendUserFeedbackEmailsAsync(jobEmailMessageDto);
    }

    private string GenerateFeedbackEmailBody(string token)
    {
        var uri = $"{AppConstants.FE_APP_FEEDBACK_URL}/{token}";
        return $"<a href='{uri}'>Click here to give feedback</a>";
    }
}
