using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Geos.Mappers;
using System.Collections.Generic;
using System.Linq;

// Type alias
using _TCreateCommand =
    GeoTaskManager.Application.Geos.Commands.GeoCreateCommand;

namespace GeoTaskManager.Application.Geos.Mappers
{
    public static class GeoCreateCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this _TCreateCommand from)
            => from is null
                ? null
                : new Dictionary<string, object>
                {
                    { nameof(from.Description),  from.Description },
                    { nameof(from.IsArchived),  from.IsArchived},
                    { nameof(from.Title),  from.Title},
                    { nameof(from.GeoJson),  from.GeoJson},
                    { nameof(from.ProjectId),  from.ProjectId},
                    {
                        nameof(from.CurrentPrincipal),
                        string.Join(',', from.CurrentPrincipal
                            .ToDictionary()
                            .ToList()
                            .Select(x => $"\"{x.Key}\"={x.Value}"))
                    },
                };
    }
}
