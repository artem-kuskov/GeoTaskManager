using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Projects.Mappers;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Actors.Mappers
{
    internal static class DbCreateEntityCommandActorExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbCreateCommand<Actor> from)
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
