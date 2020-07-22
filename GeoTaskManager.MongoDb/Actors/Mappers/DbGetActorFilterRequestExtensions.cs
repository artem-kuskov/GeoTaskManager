using GeoTaskManager.Application.Actors.DbQueries;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Actors.Mappers
{
    internal static class DbGetActorFilterRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbGetActorFilterRequest from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.ActorRoleMask), from.ActorRoleMask},
                { nameof(from.Archived), from.Archived },
                { nameof(from.Contains), from.Contains },
                { nameof(from.Limit), from.Limit },
                { nameof(from.Offset), from.Offset },
            };
        }
    }
}
