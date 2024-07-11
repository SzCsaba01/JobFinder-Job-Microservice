using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access;
public class RecommendationRepository : IRecommendationRepository
{
    private readonly DataContext _dataContext;

    public RecommendationRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<RecommendationEntity>?> GetRecommendationsByUserProfileIdAsync(Guid userProfileId)
    {
        return await _dataContext.Recommendations
                    .Where(x => x.UserProfileId == userProfileId)
                    .Include(x => x.Jobs)
                    .AsNoTracking()
                    .ToListAsync();
    }

    public async Task<List<JobEntity>?> GetNewRecommendedJobsByUserProfileIdAsync(Guid userProfileId, int count)
    {
        var query = _dataContext.Recommendations
                    .Where(x => x.UserProfileId == userProfileId)
                    .Include(x => x.Jobs)
                        .ThenInclude(x => x.Job)
                    .OrderByDescending(x => x.CreatedDate)
                    .AsNoTracking()
                    .Take(count);
        
        var jobs = await query
                .Select(x => x.Jobs.Select(y => new JobEntity
                {
                    Id = y.Job.Id,
                    Title = y.Job.Title,
                    CompanyName = y.Job.CompanyName,
                    Company = y.Job.Company,
                    Locations = y.Job.Locations,
                    ContractType = y.Job.ContractType,
                    ContractTypeName = y.Job.ContractTypeName,
                    DatePosted = y.Job.DatePosted,
                    DateClosed = y.Job.DateClosed,
                    isActive = y.Job.isActive,
                    isRemote = y.Job.isRemote,
                    Categories = y.Job.Categories,
                    Url = y.Job.Url,
                }).ToList())
                .ToListAsync();

        return jobs.SelectMany(x => x).ToList();
    }

    public async Task<RecommendationEntity?> GetRecommendationAfterDateByUserProfileIdAsync(Guid userProfileId, DateTime date)
    {
        return await _dataContext.Recommendations
                    .Where(x => x.UserProfileId == userProfileId && x.CreatedDate > date)
                    .Include(x => x.Jobs)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
    }

    public async Task AddRecommendationAsync(RecommendationEntity recommendation)
    {
        await _dataContext.Recommendations.AddAsync(recommendation);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteRecommendationAsync(RecommendationEntity recommendation)
    {
        _dataContext.Remove(recommendation);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteRecommendationsAsync(List<RecommendationEntity> recommendations)
    {
        _dataContext.RemoveRange(recommendations);
        await _dataContext.SaveChangesAsync();
    }
}
