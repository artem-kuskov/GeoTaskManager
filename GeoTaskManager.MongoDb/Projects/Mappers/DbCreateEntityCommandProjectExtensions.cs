using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.MongoDb.Projects.Mappers;
using System.Collections.Generic;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.MongoDb.Projects.Mappers
{
    internal static class DbCreateEntityCommandProjectExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbCreateCommand<_TEntity> from)
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
