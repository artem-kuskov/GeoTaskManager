using GeoTaskManager.Api.Core.Helpers;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using System.Linq;
using _ApiEntityType = GeoTaskManager.Api.Geos.Models.ApiGeo;
using _EntityType = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Api.Geos.Mappers
{
    internal static class GeoListResponseExtensions
    {
        public static ApiList<_ApiEntityType> ToEntityList<TEntity>
            (this ListResponse<_EntityType> from)
            where TEntity : _ApiEntityType
            => from is null
                ? null
                : new ApiList<_ApiEntityType>
                {
                    TotalCount = from.TotalCount,
                    Entities =
                    {
                        from.Entities.Select(x => x.ToEntity<_ApiEntityType>())
                    }
                };
    }
}
