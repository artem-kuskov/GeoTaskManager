using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.MongoDb.Geos.Mappers;
using System.Collections.Generic;
using System.Linq;

// Type alias
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.MongoDb.Geos.Mappers
{
    internal static class DbCreateEntityCommandGeoExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbCreateCommand<_TEntity> from)
        {
            if (from is null)
            {
                return null;
            }

            if (from.Entity is null)
            {
                return new Dictionary<string, object>();
            }

            var dict = new Dictionary<string, object>();

            var entityDict = from.Entity.ToDictionary();
            foreach (var item in entityDict)
            {
                dict.TryAdd(item.Key, item.Value);
            }

            return dict;
        }
    }
}
