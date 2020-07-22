using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Mappers
{
    public static class EntityQueryGeoTaskExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this EntityQuery<GeoTask> from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                {
                    nameof(from.CurrentPrincipal),
                    from.CurrentPrincipal
                        .ToDictionary()
                        .ToList()
                        .Select(x => $"\"{x.Key}\"={x.Value}")
                },
                { nameof(from.Id), from.Id },
            };
        }
    }
}
