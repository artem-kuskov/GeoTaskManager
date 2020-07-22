using GeoTaskManager.Application.Actors.DbQueries;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Actors.Mappers
{
    internal static class DbGetActorByNameRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbGetActorByNameRequest from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.UserName), from.UserName}
            };
        }
    }
}
