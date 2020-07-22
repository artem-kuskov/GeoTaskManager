using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Geos.Mappers;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Geos.Mappers
{
    internal static class DbUpdateCommandGeoExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbUpdateCommand<Geo> from)
        {
            if (from is null)
            {
                return null;
            }

            var dict = new Dictionary<string, object>();

            if (from.Entity is null)
            {
                return dict;
            }

            var entityDict = from.Entity.ToDictionary();
            foreach (var item in entityDict)
            {
                dict.TryAdd(item.Key, item.Value);
            }

            return dict;
        }
    }
}
