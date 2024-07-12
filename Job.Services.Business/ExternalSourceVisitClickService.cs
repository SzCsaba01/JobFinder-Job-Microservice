using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Job.Services.Contracts;
using Project.Services.Business.Exceptions;

namespace Job.Services.Business;
public class ExternalSourceVisitClickService : IExternalSourceVisitClickService
{
    private readonly IExternalSourceVisitClickRepository _externalSourceVisitClickClickRepository;

    public ExternalSourceVisitClickService(IExternalSourceVisitClickRepository externalSourceVisitClickRepository)
    {
        _externalSourceVisitClickClickRepository = externalSourceVisitClickRepository;
    }
    public async Task<JobFilterResultDto> GetFilteredExternalSourceVisitsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId)
    {
        var filteredAppliedJobs = await _externalSourceVisitClickClickRepository.GetFilteredExternalSourceVisitsByUserProfileIdAsync(jobFilterDto, userProfileId);

        return filteredAppliedJobs;
    }

    public async Task AddExternalSourceVisitClickAsync(Guid userProfileId, Guid jobId)
    {
        var existingClick = await _externalSourceVisitClickClickRepository.GetExternalSourceVisitClickByUserProfileIdAndJobIdAsync(userProfileId, jobId);

        if (existingClick is not null)
        {
            return;
        }

        var jobApplicationClick = new ExternalSourceVisitClickEntity
        {
            UserProfileId = userProfileId,
            JobId = jobId,
            ClickDate = DateTime.Now,
            isFeedbackMailSent = false
        };

        await _externalSourceVisitClickClickRepository.AddExternalSourceVisitClickAsync(jobApplicationClick);
    }
}
