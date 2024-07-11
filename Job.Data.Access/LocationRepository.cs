using Job.Data.Access.Data;
using Job.Data.Contracts;
using Job.Data.Object.Entities;

namespace Job.Data.Access;
public class LocationRepository : ILocationRepository
{
    private readonly DataContext _dataContext;

    public LocationRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task AddLocationsAsync(ICollection<LocationEntity> locationEntities)
    {
        await _dataContext.Locations.AddRangeAsync(locationEntities);
        await _dataContext.SaveChangesAsync();
    }

    public async Task AddLocationAsync(LocationEntity locationEntity)
    {
        await _dataContext.Locations.AddAsync(locationEntity);
        await _dataContext.SaveChangesAsync();
    }
}
