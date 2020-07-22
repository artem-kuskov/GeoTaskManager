using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Geos.Models;
using MongoDB.Bson;

namespace GeoTaskManager.MongoDb.Geos.Mappers
{
    internal static class DbGeoExtensions
    {
        public static Geo ToEntity<TEntity>
            (this DbGeo from)
            where TEntity : Geo
            => from is null
                ? null
                : new Geo
                {
                    IsArchived = from.IsArchived,
                    ProjectId = from.ProjectId,
                    CreatedAt = from.CreatedAt,
                    CreatedBy = from.CreatedBy,
                    Description = from.Description,
                    Id = from.Id,
                    Title = from.Title,
                    GeoJson = from.GeoJson.ToJson()
                };
    }
}
