using GeoTaskManager.Application.Projects.DbQueries;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Projects.Mappers
{
    internal static class DbGetProjectFilterRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbGetProjectFilterRequest from)
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
            };
        }
    }
}
