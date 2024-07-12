using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Job;
using Job.Data.Object.Entities;
using Job.Services.Contracts;
using Project.Services.Business.Exceptions;

namespace Job.Services.Business;
public class SavedJobService : ISavedJobService
{
    private readonly ISavedJobRepository _savedJobRepository;

    public SavedJobService(ISavedJobRepository savedJobRepository)
    {
        _savedJobRepository = savedJobRepository;
    }

    public async Task<JobFilterResultDto> GetFilteredSavedJobsByUserProfileIdAsync(JobFilterDto jobFilterDto, Guid userProfileId)
    {
        var filteredSavedJobs = await _savedJobRepository.GetFilteredSavedJobsByUserProfileIdAsync(jobFilterDto, userProfileId);

        return filteredSavedJobs;
    }

    public async Task<List<Guid>> GetSavedJobIdsByUserProfileIdAsync(Guid userProfileId)
    {
        var savedJobs = await _savedJobRepository.GetSavedJobsByUserProfileIdAsync(userProfileId);

        if (savedJobs == null)
        {
            return new List<Guid>();
        }

        var savedJobIds = savedJobs.Select(x => x.JobId).ToList();

        return savedJobIds;
    }

    public async Task AddSavedJobAsync(Guid userProfileId, Guid jobId)
    {
        var savedJob = new SavedJobEntity
        {
            UserProfileId = userProfileId,
            SavedDate = DateTime.Now,
            JobId = jobId
        };

        await _savedJobRepository.AddSavedJobAsync(savedJob);
    }

    public async Task DeleteSavedJobAsync(Guid userProfileId, Guid jobId)
    {
        var savedJob = await _savedJobRepository.GetSavedJobsByUserProfileIdAndJobIdAsync(userProfileId, jobId);

        if (savedJob is null)
        {
            throw new ModelNotFoundException("Saved job not found!");
        }

        await _savedJobRepository.DeleteSavedJobAsync(savedJob);
    }
}
