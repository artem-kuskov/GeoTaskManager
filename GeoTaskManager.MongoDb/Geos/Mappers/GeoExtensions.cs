using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Geos.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeoTaskManager.MongoDb.Geos.Mappers
{
    internal static class GeoExtensions
    {
        public static DbGeo ToEntity<TEntity>
            (this Geo from)
            where TEntity : DbGeo
            => from is null
                ? null
                : new DbGeo
                {
                    IsArchived = from.IsArchived,
                    ProjectId = from.ProjectId,
                    CreatedAt = from.CreatedAt,
                    CreatedBy = from.CreatedBy,
                    Description = from.Description,
                    Id = from.Id,
                    Title = from.Title,
                    GeoJson = BsonSerializer
                        .Deserialize
                            <GeoJsonFeatureCollection
                                <GeoJson2DGeographicCoordinates>>(from.GeoJson)
                };
    }
}
