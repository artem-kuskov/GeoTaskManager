using GeoTaskManager.Api.Actors.Mappers;
using _ApiEntityType = GeoTaskManager.Api.Geos.Models.ApiGeo;
using _EntityType = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Api.Geos.Mappers
{
    internal static class GeoExtensions
    {
        public static _ApiEntityType ToEntity<TEntity>(this _EntityType from)
            where TEntity : _ApiEntityType
            => from is null
                ? null
                : new _ApiEntityType
                {
                    Description = from.Description,
                    Id = from.Id,
                    Title = from.Title,
                    CreatedAt = from.CreatedAt,
                    CreatedBy = from.CreatedBy.ToApiActor(),
                    IsArchived = from.IsArchived,
                    GeoJson = from.GeoJson,
                    ProjectId = from.ProjectId
                };
    }
}
