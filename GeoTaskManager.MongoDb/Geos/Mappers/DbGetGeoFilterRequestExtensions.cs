using GeoTaskManager.Application.Geos.DbQueries;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Geos.Mappers
{
    internal static class DbGetGeoFilterRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbGetGeoFilterRequest from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Archived), from.Archived },
                { nameof(from.Contains), from.Contains },
                { nameof(from.Limit), from.Limit },
                { nameof(from.Offset), from.Offset },
                { nameof(from.DistanceLat), from.DistanceLat },
                { nameof(from.DistanceLong), from.DistanceLong },
                { nameof(from.CenterLat), from.CenterLat },
                { nameof(from.CenterLong), from.CenterLong },
                { nameof(from.ProjectId), from.ProjectId },
            };
        }
    }
}
