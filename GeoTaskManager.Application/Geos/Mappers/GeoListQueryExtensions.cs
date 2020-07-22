using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Geos.DbQueries;
using GeoTaskManager.Application.Geos.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

// Type alias
using _TEntity =
    GeoTaskManager.Application.Geos.Queries.GeoListQuery;

namespace GeoTaskManager.Application.Geos.Mappers
{
    public static class GeoListQueryExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this _TEntity from)
            => from is null
                ? null
                : new Dictionary<string, object>
                {
                    { nameof(from.Archived), from.Archived },
                    { nameof(from.Limit), from.Limit},
                    { nameof(from.Offset), from.Offset },
                    { nameof(from.ProjectId), from.ProjectId },
                    { nameof(from.DistanceLat), from.DistanceLat },
                    { nameof(from.DistanceLong), from.DistanceLong },
                    { nameof(from.CenterLat), from.CenterLat },
                    { nameof(from.CenterLong), from.CenterLong },
                    { nameof(from.СontainsKeyWords), from.СontainsKeyWords},
                    { nameof(from.CurrentPrincipal),
                        String.Join(',',
                            from.CurrentPrincipal
                                .ToDictionary()
                                .Select(x => $"{x.Key}={x.Value}"))},
                };

        public static DbGetGeoFilterRequest ToDbGetGeoFilterRequest
            (this _TEntity from, int defaultLimit, int maxLimit)
            => from is null
                ? null
                : new DbGetGeoFilterRequest
                {
                    Offset = Math.Max(0, from.Offset),
                    Limit = from.Limit <= 0
                        ? defaultLimit
                        : Math.Min(from.Limit, maxLimit),
                    Archived = from.Archived,
                    ProjectId = from.ProjectId,
                    Contains = from.СontainsKeyWords,
                    DistanceLat = from.DistanceLat,
                    DistanceLong = from.DistanceLong,
                    CenterLat = from.CenterLat,
                    CenterLong = from.CenterLong
                };
    }
}
