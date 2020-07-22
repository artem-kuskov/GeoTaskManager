using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.Projects.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

// Type alias
using _TEntity =
    GeoTaskManager.Application.Geos.Commands.GeoUpdateCommand;

namespace GeoTaskManager.Application.Geos.Mappers
{
    public static class GeoUpdateCommandExtensions
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
                    { nameof(from.GeoJson), from.GeoJson },
                    { nameof(from.CurrentPrincipal),
                        String.Join(',', from.CurrentPrincipal
                            .ToDictionary()
                            .ToList()
                            .Select(x => $"\"{x.Key}\"={x.Value}"))
                    },
                };
    }
}
