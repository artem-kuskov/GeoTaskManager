using System.Collections.Generic;
using System.Security.Claims;
using _ApiEntityListQueryType =
    GeoTaskManager.Api.Geos.Models.ApiGeoListQuery;
using _EntityListQueryType =
    GeoTaskManager.Application.Geos.Queries.GeoListQuery;
using _EntityType = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Api.Geos.Mappers
{
    internal static class ApiGeoListQueryExtensions
    {
        public static _EntityListQueryType ToListQuery<TEntity>
            (this _ApiEntityListQueryType from,
            ClaimsPrincipal currentPrincipal)
            where TEntity : _EntityType
            => from is null
                ? null
                : new _EntityListQueryType
                {
                    Archived = from.Archived,
                    CurrentPrincipal = currentPrincipal,
                    ProjectId = from.ProjectId,
                    Limit = from.Limit,
                    Offset = from.Offset,
                    СontainsKeyWords = from.ContainsKeyWords,
                    DistanceLong = from.DistanceLong,
                    CenterLat = from.CenterLat,
                    CenterLong = from.CenterLong,
                    DistanceLat = from.DistanceLat,
                };

        public static Dictionary<string, object> ToDictionary
            (this _ApiEntityListQueryType from)
            => from is null
                ? null
                : new Dictionary<string, object>
                {
                    { nameof(from.Archived),  from.Archived },
                    { nameof(from.Limit),  from.Limit },
                    { nameof(from.Offset),  from.Offset },
                    { nameof(from.ContainsKeyWords),  from.ContainsKeyWords },
                    { nameof(from.ProjectId), from.ProjectId },
                    { nameof(from.DistanceLong), from.DistanceLong },
                    { nameof(from.CenterLat), from.CenterLat },
                    { nameof(from.CenterLong), from.CenterLong },
                    { nameof(from.DistanceLat), from.DistanceLat },
                };
    }
}
