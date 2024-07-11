using Job.Data.Object.Entities;

namespace Job.Data.Contracts;
public interface ILocationRepository
{
    public Task AddLocationsAsync(ICollection<LocationEntity> locationEntities);
    public Task AddLocationAsync(LocationEntity locationEntity);
}
