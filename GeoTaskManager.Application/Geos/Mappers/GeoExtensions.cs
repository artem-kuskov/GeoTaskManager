using GeoTaskManager.Application.Actors.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

// Type alias
using _TEntity =
    GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Mappers
{
    public static class GeoExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this _TEntity from)
            => from is null
                ? null
                : new Dictionary<string, object>
                {
                    { nameof(from.Id), from.Id },
                    { nameof(from.Description),  from.Description },
                    { nameof(from.IsArchived),  from.IsArchived},
                    { nameof(from.Title),  from.Title},
                    { nameof(from.ProjectId), from.ProjectId },
                    { nameof(from.GeoJson), from.GeoJson },
                    { nameof(from.CreatedAt), from.CreatedAt },
                    { nameof(from.CreatedBy),
                        String.Join(',', from.CreatedBy
                            .ToDictionary()
                            .Select(x => $"{x.Key}={x.Value}"))}
                };
    }
}
