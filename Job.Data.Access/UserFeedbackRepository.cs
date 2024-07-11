using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class UserFeedbackRepository : IUserFeedbackRepository
{
    private readonly DataContext _dataContext;

    public UserFeedbackRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<UserFeedbackEntity?> GetUserFeedbackByTokenAsync(string token, Guid userProfileId)
    {
        return await _dataContext.UserFeedbacks
                .Where(x => x.UserProfileId == userProfileId && x.Token == token && x.FeedbackDate == null)
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }

    public async Task<List<UserFeedbackEntity>?> GetFilteredFeedbacksAsync(UserFeedbackFilterDto userFeedbackFilter)
    {
        var querry = _dataContext.UserFeedbacks
                .Include(x => x.Job)
                    .ThenInclude(x => x.Categories)
                .Include(x => x.Job)
                    .ThenInclude(x => x.ContractType)
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .AsNoTracking();

        if (userFeedbackFilter.StartDate != null)
        {
            querry = querry.Where(x => x.FeedbackDate > userFeedbackFilter.StartDate);
        }

        if (userFeedbackFilter.EndDate != null)
        {
            querry = querry.Where(x => x.FeedbackDate < userFeedbackFilter.EndDate);
        }

        if (userFeedbackFilter.MinRating != null)
        {
            querry = querry.Where(x => x.CompanyRating >= userFeedbackFilter.MinRating);
        }

        if (userFeedbackFilter.MaxRating != null)
        {
            querry = querry.Where(x => x.CompanyRating <= userFeedbackFilter.MaxRating);
        }

        if (userFeedbackFilter.ApplicationStatuses is not null && userFeedbackFilter.ApplicationStatuses.Any())
        {
            querry = querry.Where(x => userFeedbackFilter.ApplicationStatuses.Contains(x.ApplicationStatus));
        }

        if (userFeedbackFilter.Companies is not null && userFeedbackFilter.Companies.Any())
        {
            querry = querry.Where(x => userFeedbackFilter.Companies.Contains(x.Job.CompanyName));
        }

        if (userFeedbackFilter.Categories is not null && userFeedbackFilter.Categories.Any())
        {
            querry = querry.Where(x => x.Job.Categories.Any(y => userFeedbackFilter.Categories.Contains(y.CategoryName)));
        }

        if (userFeedbackFilter.ContractTypes is not null && userFeedbackFilter.ContractTypes.Any())
        {
            querry = querry.Where(x => userFeedbackFilter.ContractTypes.Contains(x.Job.ContractType.Name));
        }

        if (userFeedbackFilter.JobId is not null)
        {
            querry = querry.Where(x => x.JobId == userFeedbackFilter.JobId);
        }

        return await querry.ToListAsync();
    }

    public async Task<List<UserFeedbackEntity>?> GetUserFeedbacksByUserProfileIdAsync(Guid userProfileId)
    {
        return await _dataContext.UserFeedbacks
                .Where(x => x.UserProfileId == userProfileId)
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task AddUserFeedbacksAsync(List<UserFeedbackEntity> userFeedback)
    {
        await _dataContext.UserFeedbacks.AddRangeAsync(userFeedback);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteUserFeedbacksAsync(List<UserFeedbackEntity> userFeedbacks)
    {
        _dataContext.RemoveRange(userFeedbacks);
        await _dataContext.SaveChangesAsync();
    }

    public async Task UpdateUserFeedbackAsync(UserFeedbackEntity userFeedback)
    {
        _dataContext.UserFeedbacks.Update(userFeedback);
        await _dataContext.SaveChangesAsync();
    }
}
